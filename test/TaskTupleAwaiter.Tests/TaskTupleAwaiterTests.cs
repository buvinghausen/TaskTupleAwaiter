using Xunit;

namespace TaskTupleAwaiter.Tests;

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
		var (a, b, c, d) = await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync());
		Assert.IsType<string>(a);
		Assert.IsType<Guid>(b);
		Assert.IsType<string>(c);
		Assert.IsType<Guid>(d);
	}

	[Fact]
	public async Task CanAwaitFiveTasksWithNewSyntax()
	{
		var (a, b, c, d, e) = await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync());
		Assert.IsType<string>(a);
		Assert.IsType<Guid>(b);
		Assert.IsType<string>(c);
		Assert.IsType<Guid>(d);
		Assert.IsType<string>(e);
	}

	[Fact]
	public async Task CanAwaitSixTasksWithNewSyntax()
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
	public async Task CanAwaitSevenTasksWithNewSyntax()
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
	public async Task CanAwaitEightTasksWithNewSyntax()
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
	public async Task CanAwaitNineTasksWithNewSyntax()
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
	public async Task CanAwaitTenTasksWithNewSyntax()
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
		var slow = new TaskCompletionSource<int>();

		var awaitable = (task1, task2, slow.Task).ConfigureAwait(continueOnCapturedContext);

		Assert.False(awaitable.GetAwaiter().IsCompleted);
		slow.SetResult(42);
		await awaitable;

		Assert.True(task1.IsCompleted & task2.IsCompleted);
	}

	[Theory]
	[MemberData(nameof(ContinueOnCapturedContextOptions))]
	public async Task AwaitFourTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
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
	public async Task AwaitFiveTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
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
	public async Task AwaitSixTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
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
	public async Task AwaitSevenTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
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
	public async Task AwaitEightTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
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
		var slow = new TaskCompletionSource<int>();

		var awaitable = (task1, task2, task3, task4, task5, task6, task7, task8, slow.Task).ConfigureAwait(continueOnCapturedContext);

		Assert.False(awaitable.GetAwaiter().IsCompleted);
		slow.SetResult(42);
		await awaitable;

		Assert.True(task1.IsCompleted & task2.IsCompleted & task3.IsCompleted & task4.IsCompleted & task5.IsCompleted & task6.IsCompleted & task7.IsCompleted & task8.IsCompleted);
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
		var slow = new TaskCompletionSource<int>();

		var awaitable = (task1, task2, task3, task4, task5, task6, task7, task8, task9, slow.Task).ConfigureAwait(continueOnCapturedContext);

		Assert.False(awaitable.GetAwaiter().IsCompleted);
		slow.SetResult(42);
		await awaitable;

		Assert.True(task1.IsCompleted & task2.IsCompleted & task3.IsCompleted & task4.IsCompleted & task5.IsCompleted & task6.IsCompleted & task7.IsCompleted & task8.IsCompleted & task9.IsCompleted);
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
