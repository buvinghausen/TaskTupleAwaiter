using BenchmarkDotNet.Attributes;

namespace TaskTupleAwaiter.Benchmarks;

[MemoryDiagnoser]
public class ConfigureAwaitBenchmarks
{
	[Benchmark]
	public async Task Typed_Arity4_Bool_False() =>
		_ = await (
			Task.FromResult(1), Task.FromResult(2),
			Task.FromResult(3), Task.FromResult(4)).ConfigureAwait(false);

	[Benchmark]
	public async Task Typed_Arity4_Options_None() =>
		_ = await (
			Task.FromResult(1), Task.FromResult(2),
			Task.FromResult(3), Task.FromResult(4)).ConfigureAwait(ConfigureAwaitOptions.None);

	[Benchmark]
	public async Task Typed_Arity16_Bool_False() =>
		_ = await (
			Task.FromResult(1), Task.FromResult(2), Task.FromResult(3), Task.FromResult(4),
			Task.FromResult(5), Task.FromResult(6), Task.FromResult(7), Task.FromResult(8),
			Task.FromResult(9), Task.FromResult(10), Task.FromResult(11), Task.FromResult(12),
			Task.FromResult(13), Task.FromResult(14), Task.FromResult(15), Task.FromResult(16)
		).ConfigureAwait(false);

	[Benchmark]
	public async Task Typed_Arity16_Options_None() =>
		_ = await (
			Task.FromResult(1), Task.FromResult(2), Task.FromResult(3), Task.FromResult(4),
			Task.FromResult(5), Task.FromResult(6), Task.FromResult(7), Task.FromResult(8),
			Task.FromResult(9), Task.FromResult(10), Task.FromResult(11), Task.FromResult(12),
			Task.FromResult(13), Task.FromResult(14), Task.FromResult(15), Task.FromResult(16)
		).ConfigureAwait(ConfigureAwaitOptions.None);

	[Benchmark]
	public async Task NonGeneric_Arity4_Bool_False() =>
		await (
			(Task)Task.CompletedTask, Task.CompletedTask,
			Task.CompletedTask, Task.CompletedTask).ConfigureAwait(false);

	[Benchmark]
	public async Task NonGeneric_Arity16_Options_None() =>
		await (
			(Task)Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask,
			Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask,
			Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask,
			Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask
		).ConfigureAwait(ConfigureAwaitOptions.None);
}
