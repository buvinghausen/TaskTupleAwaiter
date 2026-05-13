# CLAUDE.md — TaskTupleAwaiter

## Project Overview

TaskTupleAwaiter provides extension methods that allow you to `await` a `ValueTuple` of `Task<T>` (or non-generic `Task`) instances and destructure the results in a single line. In this repository, a Roslyn incremental source generator (`src/TaskTupleAwaiter.Generator`) generates the extension-method source during library build under `namespace System.Threading.Tasks`, and that generated code is compiled into `TaskTupleAwaiter.dll` for each target framework. Consumers install and reference the compiled package binaries; the generator is a private build-time implementation detail.

## Repository Layout

```
TaskTupleAwaiter/
├── src/
│   ├── TaskTupleAwaiter/                  # Main library shell (netstandard2.0, net462, net8.0, net10.0)
│   │                                      #   No hand-authored .cs sources — code is generated at build and compiled into the library.
│   └── TaskTupleAwaiter.Generator/        # Roslyn incremental source generator (netstandard2.0)
│       └── TaskTupleExtensionsGenerator.cs
├── test/
│   ├── TaskTupleAwaiter.Tests/            # xUnit v3 test project
│   │   ├── TaskTupleAwaiterTests.cs
│   │   ├── BehaviorComparisonTests.cs
│   │   ├── Adapters/
│   │   │   └── AwaiterAdapter.cs
│   │   ├── DummyException.cs
│   │   ├── On.cs
│   │   └── SpySynchronizationContext.cs
│   ├── TaskTupleAwaiter.AotSmokeTest/     # NativeAOT downstream-consumer smoke-test (net8.0, net10.0, net11.0)
│   │   ├── TaskTupleAwaiter.AotSmokeTest.csproj
│   │   └── Program.cs
│   └── TaskTupleAwaiter.Benchmarks/        # BenchmarkDotNet harness (net8.0, net10.0)
│       ├── TaskTupleAwaiter.Benchmarks.csproj  # xUnit/Shouldly inheritance from test/Directory.Build.props is bypassed via the MSBuildProjectName condition there, not via a local Directory.Build.props.
│       ├── Program.cs                     #   BenchmarkSwitcher entry point.
│       ├── TypedTupleAwaitBenchmarks.cs
│       ├── NonGenericTupleAwaitBenchmarks.cs
│       ├── ConfigureAwaitBenchmarks.cs
│       └── README.md                      #   How to run; the runs are local-only, not CI.
├── docs/superpowers/                      # Specs and implementation plans
├── README.md
├── LICENSE.txt
└── CLAUDE.md                              # This file
```

## Technology Stack

| Concern | Choice |
|---|---|
| Language | C# 14.0 |
| Library TFMs | netstandard2.0, net462, net8.0, net10.0 |
| Generator target | netstandard2.0 (Roslyn analyzer requirement) |
| AOT-compatible TFMs | net8.0+ (`<IsAotCompatible>true</IsAotCompatible>` via `IsTargetFrameworkCompatible`) |
| Generator framework | Roslyn `IIncrementalGenerator` |
| Test framework | xUnit v3 |
| Assertion library | Shouldly |
| Max tuple arity | 16 |

## Key Design Decisions

### Source Generator (`TaskTupleExtensionsGenerator`)
- Implements `IIncrementalGenerator` (not the older `ISourceGenerator`).
- **Feature-detects** `ConfigureAwaitOptions` at compile time by resolving the type `System.Threading.Tasks.ConfigureAwaitOptions` from the target compilation — **do not use** `#if NET8_0_OR_GREATER` or preprocessor symbols.
- Emits `Task.WhenAll([tasks.Item1, ..., tasks.ItemN])` as a **collection expression**. On `netstandard2.0` / `net462` / `net8.0` the compiler binds to `Task.WhenAll(params Task[])` (heap-allocated array — same IL as before this approach). On `net10.0`+ the compiler prefers `Task.WhenAll(ReadOnlySpan<Task>)` and stack-allocates the buffer, eliminating the per-await `Task[]` heap allocation. No runtime feature detection needed for this — overload preference is purely a compiler/TFM concern.
- Emits a single file `TaskTupleExtensions.g.cs` into the `System.Threading.Tasks` namespace (suppressing `IDE0130`).
- Arity-1 typed tuples (`ValueTuple<Task<T1>>`) delegate directly to the inner task's awaiter — no custom awaiter struct is generated.
- Arities 2–16 emit `TupleTaskAwaiter<T1,...,TN>` and `TupleConfiguredTaskAwaitable<T1,...,TN>` `readonly record struct` types per arity.
- Non-generic `Task` tuples (arity 1–16) are emitted in a separate `#region Task` section; they return `TaskAwaiter` / `ConfiguredTaskAwaitable` directly via `Task.WhenAll(...)`.

### Awaiter Pattern
- All custom awaiter structs implement `ICriticalNotifyCompletion`.
- `UnsafeOnCompleted` is annotated `[SecurityCritical]`.
- `GetResult()` calls `_whenAllAwaiter.GetResult()` first (to propagate exceptions), then returns a value tuple of `.Result` on each individual task.

## Build & Test

```sh
# Restore, build, and run all tests
dotnet build
dotnet test

# Run only a specific test class
dotnet test --filter "FullyQualifiedName~TaskTupleAwaiterTests"
```

## Coding Conventions

- Use **tabs** for indentation (consistent with existing source files).
- All generated code must begin with `// <auto-generated/>`.
- Helper string methods (`TypeParams`, `Items`, `Results`, etc.) live as `static` methods at the bottom of the generator class under the `// ── String helpers ───` region comment.
- Keep region comments (`#region (Task<T1>..Task<TN>)`) in the generated output for readability.
- Prefer `static` lambdas (captures disallowed) inside `IncrementalGeneratorInitializationContext` pipeline calls.
- Arity constant `MaxArity = 16` is defined once at the top of the generator class — change it there only.

## Adding a New `ConfigureAwait` Overload

1. Update `AppendTypedArity1`, `AppendTypedArity`, and `AppendNonGenericSection` / `AppendNonGenericArity` in `TaskTupleExtensionsGenerator.cs`.
2. If the overload is conditional on a runtime feature, add a new feature-detection provider in `Initialize` (follow the `hasAwaitOptionsProvider` pattern).
3. Add corresponding tests in `TaskTupleAwaiter.Tests` covering all arities (use the `EachArity` / `EachIndexForEachArity` `TheoryData` helpers from `BehaviorComparisonTests`).

## License

MIT © Brian Buvinghausen. Original concept by Joseph Musser (@jnm2).
