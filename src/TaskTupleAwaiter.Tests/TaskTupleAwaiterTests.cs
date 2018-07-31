using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace TaskTupleAwaiter.Tests
{
	public class TaskTupleAwaiterTests
	{
		[Fact]
		public async Task CanAwaitTwoTasksWithNewSyntax()
		{
			var (a, b) = await (GetStringAsync(), GetGuidAsync());
			Assert.IsType<string>(a);
			Assert.IsType<Guid>(b);
		}

		[Fact]
		public async Task CanAwaitThreeTasksWithNewSyntax()
		{
			var (a, b, c) = await (GetStringAsync(), GetGuidAsync(), GetStringAsync());
			Assert.IsType<string>(a);
			Assert.IsType<Guid>(b);
			Assert.IsType<string>(c);
		}

		[Fact]
		public async Task CanAwaitFourTasksWithNewSyntax()
		{
			var (a, b, c, d) =
				await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync());
			Assert.IsType<string>(a);
			Assert.IsType<Guid>(b);
			Assert.IsType<string>(c);
			Assert.IsType<Guid>(d);
		}

		[Fact]
		public async Task CanAwaitFiveTasksWithNewSyntax()
		{
			var (a, b, c, d, e) =
				await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(),
					GetStringAsync());
			Assert.IsType<string>(a);
			Assert.IsType<Guid>(b);
			Assert.IsType<string>(c);
			Assert.IsType<Guid>(d);
			Assert.IsType<string>(e);
		}

		[Fact]
		public async Task CanAwaitSixTasksWithNewSyntax()
		{
			var (a, b, c, d, e, f) =
				await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(),
					GetStringAsync(), GetGuidAsync());
			Assert.IsType<string>(a);
			Assert.IsType<Guid>(b);
			Assert.IsType<string>(c);
			Assert.IsType<Guid>(d);
			Assert.IsType<string>(e);
			Assert.IsType<Guid>(f);
		}

		[Fact]
		public async Task CanAwaitSevenTasksWithNewSyntax()
		{
			var (a, b, c, d, e, f, g) =
				await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(),
					GetStringAsync(), GetGuidAsync(), GetStringAsync());
			Assert.IsType<string>(a);
			Assert.IsType<Guid>(b);
			Assert.IsType<string>(c);
			Assert.IsType<Guid>(d);
			Assert.IsType<string>(e);
			Assert.IsType<Guid>(f);
			Assert.IsType<string>(g);
		}

		[Fact]
		public async Task CanAwaitEightTasksWithNewSyntax()
		{
			var (a, b, c, d, e, f, g, h) =
				await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(),
					GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync());
			Assert.IsType<string>(a);
			Assert.IsType<Guid>(b);
			Assert.IsType<string>(c);
			Assert.IsType<Guid>(d);
			Assert.IsType<string>(e);
			Assert.IsType<Guid>(f);
			Assert.IsType<string>(g);
			Assert.IsType<Guid>(h);
		}

		[Fact]
		public async Task CanAwaitNineTasksWithNewSyntax()
		{
			var (a, b, c, d, e, f, g, h, i) =
				await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(),
					GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(),
					GetStringAsync());
			Assert.IsType<string>(a);
			Assert.IsType<Guid>(b);
			Assert.IsType<string>(c);
			Assert.IsType<Guid>(d);
			Assert.IsType<string>(e);
			Assert.IsType<Guid>(f);
			Assert.IsType<string>(g);
			Assert.IsType<Guid>(h);
			Assert.IsType<string>(i);
		}

		[Fact]
		public async Task CanAwaitTenTasksWithNewSyntax()
		{
			var (a, b, c, d, e, f, g, h, i, j) =
				await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(),
					GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(),
					GetStringAsync(), GetGuidAsync());
			Assert.IsType<string>(a);
			Assert.IsType<Guid>(b);
			Assert.IsType<string>(c);
			Assert.IsType<Guid>(d);
			Assert.IsType<string>(e);
			Assert.IsType<Guid>(f);
			Assert.IsType<string>(g);
			Assert.IsType<Guid>(h);
			Assert.IsType<string>(i);
			Assert.IsType<Guid>(j);
		}

		public static IEnumerable<object[]> ContinueOnCapturedContextOptions()
		{
			yield return new object[] { true };
			yield return new object[] { false };
		}

		[Theory]
		[MemberData(nameof(ContinueOnCapturedContextOptions))]
		public async Task AwaitThreeTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
		{
			var task1 = GetStringAsync();
			var task2 = GetStringAsync();
			var slowTask = GetIntSlowAsync();
			var waitAllTask = (task1, task2, slowTask).ConfigureAwait(continueOnCapturedContext);

			waitAllTask.GetAwaiter().OnCompleted(() =>
			{
				Assert.True(task1.IsCompleted);
				Assert.True(task2.IsCompleted);
				Assert.True(slowTask.IsCompleted);
			});
			var (a, b, c) = await waitAllTask;
		}

		[Theory]
		[MemberData(nameof(ContinueOnCapturedContextOptions))]
		public async Task AwaitFourTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
		{
			var task1 = GetStringAsync();
			var task2 = GetStringAsync();
			var task3 = GetStringAsync();
			var slowTask = GetIntSlowAsync();
			var waitAllTask = (task1, task2, task3, slowTask).ConfigureAwait(continueOnCapturedContext);

			waitAllTask.GetAwaiter().OnCompleted(() =>
			{
				Assert.True(task1.IsCompleted);
				Assert.True(task2.IsCompleted);
				Assert.True(task3.IsCompleted);
				Assert.True(slowTask.IsCompleted);
			});
			var (a, b, c, d) = await waitAllTask;
		}

		[Theory]
		[MemberData(nameof(ContinueOnCapturedContextOptions))]
		public async Task AwaitFiveTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
		{
			var task1 = GetStringAsync();
			var task2 = GetStringAsync();
			var task3 = GetStringAsync();
			var task4 = GetStringAsync();
			var slowTask = GetIntSlowAsync();
			var waitAllTask = (task1, task2, task3, task4, slowTask).ConfigureAwait(continueOnCapturedContext);

			waitAllTask.GetAwaiter().OnCompleted(() =>
			{
				Assert.True(task1.IsCompleted);
				Assert.True(task2.IsCompleted);
				Assert.True(task3.IsCompleted);
				Assert.True(task4.IsCompleted);
				Assert.True(slowTask.IsCompleted);
			});
			var (a, b, c, d, e) = await waitAllTask;
		}

		[Theory]
		[MemberData(nameof(ContinueOnCapturedContextOptions))]
		public async Task AwaitSixTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
		{
			var task1 = GetStringAsync();
			var task2 = GetStringAsync();
			var task3 = GetStringAsync();
			var task4 = GetStringAsync();
			var task5 = GetStringAsync();
			var slowTask = GetIntSlowAsync();
			var waitAllTask = (task1, task2, task3, task4, task5, slowTask).ConfigureAwait(continueOnCapturedContext);

			waitAllTask.GetAwaiter().OnCompleted(() =>
			{
				Assert.True(task1.IsCompleted);
				Assert.True(task2.IsCompleted);
				Assert.True(task3.IsCompleted);
				Assert.True(task4.IsCompleted);
				Assert.True(task5.IsCompleted);
				Assert.True(slowTask.IsCompleted);
			});
			var (a, b, c, d, e, f) = await waitAllTask;
		}

		[Theory]
		[MemberData(nameof(ContinueOnCapturedContextOptions))]
		public async Task AwaitSevenTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
		{
			var task1 = GetStringAsync();
			var task2 = GetStringAsync();
			var task3 = GetStringAsync();
			var task4 = GetStringAsync();
			var task5 = GetStringAsync();
			var task6 = GetStringAsync();
			var slowTask = GetIntSlowAsync();
			var waitAllTask = (task1, task2, task3, task4, task5, task6, slowTask).ConfigureAwait(continueOnCapturedContext);

			waitAllTask.GetAwaiter().OnCompleted(() =>
			{
				Assert.True(task1.IsCompleted);
				Assert.True(task2.IsCompleted);
				Assert.True(task3.IsCompleted);
				Assert.True(task4.IsCompleted);
				Assert.True(task5.IsCompleted);
				Assert.True(task6.IsCompleted);
				Assert.True(slowTask.IsCompleted);
			});
			var (a, b, c, d, e, f, g) = await waitAllTask;
		}


		[Theory]
		[MemberData(nameof(ContinueOnCapturedContextOptions))]
		public async Task AwaitEightTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
		{
			var task1 = GetStringAsync();
			var task2 = GetStringAsync();
			var task3 = GetStringAsync();
			var task4 = GetStringAsync();
			var task5 = GetStringAsync();
			var task6 = GetStringAsync();
			var task7 = GetStringAsync();
			var slowTask = GetIntSlowAsync();
			var waitAllTask = (task1, task2, task3, task4, task5, task6, task7, slowTask).ConfigureAwait(continueOnCapturedContext);

			waitAllTask.GetAwaiter().OnCompleted(() =>
			{
				Assert.True(task1.IsCompleted);
				Assert.True(task2.IsCompleted);
				Assert.True(task3.IsCompleted);
				Assert.True(task4.IsCompleted);
				Assert.True(task5.IsCompleted);
				Assert.True(task6.IsCompleted);
				Assert.True(task7.IsCompleted);
				Assert.True(slowTask.IsCompleted);
			});
			var (a, b, c, d, e, f, g, h) = await waitAllTask;
		}

		[Theory]
		[MemberData(nameof(ContinueOnCapturedContextOptions))]
		public async Task AwaitNineTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
		{
			var task1 = GetStringAsync();
			var task2 = GetStringAsync();
			var task3 = GetStringAsync();
			var task4 = GetStringAsync();
			var task5 = GetStringAsync();
			var task6 = GetStringAsync();
			var task7 = GetStringAsync();
			var task8 = GetStringAsync();
			var slowTask = GetIntSlowAsync();
			var waitAllTask = (task1, task2, task3, task4, task5, task6, task7, task8, slowTask)
				.ConfigureAwait(continueOnCapturedContext);

			waitAllTask.GetAwaiter().OnCompleted(() =>
			{
				Assert.True(task1.IsCompleted);
				Assert.True(task2.IsCompleted);
				Assert.True(task3.IsCompleted);
				Assert.True(task4.IsCompleted);
				Assert.True(task5.IsCompleted);
				Assert.True(task6.IsCompleted);
				Assert.True(task7.IsCompleted);
				Assert.True(task8.IsCompleted);
				Assert.True(slowTask.IsCompleted);
			});
			var (a, b, c, d, e, f, g, h, i) = await waitAllTask;
		}

		[Theory]
		[MemberData(nameof(ContinueOnCapturedContextOptions))]
		public async Task AwaitTenTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
		{
			var task1 = GetStringAsync();
			var task2 = GetStringAsync();
			var task3 = GetStringAsync();
			var task4 = GetStringAsync();
			var task5 = GetStringAsync();
			var task6 = GetStringAsync();
			var task7 = GetStringAsync();
			var task8 = GetStringAsync();
			var task9 = GetStringAsync();
			var slowTask = GetIntSlowAsync();
			var waitAllTask = (task1, task2, task3, task4, task5, task6, task7, task8, task9, slowTask)
				.ConfigureAwait(continueOnCapturedContext);

			waitAllTask.GetAwaiter().OnCompleted(() =>
			{
				Assert.True(task1.IsCompleted);
				Assert.True(task2.IsCompleted);
				Assert.True(task3.IsCompleted);
				Assert.True(task4.IsCompleted);
				Assert.True(task5.IsCompleted);
				Assert.True(task6.IsCompleted);
				Assert.True(task7.IsCompleted);
				Assert.True(task8.IsCompleted);
				Assert.True(task9.IsCompleted);
				Assert.True(slowTask.IsCompleted);
			});
			var (a, b, c, d, e, f, g, h, i, k) = await waitAllTask;
		}

		private static async Task<string> GetStringAsync()
		{
			await Task.Delay(500);
			return Guid.NewGuid().ToString();
		}

		private static async Task<Guid> GetGuidAsync()
		{
			await Task.Delay(500);
			return Guid.NewGuid();
		}

		private static async Task<int> GetIntSlowAsync()
		{
			await Task.Delay(1000);
			return 42;
		}
	}
}
