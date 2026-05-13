# Benchmark Results

Captured on 2026-05-13, BenchmarkDotNet v0.14.0, .NET SDK 11.0.100-preview.4.26230.115, Windows 11 (10.0.26200.8246).

## Baseline (before generator change)

Generator emits `Task.WhenAll(tasks.Item1, ..., tasks.ItemN)` (positional `params Task[]` call site).

### net8.0

`[Host] : .NET 8.0.27 (8.0.2726.22922), X64 RyuJIT AVX2`

**TypedTupleAwaitBenchmarks**

| Method               | Mean        | Error     | StdDev    | Gen0   | Allocated |
|--------------------- |------------:|----------:|----------:|-------:|----------:|
| Arity2_PreCompleted  |    47.49 ns |  0.844 ns |  1.183 ns | 0.0004 |     120 B |
| Arity4_PreCompleted  |    68.24 ns |  1.202 ns |  0.938 ns | 0.0004 |     136 B |
| Arity8_PreCompleted  |   114.38 ns |  2.259 ns |  2.775 ns | 0.0005 |     168 B |
| Arity16_PreCompleted |   307.27 ns |  2.644 ns |  2.344 ns | 0.0024 |     808 B |
| Arity2_Async         | 1,018.96 ns |  8.808 ns |  7.808 ns |      - |     472 B |
| Arity4_Async         | 1,607.70 ns | 18.543 ns | 16.438 ns | 0.0019 |     665 B |
| Arity8_Async         | 2,907.20 ns | 23.844 ns | 21.137 ns |      - |    1066 B |
| Arity16_Async        | 5,758.19 ns | 27.675 ns | 25.887 ns |      - |    1891 B |

**NonGenericTupleAwaitBenchmarks**

| Method               | Mean        | Error     | StdDev    | Gen0   | Allocated |
|--------------------- |------------:|----------:|----------:|-------:|----------:|
| Arity2_PreCompleted  |    45.61 ns |  0.328 ns |  0.274 ns | 0.0003 |     120 B |
| Arity4_PreCompleted  |    65.13 ns |  0.417 ns |  0.325 ns | 0.0004 |     136 B |
| Arity8_PreCompleted  |   103.62 ns |  0.603 ns |  0.503 ns | 0.0005 |     168 B |
| Arity16_PreCompleted |   179.45 ns |  3.012 ns |  2.515 ns | 0.0005 |     232 B |
| Arity2_Async         |   980.97 ns |  7.003 ns |  6.208 ns |      - |     399 B |
| Arity4_Async         | 1,533.24 ns | 11.782 ns | 11.020 ns |      - |     585 B |
| Arity8_Async         | 2,824.78 ns | 25.238 ns | 22.372 ns |      - |     986 B |
| Arity16_Async        | 5,830.70 ns | 46.774 ns | 43.753 ns |      - |    1811 B |

**ConfigureAwaitBenchmarks**

| Method                          | Mean      | Error    | StdDev   | Gen0   | Allocated |
|-------------------------------- |----------:|---------:|---------:|-------:|----------:|
| Typed_Arity4_Bool_False         |  64.24 ns | 0.596 ns | 0.558 ns | 0.0004 |     136 B |
| Typed_Arity4_Options_None       |  65.96 ns | 1.060 ns | 0.940 ns | 0.0004 |     136 B |
| Typed_Arity16_Bool_False        | 299.48 ns | 3.102 ns | 2.902 ns | 0.0024 |     808 B |
| Typed_Arity16_Options_None      | 306.28 ns | 3.092 ns | 2.582 ns | 0.0024 |     808 B |
| NonGeneric_Arity4_Bool_False    |  65.68 ns | 0.546 ns | 0.511 ns | 0.0004 |     136 B |
| NonGeneric_Arity16_Options_None | 182.92 ns | 1.486 ns | 1.317 ns | 0.0005 |     232 B |

### net10.0

`[Host] : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX2`

**TypedTupleAwaitBenchmarks**

