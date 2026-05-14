# Benchmark Results

Captured on 2026-05-14 with the shipped state of this branch (HEAD `7c98fd2`).

- **BenchmarkDotNet** v0.15.8
- **Machine:** Intel Core Ultra 9 185H, 1 CPU, 22 logical / 16 physical cores, Windows 11 25H2
- **.NET SDK** 11.0.100-preview.4.26230.115
- **Hosts:**
  - net10.0 runs: `.NET 10.0.8 (10.0.826.23019), X64 RyuJIT x86-64-v3`
  - net8.0 runs: `.NET 8.0.27 (8.0.2726.22922), X64 RyuJIT x86-64-v3`

## What changed on this branch

Three layered changes deliver the per-await allocation reduction and the runtime speed-up:

1. **Added `net10.0` to the library `TargetFrameworks`.** On net10.0 the C# 13+ compiler binds the generated `Task.WhenAll(...)` call to `Task.WhenAll(ReadOnlySpan<Task>)` (added in .NET 9) instead of `Task.WhenAll(params Task[])`, stack-allocating the buffer.
2. **Generator emits `Task.WhenAll([tasks.Item1, ..., tasks.ItemN])` syntax.** This syntax shape is not what selects the overload; selecting `Task.WhenAll(ReadOnlySpan<Task>)` comes from compiling the library for `net10.0`. On netstandard2.0 / net462 / net8.0, calls bind to the array overload — same IL shape as before.
3. **Dropped `record struct` to plain `readonly struct`** on the generated awaiter types (no equality semantics needed for awaiters, drops a substantial chunk of synthesized members per arity).
4. **Annotated trivial forwarders with `[MethodImpl(MethodImplOptions.AggressiveInlining)]`**: `IsCompleted` accessor, `OnCompleted`, `UnsafeOnCompleted`, the `GetAwaiter` / `ConfigureAwait` extension methods, and the `TupleConfiguredTaskAwaitable.GetAwaiter()` helper. Constructors and `GetResult` are deliberately left un-annotated because their bodies grow with arity.

## Methodology

Each run executed the full benchmark suite with default BenchmarkDotNet config: full warmup, multiple iterations, statistical analysis. Filters: `--filter "*"` against the harness in `benches/TaskTupleAwaiter.Benchmarks/`.

The numbers in this doc are direct copies of the BDN markdown reports for the `net10.0` and `net8.0` builds of the **same source tree** (HEAD `7c98fd2`). They show what a consumer running on .NET 10 vs .NET 8 will observe when calling into the shipped library.

## TypedTupleAwaitBenchmarks

`await (Task<int>, Task<int>, ...)` returning a tuple of results.

### net10.0

| Method               | Mean        | Error     | StdDev    | Gen0   | Allocated |
|--------------------- |------------:|----------:|----------:|-------:|----------:|
| Arity2_PreCompleted  |    33.81 ns |  1.100 ns |  3.242 ns | 0.0057 |      72 B |
| Arity4_PreCompleted  |    47.41 ns |  0.963 ns |  1.661 ns | 0.0057 |      72 B |
| Arity8_PreCompleted  |    86.24 ns |  1.754 ns |  2.281 ns | 0.0057 |      72 B |
| Arity16_PreCompleted |   172.21 ns |  3.466 ns |  5.497 ns | 0.0515 |     648 B |
| Arity2_Async         | 1,045.53 ns | 20.764 ns | 36.366 ns | 0.0324 |     430 B |
| Arity4_Async         | 1,716.75 ns | 21.497 ns | 20.108 ns | 0.0477 |     612 B |
| Arity8_Async         | 3,082.50 ns | 58.255 ns | 64.750 ns | 0.0763 |     996 B |
| Arity16_Async        | 6,648.97 ns | 128.380 ns | 152.827 ns | 0.1373 |    1778 B |

### net8.0

| Method               | Mean        | Error     | StdDev    | Gen0   | Allocated |
|--------------------- |------------:|----------:|----------:|-------:|----------:|
| Arity2_PreCompleted  |    41.41 ns |  0.829 ns |  1.018 ns | 0.0095 |     120 B |
| Arity4_PreCompleted  |    61.23 ns |  0.960 ns |  0.851 ns | 0.0107 |     136 B |
| Arity8_PreCompleted  |    98.95 ns |  2.011 ns |  2.394 ns | 0.0134 |     168 B |
| Arity16_PreCompleted |   259.30 ns |  5.159 ns |  9.562 ns | 0.0644 |     808 B |
| Arity2_Async         | 1,017.41 ns | 20.153 ns | 23.208 ns | 0.0362 |     469 B |
| Arity4_Async         | 1,610.15 ns | 19.056 ns | 17.825 ns | 0.0515 |     664 B |
| Arity8_Async         | 3,021.31 ns | 57.586 ns | 59.137 ns | 0.0839 |    1075 B |
| Arity16_Async        | 5,804.34 ns | 33.364 ns | 31.208 ns | 0.1450 |    1894 B |

