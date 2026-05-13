# TaskTupleAwaiter.Benchmarks

BenchmarkDotNet harness measuring the allocation and time profile of awaiting `ValueTuple` of `Task` / `Task<T>` across:

- Arities 2, 4, 8, 16
- Typed (`Task<T>`) and non-generic (`Task`) tuples
- Pre-completed (`Task.FromResult`) and async (`Task.Yield`) completion modes
- `ConfigureAwait(bool)` and `ConfigureAwait(ConfigureAwaitOptions)` paths

The point of these benchmarks is to compare allocation profiles between `net8.0` (where the generated `Task.WhenAll(...)` call binds to `params Task[]` and heap-allocates the array) and `net10.0` (where it binds to `Task.WhenAll(ReadOnlySpan<Task>)` and stack-allocates).

## Running

Run all benchmarks on net10.0:

```sh
dotnet run -c Release --project test/TaskTupleAwaiter.Benchmarks -f net10.0
```

Run all benchmarks on net8.0:

```sh
dotnet run -c Release --project test/TaskTupleAwaiter.Benchmarks -f net8.0
```

Filter to one class:

```sh
dotnet run -c Release --project test/TaskTupleAwaiter.Benchmarks -f net10.0 -- --filter "*TypedTupleAwaitBenchmarks*"
```

## Expected outcome

After the generator change to emit collection-expression `Task.WhenAll([...])`:

- **net8.0:** allocations and timing unchanged from baseline. Same IL as today.
- **net10.0:** `Allocated` per op drops by approximately `24 + 8·N` bytes (the `Task[N]` array we no longer allocate). Mean time per op is flat or slightly improved due to reduced GC pressure.

## Not run in CI

Benchmark runs are slow (multi-minute) and have enough run-to-run variance that they are unsuitable for CI gating. They are a manual local check before tagging a release or when validating perf-sensitive changes.
