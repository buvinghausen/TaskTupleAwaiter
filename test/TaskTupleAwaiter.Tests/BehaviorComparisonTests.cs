using TaskTupleAwaiter.Tests.Adapters;

namespace TaskTupleAwaiter.Tests;

public static class BehaviorComparisonTests
{
	static BehaviorComparisonTests()
	{
		TaskCompletionSource<object> canceled = new();
		canceled.SetCanceled();
		CanceledTask = canceled.Task;
	}

	private static Task<object> CompletedTask { get; } = Task.FromResult<object>(null);
	private static Task<object> CanceledTask { get; }
	private static Task<object> FailedTask { get; } = TaskFromException(new DummyException());


	public static TheoryData<int> EachArity()
	{
		TheoryData<int> data = [];
		foreach (var arity in Enumerable.Range(1, 16))
		{
			data.Add(arity);
		}

		return data;
	}

	public static TheoryData<int, int> EachIndexForEachArity()
	{
		TheoryData<int, int> data = [];
		foreach (var arity in Enumerable.Range(1, 16))
		{
			foreach (var whichToWaitFor in Enumerable.Range(0, arity - 1))
			{
				data.Add(arity, whichToWaitFor);
			}
		}

		return data;
	}

	[Theory]
	[MemberData(nameof(EachIndexForEachArity))]
	private static async Task WaitsForAllTasksToCompleteWhenAllSucceed(int arity, int whichToWaitFor)
	{
		TaskCompletionSource<object> source = new();

		Task<object>[] tasks = [..Enumerable.Range(0, arity)
			.Select(index => index == whichToWaitFor ? source.Task : CompletedTask)
		];

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
		TaskCompletionSource<object> source = new();

		Task<object>[] tasks = [.. Enumerable.Range(0, arity)
			.Select(index =>
				index == whichToWaitFor ? source.Task : CanceledTask)];

		var adapters = AwaiterAdapter.CreateAllAdapters(tasks);

		AssertAllAdapters(adapters, adapter => !adapter.IsCompleted);

		source.SetCanceled(
#if !NETFRAMEWORK
			TestContext.Current.CancellationToken
#endif
		);

		foreach (var adapter in adapters)
			await Should.ThrowAsync<TaskCanceledException>(async () => await adapter);

		AssertAllAdapters(adapters, adapter => adapter.IsCompleted);
	}

	[Theory]
	[MemberData(nameof(EachIndexForEachArity))]
	private static async Task WaitsForAllTasksToCompleteWhenAllFail(int arity, int whichToWaitFor)
	{
		TaskCompletionSource<object> source = new();

		Task<object>[] tasks = [.. Enumerable.Range(0, arity)
			.Select(index =>
				index == whichToWaitFor ? source.Task : FailedTask)];

		var adapters = AwaiterAdapter.CreateAllAdapters(tasks);

		AssertAllAdapters(adapters, adapter => !adapter.IsCompleted);

		source.SetException(new DummyException());

		foreach (var adapter in adapters)
			await Should.ThrowAsync<DummyException>(async () => await adapter);

		AssertAllAdapters(adapters, adapter => adapter.IsCompleted);
	}

	[Theory]
	[MemberData(nameof(EachArity))]
	private static void CompletesSynchronouslyIfAllTasksWereCompletedSynchronously(int arity)
	{
		Task<object>[] tasks = [.. Enumerable.Repeat(CompletedTask, arity)];

		var adapters = AwaiterAdapter.CreateAllAdapters(tasks);

		AssertAllAdapters(adapters, adapter => adapter.IsCompleted);
	}

	[Theory]
	[MemberData(nameof(EachArity))]
	private static void ResultsAreInCorrectOrder(int arity)
	{
		Task<object>[] tasks = [.. Enumerable.Range(0, arity)
			.Select(index => Task.FromResult<object>(index))];

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
		TaskCompletionSource<object>[] sources =
			[.. Enumerable.Range(0, arity).Select(_ => new TaskCompletionSource<object>())];

		var adapters = AwaiterAdapter.CreateAllAdapters([.. sources.Select(source => source.Task)]);

		for (var i = sources.Length - 1; i >= 0; i--) sources[i].SetException(new DummyException());

		AssertAllAdapters(adapters,
			adapter => ReferenceEquals(sources[0].Task.Exception?.InnerException,
				Assert.ThrowsAny<DummyException>(adapter.GetResult)));
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

	private static async Task AssertUsesSynchronizationContext(int arity, bool? configureAwait,
		bool shouldUseSynchronizationContext)
	{
		TaskCompletionSource<object> source = new();

		var adapters = AwaiterAdapter.CreateAdapters([.. Enumerable.Repeat(source.Task, arity)], configureAwait);

		var copyableContext = new CopyableSynchronizationContext();
		using (TempSyncContext(copyableContext))
		{
			TaskCompletionSource<SynchronizationContext>[] resultSourcesByAdapterIndex =
				[.. adapters.Select(_ => new TaskCompletionSource<SynchronizationContext>())];

			for (var i = 0; i < adapters.Count; i++)
			{
				var adapterIndex = i;
				adapters[i].OnCompleted(() =>
					resultSourcesByAdapterIndex[adapterIndex].SetResult(SynchronizationContext.Current));
			}

			source.SetResult(null);

			var resultsByAdapterIndex = await Task.WhenAll(resultSourcesByAdapterIndex.Select(s => s.Task));

			var expected = shouldUseSynchronizationContext ? copyableContext : null;

			Assert.All(adapters.Zip(resultsByAdapterIndex, (adapter, result) => (Adapter: adapter, Result: result)),
				r => Assert.Same(expected, r.Result));
		}
	}

	private static void AssertAllAdapters(IReadOnlyCollection<AwaiterAdapter> adapters,
		Func<AwaiterAdapter, bool> predicate)
	{
#if NETFRAMEWORK
		if (adapters == null) throw new ArgumentNullException(nameof(adapters));
		if (predicate == null) throw new ArgumentNullException(nameof(predicate));
#else
		ArgumentNullException.ThrowIfNull(adapters);
		ArgumentNullException.ThrowIfNull(predicate);
#endif

		adapters.Where(adapter => !predicate.Invoke(adapter)).ShouldBeEmpty();
	}

	private static Task<object> TaskFromException(Exception exception)
	{
		TaskCompletionSource<object> source = new();
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
