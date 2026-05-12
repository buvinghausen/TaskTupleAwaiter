using System.Threading.Tasks;

// --- typed arity-1: ValueTuple<Task<T1>> ---
// The arity-1 GetAwaiter delegates directly to the inner task's TaskAwaiter<T1>.
{
	var tuple = ValueTuple.Create(Task.FromResult(1));
	var a = await tuple;
	Console.WriteLine($"arity-1 typed: {a}");
}

// --- typed arity-2 with ConfigureAwait(bool) ---
{
	var (b1, b2) = await (Task.FromResult(2), Task.FromResult("two")).ConfigureAwait(false);
	Console.WriteLine($"arity-2 typed CA(false): {b1}, {b2}");
}

// --- typed arity-2 with ConfigureAwaitOptions ---
{
	var (c1, c2) = await (Task.FromResult(3), Task.FromResult("three"))
		.ConfigureAwait(ConfigureAwaitOptions.None);
	Console.WriteLine($"arity-2 typed CA(options): {c1}, {c2}");
}

// --- typed arity-16 (upper boundary of the generator's struct emission loop) ---
{
	var (d1, d2, d3, d4, d5, d6, d7, d8, d9, d10, d11, d12, d13, d14, d15, d16) = await (
		Task.FromResult(1), Task.FromResult(2), Task.FromResult(3), Task.FromResult(4),
		Task.FromResult(5), Task.FromResult(6), Task.FromResult(7), Task.FromResult(8),
		Task.FromResult(9), Task.FromResult(10), Task.FromResult(11), Task.FromResult(12),
		Task.FromResult(13), Task.FromResult(14), Task.FromResult(15), Task.FromResult(16));
	Console.WriteLine($"arity-16 typed: {d1}..{d16} (sum check: {d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8 + d9 + d10 + d11 + d12 + d13 + d14 + d15 + d16})");
}

// --- non-generic Task tuple arity-2 ---
{
	await (Task.CompletedTask, Task.CompletedTask);
	Console.WriteLine("non-generic arity-2: ok");
}

// --- non-generic Task tuple arity-2 with ConfigureAwaitOptions ---
{
	await (Task.CompletedTask, Task.CompletedTask).ConfigureAwait(ConfigureAwaitOptions.None);
	Console.WriteLine("non-generic arity-2 CA(options): ok");
}

Console.WriteLine("AOT smoke-test completed.");
