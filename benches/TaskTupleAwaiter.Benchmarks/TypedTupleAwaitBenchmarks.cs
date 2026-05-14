using BenchmarkDotNet.Attributes;

namespace TaskTupleAwaiter.Benchmarks;

[MemoryDiagnoser]
public class TypedTupleAwaitBenchmarks
{
	[Benchmark]
	public async Task Arity2_PreCompleted() =>
		_ = await (Task.FromResult(1), Task.FromResult(2));

	[Benchmark]
	public async Task Arity4_PreCompleted() =>
		_ = await (Task.FromResult(1), Task.FromResult(2), Task.FromResult(3), Task.FromResult(4));

	[Benchmark]
	public async Task Arity8_PreCompleted() =>
		_ = await (
			Task.FromResult(1), Task.FromResult(2), Task.FromResult(3), Task.FromResult(4),
			Task.FromResult(5), Task.FromResult(6), Task.FromResult(7), Task.FromResult(8));

	[Benchmark]
	public async Task Arity16_PreCompleted() =>
		_ = await (
			Task.FromResult(1), Task.FromResult(2), Task.FromResult(3), Task.FromResult(4),
			Task.FromResult(5), Task.FromResult(6), Task.FromResult(7), Task.FromResult(8),
			Task.FromResult(9), Task.FromResult(10), Task.FromResult(11), Task.FromResult(12),
			Task.FromResult(13), Task.FromResult(14), Task.FromResult(15), Task.FromResult(16));

	[Benchmark]
	public async Task Arity2_Async() =>
		_ = await (YieldAsync(1), YieldAsync(2));

	[Benchmark]
	public async Task Arity4_Async() =>
		_ = await (YieldAsync(1), YieldAsync(2), YieldAsync(3), YieldAsync(4));

	[Benchmark]
	public async Task Arity8_Async() =>
		_ = await (
			YieldAsync(1), YieldAsync(2), YieldAsync(3), YieldAsync(4),
			YieldAsync(5), YieldAsync(6), YieldAsync(7), YieldAsync(8));

	[Benchmark]
	public async Task Arity16_Async() =>
		_ = await (
			YieldAsync(1), YieldAsync(2), YieldAsync(3), YieldAsync(4),
			YieldAsync(5), YieldAsync(6), YieldAsync(7), YieldAsync(8),
			YieldAsync(9), YieldAsync(10), YieldAsync(11), YieldAsync(12),
			YieldAsync(13), YieldAsync(14), YieldAsync(15), YieldAsync(16));

	static async Task<int> YieldAsync(int value)
	{
		await Task.Yield();
		return value;
	}
}
