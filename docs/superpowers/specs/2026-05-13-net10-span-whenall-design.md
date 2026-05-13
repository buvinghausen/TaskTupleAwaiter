# Design — net10.0 TFM + span-based `Task.WhenAll` + BenchmarkDotNet

**Date:** 2026-05-13
**Status:** Drafted, pending review

## Goal

Cut one `Task[]` heap allocation per `await` of a task tuple for consumers running on .NET 10+, without compromising the existing `netstandard2.0` / `net462` / `net8.0` surface or adding conditional compilation complexity. Establish a BenchmarkDotNet project so this and future perf claims can be measured rather than guessed.

## Background

The generator currently emits, inside every typed awaiter constructor and every non-generic extension method:

```csharp
Task.WhenAll(tasks.Item1, tasks.Item2, ..., tasks.ItemN)
```

That call binds to `Task.WhenAll(params Task[])`, which heap-allocates a `Task[N]` on every await. For a tuple of arity 16, that is 16 references plus array header per await.

.NET 9 added `Task.WhenAll(scoped ReadOnlySpan<Task>)` (and its `Task<TResult>` flavor). When a C# 13+ collection expression targets a method call that has both `params Task[]` and `ReadOnlySpan<Task>` overloads, the compiler prefers the span overload and stack-allocates the buffer — zero heap allocation.

.NET 11 / C# 15's headline feature for async perf is *runtime async*. It is a **caller-side compile feature**: the consumer's async method codegen changes. The library's awaiter structs already implement the standard `ICriticalNotifyCompletion` / `IsCompleted` / `GetResult` pattern, so consumers compiling with `runtime-async=on` benefit automatically. **No library changes are required for runtime async**, and the tests already opt in for `net11.0`.

## Scope

In scope:
1. Add `net10.0` to the library's `TargetFrameworks`.
2. Change the generator to emit `Task.WhenAll([t1, ..., tN])` (collection-expression form) in place of `Task.WhenAll(t1, ..., tN)`.
3. Add a `test/TaskTupleAwaiter.Benchmarks` project using BenchmarkDotNet.
4. Add `net10.0` to the AOT smoke-test TFMs so the new generated code path is verified under NativeAOT.

Out of scope:
- Dropping `record struct` in favor of `struct` (speculative; would be benchmark-driven future work).
- `[MethodImpl(AggressiveInlining)]` annotations (same reason).
- A `net11.0` TFM for the library (no library code difference between net10 and net11 — runtime async benefits consumers, not library authors).
- Heterogeneous `Task.WhenAll<T1, T2>` (does not exist in BCL).
- Pooling, custom WhenAll, or other in-house allocations work.

## Architecture

### Library TFMs

`src/TaskTupleAwaiter/TaskTupleAwaiter.csproj` adds `net10.0`:

```xml
<TargetFrameworks>netstandard2.0;net462;net8.0;net10.0</TargetFrameworks>
```

Resulting NuGet asset selection:

| Consumer TFM | Picks |
|---|---|
| .NET Framework 4.6.2+ | `net462` |
| .NET Standard 2.0 consumers | `netstandard2.0` |
| .NET 8 / .NET 9 | `net8.0` |
| .NET 10+ | `net10.0` ← new, gets span optimization |

.NET 9 consumers stay on `net8.0` rather than gaining a `net9.0` asset. This is deliberate: .NET 9 is STS and adding another TFM for one minor optimization isn't justified. Consumers wanting the win update to .NET 10 (current LTS).

### Generator change

`src/TaskTupleAwaiter.Generator/TaskTupleExtensionsGenerator.cs` — only the `Items(int arity)` helper changes (or its call sites), so that every `Task.WhenAll(tasks.Item1, ..., tasks.ItemN)` becomes `Task.WhenAll([tasks.Item1, ..., tasks.ItemN])`. This affects:

- `AppendTupleTaskAwaiterStruct` — typed awaiter constructor
- `AppendTupleConfiguredTaskAwaitableStruct` — typed configured awaiter inner constructor
- `AppendNonGenericArity` — both `GetAwaiter` and `ConfigureAwait` extension method bodies

**No feature detection is needed.** The C# compiler picks the overload per target framework:

- `netstandard2.0` / `net462` / `net8.0`: collection expression targets `params Task[]` — compiles to `new Task[]{...}`, identical IL to today.
- `net10.0`: collection expression targets `ReadOnlySpan<Task>` — compiles to a stack-allocated buffer via the inline-array pattern the compiler generates for collection expressions. Zero heap allocation for the task list.

Overload-resolution note: in the typed awaiter, the generic parameters `T1..TN` are distinct type parameters, so `Task.WhenAll<TResult>(params Task<TResult>[])` and its span sibling do not bind. Only the non-generic overloads (`params Task[]` and `ReadOnlySpan<Task>`) match — exactly what we want.

The existing `hasAwaitOptionsProvider` feature detection stays as-is; it controls API surface (`ConfigureAwaitOptions` overload) and is unrelated to this change.

### Benchmark project

Location: `test/TaskTupleAwaiter.Benchmarks/`

Structure:

