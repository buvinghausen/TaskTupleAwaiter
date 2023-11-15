using TaskTupleAwaiter.Tests.Adapters;
using Xunit;

namespace TaskTupleAwaiter.Tests;

public static class BehaviorComparisonTests
{
	static BehaviorComparisonTests()
	{
		var canceled = new TaskCompletionSource<object>();
		canceled.SetCanceled();
		CanceledTask = canceled.Task;
	}

	private static Task<object> CompletedTask { get; } = Task.FromResult<object>(null);
	private static Task<object> CanceledTask { get; }
	private static Task<object> FailedTask { get; } = TaskFromException(new DummyException());

	public static IEnumerable<object[]> EachArity() =>
		Enumerable.Range(1, 10).Select(arity => new object[] { arity });

	public static IEnumerable<object[]> EachIndexForEachArity() =>
		Enumerable.Range(1, 10).SelectMany(arity => Enumerable.Range(0, arity - 1), (arity, whichToWaitFor) => new object[] { arity, whichToWaitFor });

	[Theory]
	[MemberData(nameof(EachIndexForEachArity))]
	private static async Task WaitsForAllTasksToCompleteWhenAllSucceed(int arity, int whichToWaitFor)
	{
		var source = new TaskCompletionSource<object>();

		var tasks = (
			from index in Enumerable.Range(0, arity)
			select index == whichToWaitFor ? source.Task : CompletedTask).ToArray();

		var adapters = AwaiterAdapter.CreateAllAdapters(tasks);

		AssertAllAdapters(adapters, adapter => !adapter.IsCompleted);

		source.SetResult(null);

		foreach (var adapter in adapters)
			await adapter;

		AssertAllAdapters(adapters, adapter => adapter.IsCompleted);
	}

	[Theory]
	[MemberData(nameof(EachIndexForEachArity))]
	private static async Task WaitsForAllTasksToCompleteWhenAllCancel(int arity, int whichToWaitFor)
	{
		var source = new TaskCompletionSource<object>();

		var tasks = Enumerable.Range(0, arity)
			.Select(index =>
				index == whichToWaitFor ? source.Task : CanceledTask).ToArray();

		var adapters = AwaiterAdapter.CreateAllAdapters(tasks);

		AssertAllAdapters(adapters, adapter => !adapter.IsCompleted);

		source.SetCanceled();

		foreach (var adapter in adapters)
			await Assert.ThrowsAnyAsync<TaskCanceledException>(async () => await adapter);

		AssertAllAdapters(adapters, adapter => adapter.IsCompleted);
	}

	[Theory]
	[MemberData(nameof(EachIndexForEachArity))]
	private static async Task WaitsForAllTasksToCompleteWhenAllFail(int arity, int whichToWaitFor)
	{
		var source = new TaskCompletionSource<object>();

		var tasks = Enumerable.Range(0, arity)
			.Select(index =>
				index == whichToWaitFor ? source.Task : FailedTask).ToArray();

		var adapters = AwaiterAdapter.CreateAllAdapters(tasks);

		AssertAllAdapters(adapters, adapter => !adapter.IsCompleted);

		source.SetException(new DummyException());

		foreach (var adapter in adapters)
			await Assert.ThrowsAnyAsync<DummyException>(async () => await adapter);

		AssertAllAdapters(adapters, adapter => adapter.IsCompleted);
	}

	[Theory]
	[MemberData(nameof(EachArity))]
	private static void CompletesSynchronouslyIfAllTasksWereCompletedSynchronously(int arity)
	{
		var tasks = Enumerable.Repeat(CompletedTask, arity).ToArray();

		var adapters = AwaiterAdapter.CreateAllAdapters(tasks);

		AssertAllAdapters(adapters, adapter => adapter.IsCompleted);
	}

