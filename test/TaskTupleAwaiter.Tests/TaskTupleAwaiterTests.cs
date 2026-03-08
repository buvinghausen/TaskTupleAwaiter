namespace TaskTupleAwaiter.Tests;

public sealed class TaskTupleAwaiterTests
{
	[Fact]
	async Task CanAwaitTwoTasksWithNewSyntax()
	{
		var (a, b) = await (GetStringAsync(), GetGuidAsync());
		a.ShouldBeOfType<string>();
		b.ShouldBeOfType<Guid>();
	}

	[Fact]
	async Task CanAwaitThreeTasksWithNewSyntax()
	{
		var (a, b, c) = await (GetStringAsync(), GetGuidAsync(), GetStringAsync());
		a.ShouldBeOfType<string>();
		b.ShouldBeOfType<Guid>();
		c.ShouldBeOfType<string>();
	}

	[Fact]
	async Task CanAwaitFourTasksWithNewSyntax()
	{
		var (a, b, c, d) = await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync());
		a.ShouldBeOfType<string>();
		b.ShouldBeOfType<Guid>();
		c.ShouldBeOfType<string>();
		d.ShouldBeOfType<Guid>();
	}

	[Fact]
	async Task CanAwaitFiveTasksWithNewSyntax()
	{
		var (a, b, c, d, e) = await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync());
		a.ShouldBeOfType<string>();
		b.ShouldBeOfType<Guid>();
		c.ShouldBeOfType<string>();
		d.ShouldBeOfType<Guid>();
		e.ShouldBeOfType<string>();
	}

	[Fact]
	async Task CanAwaitSixTasksWithNewSyntax()
	{
		var (a, b, c, d, e, f) = await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync());
		a.ShouldBeOfType<string>();
		b.ShouldBeOfType<Guid>();
		c.ShouldBeOfType<string>();
		d.ShouldBeOfType<Guid>();
		e.ShouldBeOfType<string>();
		f.ShouldBeOfType<Guid>();
	}

	[Fact]
	async Task CanAwaitSevenTasksWithNewSyntax()
	{
		var (a, b, c, d, e, f, g) = await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync());
		a.ShouldBeOfType<string>();
		b.ShouldBeOfType<Guid>();
		c.ShouldBeOfType<string>();
		d.ShouldBeOfType<Guid>();
		e.ShouldBeOfType<string>();
		f.ShouldBeOfType<Guid>();
		g.ShouldBeOfType<string>();
	}

	[Fact]
	async Task CanAwaitEightTasksWithNewSyntax()
	{
		var (a, b, c, d, e, f, g, h) = await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync());
		a.ShouldBeOfType<string>();
		b.ShouldBeOfType<Guid>();
		c.ShouldBeOfType<string>();
		d.ShouldBeOfType<Guid>();
		e.ShouldBeOfType<string>();
		f.ShouldBeOfType<Guid>();
		g.ShouldBeOfType<string>();
		h.ShouldBeOfType<Guid>();
	}

	[Fact]
	async Task CanAwaitNineTasksWithNewSyntax()
	{
		var (a, b, c, d, e, f, g, h, i) = await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync());
		a.ShouldBeOfType<string>();
		b.ShouldBeOfType<Guid>();
		c.ShouldBeOfType<string>();
		d.ShouldBeOfType<Guid>();
		e.ShouldBeOfType<string>();
		f.ShouldBeOfType<Guid>();
		g.ShouldBeOfType<string>();
		h.ShouldBeOfType<Guid>();
		i.ShouldBeOfType<string>();
	}

	[Fact]
	async Task CanAwaitTenTasksWithNewSyntax()
	{
		var (a, b, c, d, e, f, g, h, i, j) = await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync());
		a.ShouldBeOfType<string>();
		b.ShouldBeOfType<Guid>();
		c.ShouldBeOfType<string>();
		d.ShouldBeOfType<Guid>();
		e.ShouldBeOfType<string>();
		f.ShouldBeOfType<Guid>();
		g.ShouldBeOfType<string>();
		h.ShouldBeOfType<Guid>();
		i.ShouldBeOfType<string>();
		j.ShouldBeOfType<Guid>();
	}

	[Fact]
	async Task CanAwaitElevenTasksWithNewSyntax()
	{
		var (a, b, c, d, e, f, g, h, i, j, k) = await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync());
		a.ShouldBeOfType<string>();
		b.ShouldBeOfType<Guid>();
		c.ShouldBeOfType<string>();
		d.ShouldBeOfType<Guid>();
		e.ShouldBeOfType<string>();
		f.ShouldBeOfType<Guid>();
		g.ShouldBeOfType<string>();
		h.ShouldBeOfType<Guid>();
		i.ShouldBeOfType<string>();
		j.ShouldBeOfType<Guid>();
		k.ShouldBeOfType<string>();
	}

	[Fact]
	async Task CanAwaitTwelveTasksWithNewSyntax()
	{
		var (a, b, c, d, e, f, g, h, i, j, k, l) = await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync());
		a.ShouldBeOfType<string>();
		b.ShouldBeOfType<Guid>();
		c.ShouldBeOfType<string>();
		d.ShouldBeOfType<Guid>();
		e.ShouldBeOfType<string>();
		f.ShouldBeOfType<Guid>();
		g.ShouldBeOfType<string>();
		h.ShouldBeOfType<Guid>();
		i.ShouldBeOfType<string>();
		j.ShouldBeOfType<Guid>();
		k.ShouldBeOfType<string>();
		l.ShouldBeOfType<Guid>();
	}

	[Fact]
	async Task CanAwaitThirteenTasksWithNewSyntax()
	{
		var (a, b, c, d, e, f, g, h, i, j, k, l, m) = await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync());
		a.ShouldBeOfType<string>();
		b.ShouldBeOfType<Guid>();
		c.ShouldBeOfType<string>();
		d.ShouldBeOfType<Guid>();
		e.ShouldBeOfType<string>();
		f.ShouldBeOfType<Guid>();
		g.ShouldBeOfType<string>();
		h.ShouldBeOfType<Guid>();
		i.ShouldBeOfType<string>();
		j.ShouldBeOfType<Guid>();
		k.ShouldBeOfType<string>();
		l.ShouldBeOfType<Guid>();
		m.ShouldBeOfType<string>();
	}

	[Fact]
	async Task CanAwaitFourteenTasksWithNewSyntax()
	{
		var (a, b, c, d, e, f, g, h, i, j, k, l, m, n) = await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync());
		a.ShouldBeOfType<string>();
		b.ShouldBeOfType<Guid>();
		c.ShouldBeOfType<string>();
		d.ShouldBeOfType<Guid>();
		e.ShouldBeOfType<string>();
		f.ShouldBeOfType<Guid>();
		g.ShouldBeOfType<string>();
		h.ShouldBeOfType<Guid>();
		i.ShouldBeOfType<string>();
		j.ShouldBeOfType<Guid>();
		k.ShouldBeOfType<string>();
		l.ShouldBeOfType<Guid>();
		m.ShouldBeOfType<string>();
		n.ShouldBeOfType<Guid>();
	}

	[Fact]
	async Task CanAwaitFifteenTasksWithNewSyntax()
	{
		var (a, b, c, d, e, f, g, h, i, j, k, l, m, n, o) = await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync());
		a.ShouldBeOfType<string>();
		b.ShouldBeOfType<Guid>();
		c.ShouldBeOfType<string>();
		d.ShouldBeOfType<Guid>();
		e.ShouldBeOfType<string>();
		f.ShouldBeOfType<Guid>();
		g.ShouldBeOfType<string>();
		h.ShouldBeOfType<Guid>();
		i.ShouldBeOfType<string>();
		j.ShouldBeOfType<Guid>();
		k.ShouldBeOfType<string>();
		l.ShouldBeOfType<Guid>();
		m.ShouldBeOfType<string>();
		n.ShouldBeOfType<Guid>();
		o.ShouldBeOfType<string>();
	}

	[Fact]
	async Task CanAwaitSixteenTasksWithNewSyntax()
	{
		var (a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p) = await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync());
		a.ShouldBeOfType<string>();
		b.ShouldBeOfType<Guid>();
		c.ShouldBeOfType<string>();
		d.ShouldBeOfType<Guid>();
		e.ShouldBeOfType<string>();
		f.ShouldBeOfType<Guid>();
		g.ShouldBeOfType<string>();
		h.ShouldBeOfType<Guid>();
		i.ShouldBeOfType<string>();
		j.ShouldBeOfType<Guid>();
		k.ShouldBeOfType<string>();
		l.ShouldBeOfType<Guid>();
		m.ShouldBeOfType<string>();
		n.ShouldBeOfType<Guid>();
		o.ShouldBeOfType<string>();
		p.ShouldBeOfType<Guid>();
	}

		yield return [true];
	public static TheoryData<bool> ContinueOnCapturedContextOptions() =>
	[
		true,
		false
	];

	[Theory]
	[MemberData(nameof(ContinueOnCapturedContextOptions))]
	async Task AwaitThreeTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
	{
		var task1 = GetStringAsync();
		var task2 = GetStringAsync();
		var slow = new TaskCompletionSource<int>();

		var awaitable = (task1, task2, slow.Task).ConfigureAwait(continueOnCapturedContext);

		awaitable.GetAwaiter().IsCompleted.ShouldBeFalse();
		
		slow.SetResult(42);
		await awaitable;

		(task1.IsCompleted & task2.IsCompleted).ShouldBeTrue();
	}

	[Theory]
	[MemberData(nameof(ContinueOnCapturedContextOptions))]
	async Task AwaitFourTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
	{
		var task1 = GetStringAsync();
		var task2 = GetStringAsync();
		var task3 = GetStringAsync();
		var slow = new TaskCompletionSource<int>();

		var awaitable = (task1, task2, task3, slow.Task).ConfigureAwait(continueOnCapturedContext);

		awaitable.GetAwaiter().IsCompleted.ShouldBeFalse();
		slow.SetResult(42);
		await awaitable;

		(task1.IsCompleted & task2.IsCompleted & task3.IsCompleted).ShouldBeTrue();
	}

	[Theory]
	[MemberData(nameof(ContinueOnCapturedContextOptions))]
	async Task AwaitFiveTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
	{
		var task1 = GetStringAsync();
		var task2 = GetStringAsync();
		var task3 = GetStringAsync();
		var task4 = GetStringAsync();
		var slow = new TaskCompletionSource<int>();

		var awaitable = (task1, task2, task3, task4, slow.Task).ConfigureAwait(continueOnCapturedContext);

		awaitable.GetAwaiter().IsCompleted.ShouldBeFalse();
		slow.SetResult(42);
		await awaitable;

		(task1.IsCompleted & task2.IsCompleted & task3.IsCompleted & task4.IsCompleted).ShouldBeTrue();
	}

	[Theory]
	[MemberData(nameof(ContinueOnCapturedContextOptions))]
	async Task AwaitSixTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
	{
		var task1 = GetStringAsync();
		var task2 = GetStringAsync();
		var task3 = GetStringAsync();
		var task4 = GetStringAsync();
		var task5 = GetStringAsync();
		var slow = new TaskCompletionSource<int>();

		var awaitable = (task1, task2, task3, task4, task5, slow.Task).ConfigureAwait(continueOnCapturedContext);

		awaitable.GetAwaiter().IsCompleted.ShouldBeFalse();
		slow.SetResult(42);
		await awaitable;

		(task1.IsCompleted & task2.IsCompleted & task3.IsCompleted & task4.IsCompleted & task5.IsCompleted).ShouldBeTrue();
	}

	[Theory]
	[MemberData(nameof(ContinueOnCapturedContextOptions))]
	async Task AwaitSevenTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
	{
		var task1 = GetStringAsync();
		var task2 = GetStringAsync();
		var task3 = GetStringAsync();
		var task4 = GetStringAsync();
		var task5 = GetStringAsync();
		var task6 = GetStringAsync();
		var slow = new TaskCompletionSource<int>();

		var awaitable = (task1, task2, task3, task4, task5, task6, slow.Task).ConfigureAwait(continueOnCapturedContext);

		awaitable.GetAwaiter().IsCompleted.ShouldBeFalse();
		slow.SetResult(42);
		await awaitable;

		(task1.IsCompleted & task2.IsCompleted & task3.IsCompleted & task4.IsCompleted & task5.IsCompleted & task6.IsCompleted).ShouldBeTrue();
	}

	[Theory]
	[MemberData(nameof(ContinueOnCapturedContextOptions))]
	async Task AwaitEightTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
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

		awaitable.GetAwaiter().IsCompleted.ShouldBeFalse();
		slow.SetResult(42);
		await awaitable;
		(task1.IsCompleted & task2.IsCompleted & task3.IsCompleted & task4.IsCompleted & task5.IsCompleted & task6.IsCompleted & task7.IsCompleted).ShouldBeTrue();
	}

	[Theory]
	[MemberData(nameof(ContinueOnCapturedContextOptions))]
	async Task AwaitNineTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
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

		awaitable.GetAwaiter().IsCompleted.ShouldBeFalse();
		slow.SetResult(42);
		await awaitable;

		(task1.IsCompleted & task2.IsCompleted & task3.IsCompleted & task4.IsCompleted & task5.IsCompleted & task6.IsCompleted & task7.IsCompleted & task8.IsCompleted).ShouldBeTrue();
	}

	[Theory]
	[MemberData(nameof(ContinueOnCapturedContextOptions))]
	async Task AwaitTenTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
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

		awaitable.GetAwaiter().IsCompleted.ShouldBeFalse();
		slow.SetResult(42);
		await awaitable;

		(task1.IsCompleted & task2.IsCompleted & task3.IsCompleted & task4.IsCompleted & task5.IsCompleted & task6.IsCompleted & task7.IsCompleted & task8.IsCompleted & task9.IsCompleted).ShouldBeTrue();
	}

	[Theory]
	[MemberData(nameof(ContinueOnCapturedContextOptions))]
	async Task AwaitElevenTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
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

		awaitable.GetAwaiter().IsCompleted.ShouldBeFalse();
		slow.SetResult(42);
		await awaitable;

		(task1.IsCompleted & task2.IsCompleted & task3.IsCompleted & task4.IsCompleted & task5.IsCompleted & task6.IsCompleted & task7.IsCompleted & task8.IsCompleted & task9.IsCompleted & task10.IsCompleted).ShouldBeTrue();
	}

	[Theory]
	[MemberData(nameof(ContinueOnCapturedContextOptions))]
	async Task AwaitTwelveTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
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

		awaitable.GetAwaiter().IsCompleted.ShouldBeFalse();
		slow.SetResult(42);
		await awaitable;

		(task1.IsCompleted & task2.IsCompleted & task3.IsCompleted & task4.IsCompleted & task5.IsCompleted & task6.IsCompleted & task7.IsCompleted & task8.IsCompleted & task9.IsCompleted & task10.IsCompleted & task11.IsCompleted).ShouldBeTrue();
	}

	[Theory]
	[MemberData(nameof(ContinueOnCapturedContextOptions))]
	async Task AwaitThirteenTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
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

		awaitable.GetAwaiter().IsCompleted.ShouldBeFalse();
		slow.SetResult(42);
		await awaitable;

		(task1.IsCompleted & task2.IsCompleted & task3.IsCompleted & task4.IsCompleted & task5.IsCompleted & task6.IsCompleted & task7.IsCompleted & task8.IsCompleted & task9.IsCompleted & task10.IsCompleted & task11.IsCompleted & task12.IsCompleted).ShouldBeTrue();
	}

	[Theory]
	[MemberData(nameof(ContinueOnCapturedContextOptions))]
	async Task AwaitFourteenTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
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

		awaitable.GetAwaiter().IsCompleted.ShouldBeFalse();
		slow.SetResult(42);
		await awaitable;

		(task1.IsCompleted & task2.IsCompleted & task3.IsCompleted & task4.IsCompleted & task5.IsCompleted & task6.IsCompleted & task7.IsCompleted & task8.IsCompleted & task9.IsCompleted & task10.IsCompleted & task11.IsCompleted & task12.IsCompleted & task13.IsCompleted).ShouldBeTrue();
	}

	[Theory]
	[MemberData(nameof(ContinueOnCapturedContextOptions))]
	async Task AwaitFifteenTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
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

		awaitable.GetAwaiter().IsCompleted.ShouldBeFalse();
		slow.SetResult(42);
		await awaitable;

		(task1.IsCompleted & task2.IsCompleted & task3.IsCompleted & task4.IsCompleted & task5.IsCompleted & task6.IsCompleted & task7.IsCompleted & task8.IsCompleted & task9.IsCompleted & task10.IsCompleted & task11.IsCompleted & task12.IsCompleted & task13.IsCompleted & task14.IsCompleted).ShouldBeTrue();
	}

	[Theory]
	[MemberData(nameof(ContinueOnCapturedContextOptions))]
	async Task AwaitSixteenTasksWaitsForAllOfThemWhenConfigured(bool continueOnCapturedContext)
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

		awaitable.GetAwaiter().IsCompleted.ShouldBeFalse();
		slow.SetResult(42);
		await awaitable;

		(task1.IsCompleted & task2.IsCompleted & task3.IsCompleted & task4.IsCompleted & task5.IsCompleted & task6.IsCompleted & task7.IsCompleted & task8.IsCompleted & task9.IsCompleted & task10.IsCompleted & task11.IsCompleted & task12.IsCompleted & task13.IsCompleted & task14.IsCompleted & task15.IsCompleted).ShouldBeTrue();
	}

	static async Task<string> GetStringAsync()
	{
		await Task.Yield();
		return Guid.NewGuid().ToString();
	}

	static async Task<Guid> GetGuidAsync()
	{
		await Task.Yield();
		return Guid.NewGuid();
	}
}