```
test/TaskTupleAwaiter.Benchmarks/
├── TaskTupleAwaiter.Benchmarks.csproj
├── Program.cs
├── TypedTupleAwaitBenchmarks.cs
├── NonGenericTupleAwaitBenchmarks.cs
└── ConfigureAwaitBenchmarks.cs
```

`TaskTupleAwaiter.Benchmarks.csproj`:

- `<TargetFrameworks>net8.0;net10.0</TargetFrameworks>` — running benchmarks against both TFMs in one job produces a side-by-side allocation table.
- `<OutputType>Exe</OutputType>`, `<IsPackable>false</IsPackable>`.
- `PackageReference Include="BenchmarkDotNet"`.
- Same pattern as `TaskTupleAwaiter.AotSmokeTest`: remove the `Shouldly`, `Xunit` usings and `xunit.v3.mtp-v2` / `Shouldly` package references that `test/Directory.Build.props` injects, and set `TestingPlatformDotnetTestSupport=false` so `dotnet test` ignores this project. Set `<UseMicrosoftTestingPlatformRunner>false</UseMicrosoftTestingPlatformRunner>`.
- `ProjectReference` to the library.

Benchmark coverage:
- Arities: 2, 4, 8, 16 (covers typical use and worst case)
- Typed (`(Task<int>, Task<int>, ...)`) and non-generic (`(Task, Task, ...)`) tuples
- Two completion modes per arity:
  - **Pre-completed** (`Task.FromResult`) — maximum signal-to-noise for allocation differences; the awaiter's hot path runs synchronously
  - **Async** (single `Task.Yield`) — realistic completion path with state-machine resumption
- `ConfigureAwait(false)` and `ConfigureAwait(ConfigureAwaitOptions.None)` variants under `ConfigureAwaitBenchmarks`
- `[MemoryDiagnoser]` on every benchmark class so per-call allocations are reported

`Program.cs` is the standard `BenchmarkSwitcher.FromAssembly(...).Run(args)` entry point so individual benchmark classes can be filtered from the command line.

CI: benchmarks are not run in CI (BenchmarkDotNet runs are slow and non-deterministic enough to flake CI). They are a manual `dotnet run -c Release -f net10.0` operation. Document this in the project README addendum.

### AOT smoke-test update

`test/TaskTupleAwaiter.AotSmokeTest/TaskTupleAwaiter.AotSmokeTest.csproj` currently has `<TargetFrameworks>net8.0;net11.0</TargetFrameworks>`. Add `net10.0` so the new generator output (collection expression binding to `ReadOnlySpan<Task>` overload) is exercised under NativeAOT.

Result: `<TargetFrameworks>net8.0;net10.0;net11.0</TargetFrameworks>`.

## Testing & verification

1. **Existing tests pass on every existing TFM.** The semantics of the awaiter don't change — only the allocation profile of the `WhenAll` call.
2. **Existing tests pass on the new net10.0 TFM.** `test/Directory.Build.props` already includes `net10.0` in `TargetFrameworks`, so the existing xUnit suite picks it up automatically.
3. **AOT smoke test passes on net10.0.** Publishes successfully and the smoke-test program prints its expected output. Validates the generated code is trim-/AOT-safe under the new code path.
4. **Benchmark report**, captured manually before and after the generator change. The expected outcome is `Allocated` per op drops by `sizeof(Task[N])` (24 + 8·N bytes on 64-bit) for every benchmark on net10.0, and is unchanged on net8.0. Mean time per op should be flat or slightly improved on net10.0 due to less GC pressure; net8.0 should be unchanged.

## Risks

- **Collection-expression IL on the older TFMs.** Source emits `[t1, ..., tN]` but on netstandard2.0/net462/net8.0 this still compiles to `new Task[]{...}` and binds to `params Task[]`. Verify by inspecting generated IL on the netstandard2.0 build; if for any reason the IL differs from today's `new Task[]{...}` shape, treat that as a build break and fall back to feature-detected emission. **Mitigation:** spot-check via `ildasm` / `dotnet-ildasm` on one arity per TFM during implementation.
- **NuGet asset selection.** Verify the resulting `.nupkg` exposes the four library folders correctly and that a sample net10.0 consumer picks `lib/net10.0/`. Done via a smoke restore against a scratch consumer project in the implementation plan.
- **AOT under net10.0.** The collection-expression form lowers to an inline array struct with `[InlineArray]`. The compiler-generated type is AOT-friendly in net8+ (validated by countless BCL APIs), but the smoke test confirms it for our specific code shape.
- **CI SDK availability.** The CI workflow must have the .NET 10 SDK installed. Confirm against the existing `global.json` / workflow file; add a setup step if missing.

## Documentation

- `CLAUDE.md`: update the Technology Stack table to include `net10.0` as a library TFM, and add a short note under "Key Design Decisions" that the generator emits collection-expression `WhenAll` calls so the compiler can pick the span overload on net9+.
- `README.md`: a short bullet under a "Performance" or "Targets" section noting net10.0 consumers get span-based `WhenAll`.
- Benchmark project gets a brief `README.md` describing how to run it locally.

## Open questions

None. The design is bounded and the only judgment call (skip net11 TFM, skip net9 TFM, no `record struct` / inlining tuning yet) is documented above with reasoning.
