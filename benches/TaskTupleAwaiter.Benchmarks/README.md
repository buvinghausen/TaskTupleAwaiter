# TaskTupleAwaiter.Benchmarks

BenchmarkDotNet harness measuring the allocation and time profile of awaiting `ValueTuple` of `Task` / `Task<T>` across:

- Arities 2, 4, 8, 16
- Typed (`Task<T>`) and non-generic (`Task`) tuples
- Pre-completed (`Task.FromResult`) and async (`Task.Yield`) completion modes
- `ConfigureAwait(bool)` and `ConfigureAwait(ConfigureAwaitOptions)` paths

The harness runs on `net11.0` and consumes the library's `net10.0` asset — the same asset a .NET 11 app gets from the package. That asset's generated `Task.WhenAll(...)` call binds to `Task.WhenAll(ReadOnlySpan<Task>)`, so the `Task[N]` array is stack-allocated rather than heap-allocated (the original net8.0 → net9.0 comparison is recorded in `docs/superpowers/specs/2026-05-13-net9-span-whenall-benchmark-results.md`).

## Running

Run all benchmarks:

```sh
dotnet run -c Release --project benches/TaskTupleAwaiter.Benchmarks
```

Filter to one class:

```sh
dotnet run -c Release --project benches/TaskTupleAwaiter.Benchmarks -- --filter "*TypedTupleAwaitBenchmarks*"
```

## Not run in CI

Benchmark runs are slow (multi-minute) and have enough run-to-run variance that they are unsuitable for CI gating. They are a manual local check before tagging a release or when validating perf-sensitive changes.
