using Xunit;

namespace TaskTupleAwaiter.Tests;

public class TaskTupleAwaiterTests
{
	[Fact]
	private async Task CanAwaitTwoTasksWithNewSyntax()
	{
		var (a, b) = await (GetStringAsync(), GetGuidAsync());
		Assert.IsType<string>(a);
		Assert.IsType<Guid>(b);
	}

	[Fact]
	private async Task CanAwaitThreeTasksWithNewSyntax()
	{
		var (a, b, c) = await (GetStringAsync(), GetGuidAsync(), GetStringAsync());
		Assert.IsType<string>(a);
		Assert.IsType<Guid>(b);
		Assert.IsType<string>(c);
	}

	[Fact]
	private async Task CanAwaitFourTasksWithNewSyntax()
	{
		var (a, b, c, d) = await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync());
		Assert.IsType<string>(a);
		Assert.IsType<Guid>(b);
		Assert.IsType<string>(c);
		Assert.IsType<Guid>(d);
	}

	[Fact]
	private async Task CanAwaitFiveTasksWithNewSyntax()
	{
		var (a, b, c, d, e) = await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync());
		Assert.IsType<string>(a);
		Assert.IsType<Guid>(b);
		Assert.IsType<string>(c);
		Assert.IsType<Guid>(d);
		Assert.IsType<string>(e);
	}

	[Fact]
	private async Task CanAwaitSixTasksWithNewSyntax()
	{
		var (a, b, c, d, e, f) = await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync());
		Assert.IsType<string>(a);
		Assert.IsType<Guid>(b);
		Assert.IsType<string>(c);
		Assert.IsType<Guid>(d);
		Assert.IsType<string>(e);
		Assert.IsType<Guid>(f);
	}

	[Fact]
	private async Task CanAwaitSevenTasksWithNewSyntax()
	{
		var (a, b, c, d, e, f, g) = await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync());
		Assert.IsType<string>(a);
		Assert.IsType<Guid>(b);
		Assert.IsType<string>(c);
		Assert.IsType<Guid>(d);
		Assert.IsType<string>(e);
		Assert.IsType<Guid>(f);
		Assert.IsType<string>(g);
	}

	[Fact]
	private async Task CanAwaitEightTasksWithNewSyntax()
	{
		var (a, b, c, d, e, f, g, h) = await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync());
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
	private async Task CanAwaitNineTasksWithNewSyntax()
	{
		var (a, b, c, d, e, f, g, h, i) = await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync());
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
	private async Task CanAwaitTenTasksWithNewSyntax()
	{
		var (a, b, c, d, e, f, g, h, i, j) = await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync());
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

	[Fact]
	private async Task CanAwaitElevenTasksWithNewSyntax()
	{
		var (a, b, c, d, e, f, g, h, i, j, k) = await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync());
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
		Assert.IsType<string>(k);
	}

	[Fact]
	private async Task CanAwaitTwelveTasksWithNewSyntax()
	{
		var (a, b, c, d, e, f, g, h, i, j, k, l) = await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync());
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
		Assert.IsType<string>(k);
		Assert.IsType<Guid>(l);
	}

	[Fact]
	private async Task CanAwaitThirteenTasksWithNewSyntax()
	{
		var (a, b, c, d, e, f, g, h, i, j, k, l, m) = await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync());
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
		Assert.IsType<string>(k);
		Assert.IsType<Guid>(l);
		Assert.IsType<string>(m);
	}

	[Fact]
	private async Task CanAwaitFourteenTasksWithNewSyntax()
	{
		var (a, b, c, d, e, f, g, h, i, j, k, l, m, n) = await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync());
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
		Assert.IsType<string>(k);
		Assert.IsType<Guid>(l);
		Assert.IsType<string>(m);
		Assert.IsType<Guid>(n);
	}

	[Fact]
	private async Task CanAwaitFifteenTasksWithNewSyntax()
	{
		var (a, b, c, d, e, f, g, h, i, j, k, l, m, n, o) = await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync());
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
		Assert.IsType<string>(k);
		Assert.IsType<Guid>(l);
		Assert.IsType<string>(m);
		Assert.IsType<Guid>(n);
		Assert.IsType<string>(o);
	}

	[Fact]
	private async Task CanAwaitSixteenTasksWithNewSyntax()
	{
		var (a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p) = await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync());
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
		Assert.IsType<string>(k);
		Assert.IsType<Guid>(l);
		Assert.IsType<string>(m);
		Assert.IsType<Guid>(n);
		Assert.IsType<string>(o);
		Assert.IsType<Guid>(p);
	}

	public static IEnumerable<object[]> ContinueOnCapturedContextOptions()
	{
		yield return new object[] { true };
		yield return new object[] { false };
	}

	[Theory]
	[MemberData(nameof(ContinueOnCapturedContextOptions))]
	private async Task AwaitThreeTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
	{
		var task1 = GetStringAsync();
		var task2 = GetStringAsync();
		var slow = new TaskCompletionSource<int>();

		var awaitable = (task1, task2, slow.Task).ConfigureAwait(continueOnCapturedContext);

		Assert.False(awaitable.GetAwaiter().IsCompleted);
		slow.SetResult(42);
		await awaitable;

		Assert.True(task1.IsCompleted & task2.IsCompleted);
	}

	[Theory]
	[MemberData(nameof(ContinueOnCapturedContextOptions))]
	private async Task AwaitFourTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
	{
		var task1 = GetStringAsync();
		var task2 = GetStringAsync();
		var task3 = GetStringAsync();
		var slow = new TaskCompletionSource<int>();

		var awaitable = (task1, task2, task3, slow.Task).ConfigureAwait(continueOnCapturedContext);

		Assert.False(awaitable.GetAwaiter().IsCompleted);
		slow.SetResult(42);
		await awaitable;

		Assert.True(task1.IsCompleted & task2.IsCompleted & task3.IsCompleted);
	}

	[Theory]
	[MemberData(nameof(ContinueOnCapturedContextOptions))]
	private async Task AwaitFiveTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
	{
		var task1 = GetStringAsync();
		var task2 = GetStringAsync();
		var task3 = GetStringAsync();
		var task4 = GetStringAsync();
		var slow = new TaskCompletionSource<int>();

		var awaitable = (task1, task2, task3, task4, slow.Task).ConfigureAwait(continueOnCapturedContext);

		Assert.False(awaitable.GetAwaiter().IsCompleted);
		slow.SetResult(42);
		await awaitable;

		Assert.True(task1.IsCompleted & task2.IsCompleted & task3.IsCompleted & task4.IsCompleted);
	}

	[Theory]
	[MemberData(nameof(ContinueOnCapturedContextOptions))]
	private async Task AwaitSixTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
	{
		var task1 = GetStringAsync();
		var task2 = GetStringAsync();
		var task3 = GetStringAsync();
		var task4 = GetStringAsync();
		var task5 = GetStringAsync();
		var slow = new TaskCompletionSource<int>();

		var awaitable = (task1, task2, task3, task4, task5, slow.Task).ConfigureAwait(continueOnCapturedContext);

		Assert.False(awaitable.GetAwaiter().IsCompleted);
		slow.SetResult(42);
		await awaitable;

		Assert.True(task1.IsCompleted & task2.IsCompleted & task3.IsCompleted & task4.IsCompleted &	task5.IsCompleted);
	}

	[Theory]
	[MemberData(nameof(ContinueOnCapturedContextOptions))]
	private async Task AwaitSevenTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
	{
		var task1 = GetStringAsync();
		var task2 = GetStringAsync();
		var task3 = GetStringAsync();
		var task4 = GetStringAsync();
		var task5 = GetStringAsync();
		var task6 = GetStringAsync();
		var slow = new TaskCompletionSource<int>();

		var awaitable = (task1, task2, task3, task4, task5, task6, slow.Task).ConfigureAwait(continueOnCapturedContext);

		Assert.False(awaitable.GetAwaiter().IsCompleted);
		slow.SetResult(42);
		await awaitable;

		Assert.True(task1.IsCompleted & task2.IsCompleted & task3.IsCompleted & task4.IsCompleted & task5.IsCompleted & task6.IsCompleted);
	}

	[Theory]
	[MemberData(nameof(ContinueOnCapturedContextOptions))]
	private async Task AwaitEightTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
	{
		var task1 = GetStringAsync();
		var task2 = GetStringAsync();
		var task3 = GetStringAsync();
		var task4 = GetStringAsync();
		var task5 = GetStringAsync();
		var task6 = GetStringAsync();
		var task7 = GetStringAsync();
		var slow = new TaskCompletionSource<int>();

		var awaitable = (task1, task2, task3, task4, task5, task6, task7, slow.Task).ConfigureAwait(continueOnCapturedContext);

		Assert.False(awaitable.GetAwaiter().IsCompleted);
		slow.SetResult(42);
		await awaitable;
		Assert.True(task1.IsCompleted & task2.IsCompleted & task3.IsCompleted & task4.IsCompleted & task5.IsCompleted & task6.IsCompleted & task7.IsCompleted);
	}

	[Theory]
	[MemberData(nameof(ContinueOnCapturedContextOptions))]
	private async Task AwaitNineTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
	{
		var task1 = GetStringAsync();
		var task2 = GetStringAsync();
		var task3 = GetStringAsync();
		var task4 = GetStringAsync();
		var task5 = GetStringAsync();
		var task6 = GetStringAsync();
		var task7 = GetStringAsync();
		var task8 = GetStringAsync();
		var slow = new TaskCompletionSource<int>();

		var awaitable = (task1, task2, task3, task4, task5, task6, task7, task8, slow.Task).ConfigureAwait(continueOnCapturedContext);

		Assert.False(awaitable.GetAwaiter().IsCompleted);
		slow.SetResult(42);
		await awaitable;

		Assert.True(task1.IsCompleted & task2.IsCompleted & task3.IsCompleted & task4.IsCompleted & task5.IsCompleted & task6.IsCompleted & task7.IsCompleted & task8.IsCompleted);
	}

	[Theory]
	[MemberData(nameof(ContinueOnCapturedContextOptions))]
	private async Task AwaitTenTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
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
		var slow = new TaskCompletionSource<int>();

		var awaitable = (task1, task2, task3, task4, task5, task6, task7, task8, task9, slow.Task).ConfigureAwait(continueOnCapturedContext);

		Assert.False(awaitable.GetAwaiter().IsCompleted);
		slow.SetResult(42);
		await awaitable;

		Assert.True(task1.IsCompleted & task2.IsCompleted & task3.IsCompleted & task4.IsCompleted & task5.IsCompleted & task6.IsCompleted & task7.IsCompleted & task8.IsCompleted & task9.IsCompleted);
	}

	[Theory]
	[MemberData(nameof(ContinueOnCapturedContextOptions))]
	private async Task AwaitElevenTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
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
		var task10 = GetStringAsync();
		var slow = new TaskCompletionSource<int>();

		var awaitable = (task1, task2, task3, task4, task5, task6, task7, task8, task9, task10, slow.Task).ConfigureAwait(continueOnCapturedContext);

		Assert.False(awaitable.GetAwaiter().IsCompleted);
		slow.SetResult(42);
		await awaitable;

		Assert.True(task1.IsCompleted & task2.IsCompleted & task3.IsCompleted & task4.IsCompleted & task5.IsCompleted & task6.IsCompleted & task7.IsCompleted & task8.IsCompleted & task9.IsCompleted & task10.IsCompleted);
	}

	[Theory]
	[MemberData(nameof(ContinueOnCapturedContextOptions))]
	private async Task AwaitTwelveTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
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
		var task10 = GetStringAsync();
		var task11 = GetStringAsync();
		var slow = new TaskCompletionSource<int>();

		var awaitable = (task1, task2, task3, task4, task5, task6, task7, task8, task9, task10, task11, slow.Task).ConfigureAwait(continueOnCapturedContext);

		Assert.False(awaitable.GetAwaiter().IsCompleted);
		slow.SetResult(42);
		await awaitable;

		Assert.True(task1.IsCompleted & task2.IsCompleted & task3.IsCompleted & task4.IsCompleted & task5.IsCompleted & task6.IsCompleted & task7.IsCompleted & task8.IsCompleted & task9.IsCompleted & task10.IsCompleted & task11.IsCompleted);
	}

	[Theory]
	[MemberData(nameof(ContinueOnCapturedContextOptions))]
	private async Task AwaitThirteenTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
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
		var task10 = GetStringAsync();
		var task11 = GetStringAsync();
		var task12 = GetStringAsync();
		var slow = new TaskCompletionSource<int>();

		var awaitable = (task1, task2, task3, task4, task5, task6, task7, task8, task9, task10, task11, task12, slow.Task).ConfigureAwait(continueOnCapturedContext);

		Assert.False(awaitable.GetAwaiter().IsCompleted);
		slow.SetResult(42);
		await awaitable;

		Assert.True(task1.IsCompleted & task2.IsCompleted & task3.IsCompleted & task4.IsCompleted & task5.IsCompleted & task6.IsCompleted & task7.IsCompleted & task8.IsCompleted & task9.IsCompleted & task10.IsCompleted & task11.IsCompleted & task12.IsCompleted);
	}

	[Theory]
	[MemberData(nameof(ContinueOnCapturedContextOptions))]
	private async Task AwaitFourteenTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
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
		var task10 = GetStringAsync();
		var task11 = GetStringAsync();
		var task12 = GetStringAsync();
		var task13 = GetStringAsync();
		var slow = new TaskCompletionSource<int>();

		var awaitable = (task1, task2, task3, task4, task5, task6, task7, task8, task9, task10, task11, task12, task13, slow.Task).ConfigureAwait(continueOnCapturedContext);

		Assert.False(awaitable.GetAwaiter().IsCompleted);
		slow.SetResult(42);
		await awaitable;

		Assert.True(task1.IsCompleted & task2.IsCompleted & task3.IsCompleted & task4.IsCompleted & task5.IsCompleted & task6.IsCompleted & task7.IsCompleted & task8.IsCompleted & task9.IsCompleted & task10.IsCompleted & task11.IsCompleted & task12.IsCompleted & task13.IsCompleted);
	}

	[Theory]
	[MemberData(nameof(ContinueOnCapturedContextOptions))]
	private async Task AwaitFifteenTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
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
		var task10 = GetStringAsync();
		var task11 = GetStringAsync();
		var task12 = GetStringAsync();
		var task13 = GetStringAsync();
		var task14 = GetStringAsync();
		var slow = new TaskCompletionSource<int>();

		var awaitable = (task1, task2, task3, task4, task5, task6, task7, task8, task9, task10, task11, task12, task13, task14, slow.Task).ConfigureAwait(continueOnCapturedContext);

		Assert.False(awaitable.GetAwaiter().IsCompleted);
		slow.SetResult(42);
		await awaitable;

		Assert.True(task1.IsCompleted & task2.IsCompleted & task3.IsCompleted & task4.IsCompleted & task5.IsCompleted & task6.IsCompleted & task7.IsCompleted & task8.IsCompleted & task9.IsCompleted & task10.IsCompleted & task11.IsCompleted & task12.IsCompleted & task13.IsCompleted & task14.IsCompleted);
	}

	[Theory]
	[MemberData(nameof(ContinueOnCapturedContextOptions))]
	private async Task AwaitSixteenTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
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
		var task10 = GetStringAsync();
		var task11 = GetStringAsync();
		var task12 = GetStringAsync();
		var task13 = GetStringAsync();
		var task14 = GetStringAsync();
		var task15 = GetStringAsync();
		var slow = new TaskCompletionSource<int>();

		var awaitable = (task1, task2, task3, task4, task5, task6, task7, task8, task9, task10, task11, task12, task13, task14, task15, slow.Task).ConfigureAwait(continueOnCapturedContext);

		Assert.False(awaitable.GetAwaiter().IsCompleted);
		slow.SetResult(42);
		await awaitable;

		Assert.True(task1.IsCompleted & task2.IsCompleted & task3.IsCompleted & task4.IsCompleted & task5.IsCompleted & task6.IsCompleted & task7.IsCompleted & task8.IsCompleted & task9.IsCompleted & task10.IsCompleted & task11.IsCompleted & task12.IsCompleted & task13.IsCompleted & task14.IsCompleted & task15.IsCompleted);
	}

	private static async Task<string> GetStringAsync()
	{
		await Task.Yield();
		return Guid.NewGuid().ToString();
	}

	private static async Task<Guid> GetGuidAsync()
	{
		await Task.Yield();
		return Guid.NewGuid();
	}
}
