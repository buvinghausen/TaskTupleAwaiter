using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace TaskTupleAwaiter.Tests
{
	public static class BehaviorComparisonTests
	{
		private static Task<object> CompletedTask { get; } = Task.FromResult<object>(null);
		private static Task<object> CanceledTask { get; }
		private static Task<object> FailedTask { get; } = TaskFromException(new DummyException());

		static BehaviorComparisonTests()
		{
			var canceled = new TaskCompletionSource<object>();
			canceled.SetCanceled();
			CanceledTask = canceled.Task;
		}

		public static IEnumerable<object[]> EachArity() =>
			from arity in Enumerable.Range(1, 10)
			select new object[] { arity };

		public static IEnumerable<object[]> EachIndexForEachArity() =>
			from arity in Enumerable.Range(1, 10)
			from whichToWaitFor in Enumerable.Range(0, arity - 1)
			select new object[] { arity, whichToWaitFor };

		[Theory, MemberData(nameof(EachIndexForEachArity))]
		public static async Task WaitsForAllTasksToCompleteWhenAllSucceed(int arity, int whichToWaitFor)
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

		[Theory, MemberData(nameof(EachIndexForEachArity))]
		public static async Task WaitsForAllTasksToCompleteWhenAllCancel(int arity, int whichToWaitFor)
		{
			var source = new TaskCompletionSource<object>();

			var tasks = (
				from index in Enumerable.Range(0, arity)
				select index == whichToWaitFor ? source.Task : CanceledTask).ToArray();

			var adapters = AwaiterAdapter.CreateAllAdapters(tasks);

			AssertAllAdapters(adapters, adapter => !adapter.IsCompleted);

			source.SetCanceled();

			foreach (var adapter in adapters)
				await Assert.ThrowsAnyAsync<TaskCanceledException>(async () => await adapter);

			AssertAllAdapters(adapters, adapter => adapter.IsCompleted);
		}

		[Theory, MemberData(nameof(EachIndexForEachArity))]
		public static async Task WaitsForAllTasksToCompleteWhenAllFail(int arity, int whichToWaitFor)
		{
			var source = new TaskCompletionSource<object>();

			var tasks = (
				from index in Enumerable.Range(0, arity)
				select index == whichToWaitFor ? source.Task : FailedTask).ToArray();

			var adapters = AwaiterAdapter.CreateAllAdapters(tasks);

			AssertAllAdapters(adapters, adapter => !adapter.IsCompleted);

			source.SetException(new DummyException());

			foreach (var adapter in adapters)
				await Assert.ThrowsAnyAsync<DummyException>(async () => await adapter);

			AssertAllAdapters(adapters, adapter => adapter.IsCompleted);
		}

		[Theory, MemberData(nameof(EachArity))]
		public static void CompletesSynchronouslyIfAllTasksWereCompletedSynchronously(int arity)
		{
			var tasks = Enumerable.Repeat(CompletedTask, arity).ToArray();

			var adapters = AwaiterAdapter.CreateAllAdapters(tasks);

			AssertAllAdapters(adapters, adapter => adapter.IsCompleted);
		}

		[Theory, MemberData(nameof(EachArity))]
		public static void ResultsAreInCorrectOrder(int arity)
		{
			var tasks = (
				from index in Enumerable.Range(0, arity)
				select Task.FromResult<object>(index)).ToArray();

			var adapters = AwaiterAdapter.CreateNonVoidResultAdapters(tasks);

			AssertAllAdapters(adapters, adapter =>
			{
				var result = adapter.GetResult();

				for (var i = 0; i < arity; i++)
				{
					if (!i.Equals(result[i])) return false;
				}

				return true;
			});
		}

		[Theory, MemberData(nameof(EachArity))]
		public static void FirstExceptionIsThrown(int arity)
		{
			var sources = (
				from index in Enumerable.Range(0, arity)
				select new TaskCompletionSource<object>()).ToArray();

			var adapters = AwaiterAdapter.CreateAllAdapters(sources.Select(source => source.Task).ToArray());

			for (var i = sources.Length - 1; i >= 0; i--)
			{
				sources[i].SetException(new DummyException());
			}

			AssertAllAdapters(adapters, adapter =>
				ReferenceEquals(
					sources[0].Task.Exception.InnerException,
					Assert.ThrowsAny<DummyException>(adapter.GetResult)));
		}

		[Theory, MemberData(nameof(EachArity))]
		public static void NonConfiguredAwaitUsesSynchronizationContext(int arity)
		{
			AssertUsesSynchronizationContext(arity, configureAwait: null, shouldUseSynchronizationContext: true);
		}

		[Theory, MemberData(nameof(EachArity))]
		public static void ConfigureAwaitTrueUsesSynchronizationContext(int arity)
		{
			AssertUsesSynchronizationContext(arity, configureAwait: true, shouldUseSynchronizationContext: true);
		}

		[Theory, MemberData(nameof(EachArity))]
		public static void ConfigureAwaitFalseDoesNotUseSynchronizationContext(int arity)
		{
			AssertUsesSynchronizationContext(arity, configureAwait: false, shouldUseSynchronizationContext: false);
		}

		private static void AssertUsesSynchronizationContext(int arity, bool? configureAwait, bool shouldUseSynchronizationContext)
		{
			var source = new TaskCompletionSource<object>();

			var adapters = AwaiterAdapter.CreateAdapters(Enumerable.Repeat(source.Task, arity).ToArray(), continueOnCapturedContext: configureAwait);

			var copyableContext = new CopyableSynchronizationContext();
			using (TempSyncContext(copyableContext))
			{
				var actualContinuationContextByAdapterIndex = new SynchronizationContext[adapters.Count];

				for (var i = 0; i < adapters.Count; i++)
				{
					var adapterIndex = i;
					adapters[i].OnCompleted(() => actualContinuationContextByAdapterIndex[adapterIndex] = SynchronizationContext.Current);
				}

				source.SetResult(null);

				for (var i = 0; i < adapters.Count; i++)
				{
					if (shouldUseSynchronizationContext)
						Assert.Same(expected: copyableContext, actual: actualContinuationContextByAdapterIndex[i]);
					else
						Assert.Null(actualContinuationContextByAdapterIndex[i]);
				}
			}
		}

		private static void AssertAllAdapters(IReadOnlyCollection<AwaiterAdapter> adapters, Func<AwaiterAdapter, bool> predicate)
		{
			if (adapters == null) throw new ArgumentNullException(nameof(adapters));
			if (predicate == null) throw new ArgumentNullException(nameof(predicate));

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
}
