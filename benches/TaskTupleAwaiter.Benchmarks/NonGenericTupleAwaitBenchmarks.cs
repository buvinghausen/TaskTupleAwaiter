using BenchmarkDotNet.Attributes;

namespace TaskTupleAwaiter.Benchmarks;

[MemoryDiagnoser]
public class NonGenericTupleAwaitBenchmarks
{
	[Benchmark]
	public async Task Arity2_PreCompleted() =>
		await ((Task)Task.CompletedTask, Task.CompletedTask);

	[Benchmark]
	public async Task Arity4_PreCompleted() =>
		await (
			(Task)Task.CompletedTask, Task.CompletedTask,
			Task.CompletedTask, Task.CompletedTask);

	[Benchmark]
	public async Task Arity8_PreCompleted() =>
		await (
			(Task)Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask,
			Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask);

	[Benchmark]
	public async Task Arity16_PreCompleted() =>
		await (
			(Task)Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask,
			Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask,
			Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask,
			Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask);

	[Benchmark]
	public async Task Arity2_Async() =>
		await (YieldAsync(), YieldAsync());

	[Benchmark]
	public async Task Arity4_Async() =>
		await (YieldAsync(), YieldAsync(), YieldAsync(), YieldAsync());

	[Benchmark]
	public async Task Arity8_Async() =>
		await (
			YieldAsync(), YieldAsync(), YieldAsync(), YieldAsync(),
			YieldAsync(), YieldAsync(), YieldAsync(), YieldAsync());

	[Benchmark]
	public async Task Arity16_Async() =>
		await (
			YieldAsync(), YieldAsync(), YieldAsync(), YieldAsync(),
			YieldAsync(), YieldAsync(), YieldAsync(), YieldAsync(),
			YieldAsync(), YieldAsync(), YieldAsync(), YieldAsync(),
			YieldAsync(), YieldAsync(), YieldAsync(), YieldAsync());

	static async Task YieldAsync() => await Task.Yield();
}