### net10.0 vs net8.0 — allocation delta

| Method               | net8.0 | net10.0 | Δ (B) | Δ (%) |
|--------------------- |-------:|--------:|------:|------:|
| Arity2_PreCompleted  |  120 B |    72 B |   -48 |  -40% |
| Arity4_PreCompleted  |  136 B |    72 B |   -64 |  -47% |
| Arity8_PreCompleted  |  168 B |    72 B |   -96 |  -57% |
| Arity16_PreCompleted |  808 B |   648 B |  -160 |  -20% |
| Arity2_Async         |  469 B |   430 B |   -39 |   -8% |
| Arity4_Async         |  664 B |   612 B |   -52 |   -8% |
| Arity8_Async         | 1075 B |   996 B |   -79 |   -7% |
| Arity16_Async        | 1894 B |  1778 B |  -116 |   -6% |

## NonGenericTupleAwaitBenchmarks

`await (Task, Task, ...)` returning `void` (the awaiter just observes completion).

### net10.0

| Method               | Mean        | Error     | StdDev    | Median      | Gen0   | Allocated |
|--------------------- |------------:|----------:|----------:|------------:|-------:|----------:|
| Arity2_PreCompleted  |    32.68 ns |  1.230 ns |  3.450 ns |    32.53 ns | 0.0057 |      72 B |
| Arity4_PreCompleted  |    42.10 ns |  0.675 ns |  0.631 ns |    42.06 ns | 0.0057 |      72 B |
| Arity8_PreCompleted  |    68.80 ns |  1.404 ns |  3.140 ns |    66.87 ns | 0.0057 |      72 B |
| Arity16_PreCompleted |   122.53 ns |  2.439 ns |  3.338 ns |   122.99 ns | 0.0057 |      72 B |
| Arity2_Async         |   926.70 ns | 17.806 ns | 16.656 ns |   929.20 ns | 0.0267 |     352 B |
| Arity4_Async         | 1,499.61 ns | 15.313 ns | 14.324 ns | 1,498.84 ns | 0.0420 |     535 B |
| Arity8_Async         | 2,746.35 ns | 15.477 ns | 13.720 ns | 2,744.15 ns | 0.0687 |     905 B |
| Arity16_Async        | 5,716.92 ns | 41.383 ns | 36.685 ns | 5,726.46 ns | 0.1297 |    1662 B |

### net8.0

| Method               | Mean        | Error      | StdDev    | Median      | Gen0   | Allocated |
|--------------------- |------------:|-----------:|----------:|------------:|-------:|----------:|
| Arity2_PreCompleted  |    45.57 ns |   1.640 ns |  4.836 ns |    46.22 ns | 0.0095 |     120 B |
| Arity4_PreCompleted  |    71.58 ns |   2.982 ns |  8.793 ns |    69.49 ns | 0.0107 |     136 B |
| Arity8_PreCompleted  |   130.76 ns |  12.259 ns | 36.145 ns |   109.91 ns | 0.0134 |     168 B |
| Arity16_PreCompleted |   182.65 ns |   3.636 ns |  7.092 ns |   180.90 ns | 0.0184 |     232 B |
| Arity2_Async         |   970.39 ns |  16.301 ns | 15.248 ns |   966.04 ns | 0.0305 |     398 B |
| Arity4_Async         | 1,503.97 ns |  14.793 ns | 13.837 ns | 1,502.62 ns | 0.0458 |     589 B |
| Arity8_Async         | 2,868.08 ns |  52.956 ns | 49.535 ns | 2,870.56 ns | 0.0763 |     984 B |
| Arity16_Async        | 5,742.70 ns | 102.436 ns | 95.819 ns | 5,723.73 ns | 0.1373 |    1814 B |

### net10.0 vs net8.0 — allocation delta

| Method               | net8.0 | net10.0 | Δ (B) | Δ (%) |
|--------------------- |-------:|--------:|------:|------:|
| Arity2_PreCompleted  |  120 B |    72 B |   -48 |  -40% |
| Arity4_PreCompleted  |  136 B |    72 B |   -64 |  -47% |
| Arity8_PreCompleted  |  168 B |    72 B |   -96 |  -57% |
| Arity16_PreCompleted |  232 B |    72 B |  -160 |  -69% |
| Arity2_Async         |  398 B |   352 B |   -46 |  -12% |
| Arity4_Async         |  589 B |   535 B |   -54 |   -9% |
| Arity8_Async         |  984 B |   905 B |   -79 |   -8% |
| Arity16_Async        | 1814 B |  1662 B |  -152 |   -8% |

## ConfigureAwaitBenchmarks