	[Theory]
	[MemberData(nameof(EachArity))]
	private static void ResultsAreInCorrectOrder(int arity)
	{
		var tasks = Enumerable.Range(0, arity)
			.Select(index => Task.FromResult<object>(index)).ToArray();

		var adapters = AwaiterAdapter.CreateNonVoidResultAdapters(tasks);

		AssertAllAdapters(adapters, adapter =>
		{
			var result = adapter.GetResult();

			for (var i = 0; i < arity; i++)
				if (!i.Equals((int)result[i]))
					return false;

			return true;
		});
	}

#if !NET8_0_OR_GREATER
	[Theory]
	[MemberData(nameof(EachArity))]
	private static void FirstExceptionIsThrown(int arity)
	{
		var sources = Enumerable.Range(0, arity).Select(_ => new TaskCompletionSource<object>()).ToArray();

		var adapters = AwaiterAdapter.CreateAllAdapters(sources.Select(source => source.Task).ToArray());

		for (var i = sources.Length - 1; i >= 0; i--) sources[i].SetException(new DummyException());

		AssertAllAdapters(adapters, adapter => ReferenceEquals(sources[0].Task.Exception?.InnerException, Assert.ThrowsAny<DummyException>(adapter.GetResult)));
	}
#endif

	[Theory]
	[MemberData(nameof(EachArity))]
	private static Task NonConfiguredAwaitUsesSynchronizationContext(int arity) =>
		AssertUsesSynchronizationContext(arity, null, true);

	[Theory]
	[MemberData(nameof(EachArity))]
	private static Task ConfigureAwaitTrueUsesSynchronizationContext(int arity) =>
		AssertUsesSynchronizationContext(arity, true, true);

	[Theory]
	[MemberData(nameof(EachArity))]
	private static Task ConfigureAwaitFalseDoesNotUseSynchronizationContext(int arity) =>
		AssertUsesSynchronizationContext(arity, false, false);

	private static async Task AssertUsesSynchronizationContext(int arity, bool? configureAwait, bool shouldUseSynchronizationContext)
	{
		var source = new TaskCompletionSource<object>();

		var adapters = AwaiterAdapter.CreateAdapters(Enumerable.Repeat(source.Task, arity).ToArray(), configureAwait);

		var copyableContext = new CopyableSynchronizationContext();
		using (TempSyncContext(copyableContext))
		{
			var resultSourcesByAdapterIndex =
				adapters.Select(_ => new TaskCompletionSource<SynchronizationContext>()).ToArray();

			for (var i = 0; i < adapters.Count; i++)
			{
				var adapterIndex = i;
				adapters[i].OnCompleted(() =>
					resultSourcesByAdapterIndex[adapterIndex].SetResult(SynchronizationContext.Current));
			}

			source.SetResult(null);

			var resultsByAdapterIndex = await Task.WhenAll(resultSourcesByAdapterIndex.Select(s => s.Task));

			var expected = shouldUseSynchronizationContext ? copyableContext : null;

			Assert.All(adapters.Zip(resultsByAdapterIndex, (adapter, result) => (Adapter: adapter, Result: result)), r => Assert.Same(expected, r.Result));
		}
	}

	private static void AssertAllAdapters(IReadOnlyCollection<AwaiterAdapter> adapters, Func<AwaiterAdapter, bool> predicate)
	{
#if NETFRAMEWORK
		if (adapters == null) throw new ArgumentNullException(nameof(adapters));
		if (predicate == null) throw new ArgumentNullException(nameof(predicate));
#else
		ArgumentNullException.ThrowIfNull(adapters, nameof(adapters));
		ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
#endif

		Assert.Empty(adapters.Where(adapter => !predicate.Invoke(adapter)));
	}

	private static Task<object> TaskFromException(Exception exception)
	{
		var source = new TaskCompletionSource<object>();
		source.SetException(exception);
		return source.Task;
	}

	private static IDisposable TempSyncContext(SynchronizationContext synchronizationContext)
	{
		var previous = SynchronizationContext.Current;
		SynchronizationContext.SetSynchronizationContext(synchronizationContext);

		return On.Dispose(() => SynchronizationContext.SetSynchronizationContext(previous));
	}
}