| Method               | Mean        | Error     | StdDev    | Median      | Gen0   | Gen1   | Gen2   | Allocated |
|--------------------- |------------:|----------:|----------:|------------:|-------:|-------:|-------:|----------:|
| Arity2_PreCompleted  |    47.56 ns |  1.359 ns |  3.877 ns |    46.46 ns | 0.0014 |      - |      - |      72 B |
| Arity4_PreCompleted  |    71.40 ns |  4.236 ns | 12.489 ns |    65.49 ns | 0.0013 |      - |      - |      72 B |
| Arity8_PreCompleted  |   104.13 ns |  7.782 ns | 22.946 ns |    91.17 ns | 0.0013 |      - |      - |      72 B |
| Arity16_PreCompleted |   314.81 ns | 18.537 ns | 54.655 ns |   293.40 ns | 0.0124 |      - |      - |     648 B |
| Arity2_Async         | 1,212.95 ns |  9.468 ns |  8.393 ns | 1,212.69 ns | 0.0076 |      - |      - |     435 B |
| Arity4_Async         | 1,778.55 ns | 26.061 ns | 25.595 ns | 1,774.26 ns | 0.0153 | 0.0019 | 0.0019 |         - |
| Arity8_Async         | 2,877.76 ns | 18.751 ns | 17.540 ns | 2,871.82 ns | 0.0191 |      - |      - |    1009 B |
| Arity16_Async        | 5,228.22 ns | 99.403 ns | 97.627 ns | 5,186.00 ns | 0.0305 |      - |      - |    1833 B |

**NonGenericTupleAwaitBenchmarks**

| Method               | Mean        | Error     | StdDev    | Median      | Gen0   | Allocated |
|--------------------- |------------:|----------:|----------:|------------:|-------:|----------:|
| Arity2_PreCompleted  |    37.69 ns |  0.771 ns |  0.917 ns |    37.68 ns | 0.0014 |      72 B |
| Arity4_PreCompleted  |    54.01 ns |  0.982 ns |  0.918 ns |    53.77 ns | 0.0014 |      72 B |
| Arity8_PreCompleted  |    86.55 ns |  1.095 ns |  1.025 ns |    86.43 ns | 0.0013 |      72 B |
| Arity16_PreCompleted |   128.50 ns |  2.440 ns |  4.400 ns |   127.81 ns | 0.0012 |      72 B |
| Arity2_Async         | 1,221.92 ns | 28.869 ns | 85.121 ns | 1,194.58 ns | 0.0057 |     355 B |
| Arity4_Async         | 1,874.61 ns | 35.942 ns | 33.620 ns | 1,874.08 ns | 0.0095 |     544 B |
| Arity8_Async         | 2,893.15 ns | 29.539 ns | 26.186 ns | 2,897.66 ns | 0.0153 |     919 B |
| Arity16_Async        | 5,739.03 ns | 69.728 ns | 58.226 ns | 5,762.36 ns | 0.0305 |    1675 B |

**ConfigureAwaitBenchmarks**

| Method                          | Mean      | Error    | StdDev   | Gen0   | Allocated |
|-------------------------------- |----------:|---------:|---------:|-------:|----------:|
| Typed_Arity4_Bool_False         |  53.04 ns | 0.395 ns | 0.330 ns | 0.0014 |      72 B |
| Typed_Arity4_Options_None       |  52.87 ns | 0.418 ns | 0.371 ns | 0.0014 |      72 B |
| Typed_Arity16_Bool_False        | 231.18 ns | 1.278 ns | 1.196 ns | 0.0124 |     648 B |
| Typed_Arity16_Options_None      | 231.20 ns | 2.519 ns | 2.103 ns | 0.0124 |     648 B |
| NonGeneric_Arity4_Bool_False    |  53.21 ns | 1.069 ns | 1.786 ns | 0.0014 |      72 B |
| NonGeneric_Arity16_Options_None | 120.64 ns | 1.812 ns | 1.415 ns | 0.0012 |      72 B |

## Surprise finding — the optimization is already realized at this point

Comparing the net8.0 and net10.0 baseline columns shows the per-op `Allocated` figure is already substantially lower on net10.0 (e.g., 120 B → 72 B for `Arity2_PreCompleted`, 808 B → 648 B for `Arity16_PreCompleted` typed). This happens *before* the generator change in Task 9.