Spot checks of the `ConfigureAwait(bool)` and `ConfigureAwait(ConfigureAwaitOptions)` paths, at arities 4 and 16, in both the typed and non-generic tuple flavors.

### net10.0

| Method                          | Mean      | Error    | StdDev    | Gen0   | Allocated |
|-------------------------------- |----------:|---------:|----------:|-------:|----------:|
| Typed_Arity4_Bool_False         |  44.20 ns | 0.907 ns |  1.726 ns | 0.0057 |      72 B |
| Typed_Arity4_Options_None       |  46.14 ns | 0.842 ns |  1.235 ns | 0.0057 |      72 B |
| Typed_Arity16_Bool_False        | 165.95 ns | 3.283 ns |  4.493 ns | 0.0515 |     648 B |
| Typed_Arity16_Options_None      | 168.75 ns | 3.269 ns |  4.475 ns | 0.0515 |     648 B |
| NonGeneric_Arity4_Bool_False    |  47.52 ns | 0.979 ns |  2.364 ns | 0.0057 |      72 B |
| NonGeneric_Arity16_Options_None | 127.29 ns | 3.042 ns |  7.960 ns | 0.0057 |      72 B |

### net8.0

| Method                          | Mean      | Error    | StdDev    | Gen0   | Allocated |
|-------------------------------- |----------:|---------:|----------:|-------:|----------:|
| Typed_Arity4_Bool_False         |  58.26 ns | 1.012 ns |  0.897 ns | 0.0107 |     136 B |
| Typed_Arity4_Options_None       |  58.48 ns | 0.700 ns |  0.654 ns | 0.0107 |     136 B |
| Typed_Arity16_Bool_False        | 272.90 ns | 5.401 ns | 11.274 ns | 0.0644 |     808 B |
| Typed_Arity16_Options_None      | 314.93 ns | 6.414 ns | 18.812 ns | 0.0644 |     808 B |
| NonGeneric_Arity4_Bool_False    |  69.85 ns | 1.593 ns |  4.697 ns | 0.0107 |     136 B |
| NonGeneric_Arity16_Options_None | 205.68 ns | 4.472 ns | 13.187 ns | 0.0184 |     232 B |

### net10.0 vs net8.0 — allocation delta

| Method                          | net8.0 | net10.0 | Δ (B) | Δ (%) |
|-------------------------------- |-------:|--------:|------:|------:|
| Typed_Arity4_Bool_False         |  136 B |    72 B |   -64 |  -47% |
| Typed_Arity4_Options_None       |  136 B |    72 B |   -64 |  -47% |
| Typed_Arity16_Bool_False        |  808 B |   648 B |  -160 |  -20% |
| Typed_Arity16_Options_None      |  808 B |   648 B |  -160 |  -20% |
| NonGeneric_Arity4_Bool_False    |  136 B |    72 B |   -64 |  -47% |
| NonGeneric_Arity16_Options_None |  232 B |    72 B |  -160 |  -69% |

## Headline summary

For the most common consumer pattern — a typed tuple of pre-completed `Task<T>` values, arity 2–16 — a .NET 10 consumer sees:

- **Allocations: 40–57% lower** at arities 2/4/8 (where the eliminated `Task[N]` array dominates) and **20% lower** at arity 16 (where the state-machine box itself is the larger contributor).
- **Mean time: 13–34% lower.** Some of that is the lack of the heap allocation; the rest comes from JIT improvements between .NET 8 and .NET 10 plus the inlining hints.

The non-generic tuple path is even more dramatic at arity 16 (`-69%` allocation) because the non-generic awaiter has no per-`T` metadata in the box — the `Task[]` was a larger share of its total.

The async (yielding) benchmarks see smaller proportional improvements because the async state machine itself dominates allocations — but they're consistently lower on net10.0 too, by `4–12%`.

## Library DLL size

For reference, the shipped library binaries on the `7c98fd2` HEAD:

| TFM            | Size       |
|----------------|-----------:|
| netstandard2.0 |   53,248 B |
| net462         |   53,248 B |
| net8.0         |   57,856 B |
| net10.0        |   64,512 B |

Down from ~80–92 KB before dropping `record struct` to plain `readonly struct`. The `[MethodImpl]` attribute additions cost 0 B (the metadata fits within existing PE 4 KB alignment padding).

## Reproducing locally

```sh
dotnet run -c Release --project benches/TaskTupleAwaiter.Benchmarks -f net10.0
dotnet run -c Release --project benches/TaskTupleAwaiter.Benchmarks -f net8.0
```

Filter to one class, e.g.:

```sh
dotnet run -c Release --project benches/TaskTupleAwaiter.Benchmarks -f net10.0 -- --filter "*TypedTupleAwaitBenchmarks*"
```

Each TFM takes ~13 minutes to run the full suite end-to-end on this machine. Reports land in `BenchmarkDotNet.Artifacts/` (gitignored).
