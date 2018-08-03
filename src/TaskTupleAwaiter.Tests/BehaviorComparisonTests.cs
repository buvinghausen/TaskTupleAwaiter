using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace TaskTupleAwaiter.Tests
{
	public static class BehaviorComparisonTests
	{
		private static Task<object> CompletedTask { get; } = Task.FromResult<object>(null);

		public static IEnumerable<object[]> EachIndexForEachArity() =>
			from arity in Enumerable.Range(1, 10)
			from whichToWaitFor in Enumerable.Range(0, arity - 1)
			select new object[] { arity, whichToWaitFor };

		[Theory, MemberData(nameof(EachIndexForEachArity))]
		public static async Task WaitsForAllTasksToComplete(int arity, int whichToWaitFor)
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

		private static void AssertAllAdapters(IReadOnlyCollection<AwaiterAdapter> adapters, Func<AwaiterAdapter, bool> predicate)
		{
			if (adapters == null) throw new ArgumentNullException(nameof(adapters));
			if (predicate == null) throw new ArgumentNullException(nameof(predicate));

			Assert.Empty(adapters.Where(adapter => !predicate.Invoke(adapter)));
		}
	}
}