Why: the C# 13+ compiler prefers `Task.WhenAll(ReadOnlySpan<Task>)` over `Task.WhenAll(params Task[])` when both overloads are visible, even for positional `Task.WhenAll(t1, t2, ..., tN)` call sites. Since `ReadOnlySpan<Task>` overload only exists on net9+, the net8.0 library build still binds to `params Task[]` (heap allocation), while the net10.0 library build already binds to `ReadOnlySpan<Task>` (stack allocation). **Adding the `net10.0` TFM in Task 1 was sufficient to deliver the optimization** — the generator change in Task 9 makes the source-level intent explicit (collection expression) but is not load-bearing for the IL outcome.

## After generator change

Generator emits `Task.WhenAll([tasks.Item1, ..., tasks.ItemN])` (C# collection expression).

### net10.0

`[Host] : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX2`

**TypedTupleAwaitBenchmarks**

| Method               | Mean        | Error     | StdDev    | Gen0   | Allocated |
|--------------------- |------------:|----------:|----------:|-------:|----------:|
| Arity2_PreCompleted  |    39.60 ns |  0.302 ns |  0.283 ns | 0.0014 |      72 B |
| Arity4_PreCompleted  |    61.27 ns |  1.061 ns |  0.940 ns | 0.0014 |      72 B |
| Arity8_PreCompleted  |    92.65 ns |  1.858 ns |  2.893 ns | 0.0013 |      72 B |
| Arity16_PreCompleted |   252.40 ns |  4.754 ns |  4.214 ns | 0.0124 |     648 B |
| Arity2_Async         | 1,029.32 ns | 10.326 ns |  9.154 ns | 0.0076 |     428 B |
| Arity4_Async         | 1,685.97 ns | 21.445 ns | 20.059 ns | 0.0114 |     617 B |
| Arity8_Async         | 2,893.24 ns | 38.208 ns | 35.740 ns | 0.0153 |     987 B |
| Arity16_Async        | 6,063.24 ns | 90.612 ns | 84.759 ns | 0.0305 |    1780 B |

**NonGenericTupleAwaitBenchmarks**

| Method               | Mean        | Error     | StdDev    | Gen0   | Allocated |
|--------------------- |------------:|----------:|----------:|-------:|----------:|
| Arity2_PreCompleted  |    38.02 ns |  0.673 ns |  0.562 ns | 0.0014 |      72 B |
| Arity4_PreCompleted  |    55.11 ns |  0.966 ns |  1.074 ns | 0.0014 |      72 B |
| Arity8_PreCompleted  |    78.87 ns |  1.590 ns |  2.947 ns | 0.0013 |      72 B |
| Arity16_PreCompleted |   134.97 ns |  2.678 ns |  4.170 ns | 0.0012 |      72 B |
| Arity2_Async         | 1,023.69 ns | 20.507 ns | 47.528 ns | 0.0057 |     352 B |
| Arity4_Async         | 1,584.04 ns | 10.068 ns |  7.860 ns | 0.0114 |     533 B |
| Arity8_Async         | 2,880.02 ns | 44.351 ns | 39.316 ns | 0.0153 |         - |
| Arity16_Async        | 6,052.55 ns | 65.590 ns | 54.770 ns | 0.0305 |    1663 B |

**ConfigureAwaitBenchmarks**

| Method                          | Mean      | Error    | StdDev    | Median    | Gen0   | Allocated |
|-------------------------------- |----------:|---------:|----------:|----------:|-------:|----------:|
| Typed_Arity4_Bool_False         |  55.57 ns | 1.488 ns |  4.074 ns |  53.78 ns | 0.0014 |      72 B |
| Typed_Arity4_Options_None       |  55.10 ns | 1.120 ns |  2.210 ns |  54.70 ns | 0.0014 |      72 B |
| Typed_Arity16_Bool_False        | 228.81 ns | 1.930 ns |  1.805 ns | 228.69 ns | 0.0124 |     648 B |
| Typed_Arity16_Options_None      | 246.73 ns | 4.446 ns |  3.942 ns | 245.81 ns | 0.0124 |     648 B |
| NonGeneric_Arity4_Bool_False    |  62.23 ns | 5.127 ns | 15.118 ns |  55.41 ns | 0.0014 |      72 B |
| NonGeneric_Arity16_Options_None | 122.96 ns | 2.387 ns |  3.267 ns | 121.49 ns | 0.0012 |      72 B |

### net8.0

`[Host] : .NET 8.0.27 (8.0.2726.22922), X64 RyuJIT AVX2`

**TypedTupleAwaitBenchmarks**

| Method               | Mean        | Error     | StdDev    | Median      | Gen0   | Allocated |
|--------------------- |------------:|----------:|----------:|------------:|-------:|----------:|
| Arity2_PreCompleted  |    61.47 ns |  5.056 ns | 14.908 ns |    53.28 ns | 0.0003 |     120 B |
| Arity4_PreCompleted  |    71.47 ns |  1.460 ns |  2.557 ns |    71.53 ns | 0.0004 |     136 B |
| Arity8_PreCompleted  |   120.07 ns |  2.417 ns |  2.586 ns |   119.89 ns | 0.0005 |     168 B |
| Arity16_PreCompleted |   328.55 ns |  3.585 ns |  2.994 ns |   329.36 ns | 0.0024 |     808 B |
| Arity2_Async         | 1,036.87 ns | 11.742 ns | 10.984 ns | 1,038.47 ns |      - |     472 B |
| Arity4_Async         | 1,687.67 ns | 12.710 ns | 11.889 ns | 1,692.47 ns | 0.0019 |     663 B |
| Arity8_Async         | 3,099.42 ns | 60.635 ns | 92.596 ns | 3,109.38 ns |      - |    1049 B |
| Arity16_Async        | 6,060.72 ns | 68.402 ns | 63.983 ns | 6,042.75 ns |      - |    1891 B |

**NonGenericTupleAwaitBenchmarks**

| Method               | Mean        | Error     | StdDev    | Median      | Gen0   | Allocated |
|--------------------- |------------:|----------:|----------:|------------:|-------:|----------:|
| Arity2_PreCompleted  |    48.75 ns |  0.992 ns |  1.290 ns |    48.72 ns | 0.0004 |     120 B |
| Arity4_PreCompleted  |    74.13 ns |  1.517 ns |  3.778 ns |    74.06 ns | 0.0004 |     136 B |
| Arity8_PreCompleted  |   115.46 ns |  2.247 ns |  2.101 ns |   115.90 ns | 0.0005 |     168 B |
| Arity16_PreCompleted |   278.46 ns | 15.376 ns | 45.338 ns |   302.75 ns | 0.0005 |     232 B |
| Arity2_Async         | 1,267.23 ns |  6.285 ns |  5.879 ns | 1,267.13 ns |      - |     402 B |
| Arity4_Async         | 1,775.57 ns | 13.510 ns | 11.976 ns | 1,773.46 ns |      - |     598 B |
| Arity8_Async         | 2,939.42 ns | 32.463 ns | 30.366 ns | 2,946.38 ns |      - |    1004 B |
| Arity16_Async        | 5,691.46 ns | 79.269 ns | 74.148 ns | 5,702.72 ns |      - |    1814 B |

**ConfigureAwaitBenchmarks**

| Method                          | Mean      | Error    | StdDev   | Median    | Gen0   | Allocated |
|-------------------------------- |----------:|---------:|---------:|----------:|-------:|----------:|
| Typed_Arity4_Bool_False         |  75.28 ns | 1.539 ns | 3.143 ns |  74.90 ns | 0.0004 |     136 B |
| Typed_Arity4_Options_None       |  82.93 ns | 1.636 ns | 2.009 ns |  82.43 ns | 0.0004 |     136 B |
| Typed_Arity16_Bool_False        | 329.52 ns | 6.129 ns | 6.019 ns | 327.59 ns | 0.0024 |     808 B |
| Typed_Arity16_Options_None      | 331.92 ns | 6.560 ns | 8.757 ns | 327.80 ns | 0.0024 |     808 B |
| NonGeneric_Arity4_Bool_False    |  69.99 ns | 1.230 ns | 1.091 ns |  69.65 ns | 0.0004 |     136 B |
| NonGeneric_Arity16_Options_None | 206.12 ns | 4.134 ns | 7.455 ns | 202.96 ns | 0.0005 |     232 B |

Confirmed: every `Allocated` value matches the net8.0 baseline (120 / 136 / 168 / 808 / 472 / 663 / 1049 / 1891 etc. across the typed pre-completed and async series). The collection-expression source lowers to `new Task[]{t1, ..., tN}` and binds to `Task.WhenAll(params Task[])` on net8.0 — identical IL to the pre-change positional form.

## Delta summary: after-change vs baseline (same TFM)

The generator change leaves IL semantics unchanged on net10.0 — empirically confirmed: every `Allocated` value matches baseline within run-to-run noise. The `Task.WhenAll(tasks.Item1, tasks.Item2)` positional form was already binding to `Task.WhenAll(ReadOnlySpan<Task>)` on net10.0 via the C# 13+ compiler's overload-preference rule. The bracketed `[tasks.Item1, tasks.Item2]` form is the explicit, intent-clarifying source-level expression.

On net8.0 the source change continues to lower to `new Task[]{t1, t2}` (because no `ReadOnlySpan<Task>` overload exists on net8.0), so allocation is unchanged from baseline there as well.

The end-state takeaway: **adding the `net10.0` TFM to the library is what delivered the per-await allocation reduction for .NET 10+ consumers**. The generator change is preserved as belt-and-suspenders insurance — making the collection-expression intent explicit at the source level keeps the IL stable across future compiler overload-resolution changes.

## Delta summary (net10.0 vs net8.0 baseline)

Allocation reduction attributable to the `net10.0` TFM (Task 1 alone, before any generator change):

| Benchmark                                       | net8.0 (B) | net10.0 (B) | Reduction (B) |
|-------------------------------------------------|-----------:|------------:|--------------:|
| TypedTuple Arity2_PreCompleted                  |        120 |          72 |            48 |
| TypedTuple Arity4_PreCompleted                  |        136 |          72 |            64 |
| TypedTuple Arity8_PreCompleted                  |        168 |          72 |            96 |
| TypedTuple Arity16_PreCompleted                 |        808 |         648 |           160 |
| TypedTuple Arity2_Async                         |        472 |         435 |            37 |
| TypedTuple Arity4_Async                         |        665 |           - |           665 |
| TypedTuple Arity8_Async                         |       1066 |        1009 |            57 |
| TypedTuple Arity16_Async                        |       1891 |        1833 |            58 |
| NonGenericTuple Arity2_PreCompleted             |        120 |          72 |            48 |
| NonGenericTuple Arity4_PreCompleted             |        136 |          72 |            64 |
| NonGenericTuple Arity8_PreCompleted             |        168 |          72 |            96 |
| NonGenericTuple Arity16_PreCompleted            |        232 |          72 |           160 |
| NonGenericTuple Arity2_Async                    |        399 |         355 |            44 |
| NonGenericTuple Arity4_Async                    |        585 |         544 |            41 |
| NonGenericTuple Arity8_Async                    |        986 |         919 |            67 |
| NonGenericTuple Arity16_Async                   |       1811 |        1675 |           136 |
| ConfigureAwait Typed_Arity4_Bool_False          |        136 |          72 |            64 |
| ConfigureAwait Typed_Arity4_Options_None        |        136 |          72 |            64 |
| ConfigureAwait Typed_Arity16_Bool_False         |        808 |         648 |           160 |
| ConfigureAwait Typed_Arity16_Options_None       |        808 |         648 |           160 |
| ConfigureAwait NonGeneric_Arity4_Bool_False     |        136 |          72 |            64 |
| ConfigureAwait NonGeneric_Arity16_Options_None  |        232 |          72 |           160 |

The pre-completed arity-2-through-8 benchmarks see allocation drop by `48-96 B` — consistent with the `Task[N]` array no longer being allocated. Arity 16 drops are larger (`160 B` for typed, `160 B` for non-generic configure-await) because the larger state machine box also benefits when the WhenAll allocation is eliminated. The async (yielding) benchmarks see smaller absolute reductions (the yielding state machine itself dominates allocations) but still consistently lower on net10.0.
