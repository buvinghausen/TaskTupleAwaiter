# CLAUDE.md — TaskTupleAwaiter

## Project Overview

TaskTupleAwaiter provides extension methods that allow you to `await` a `ValueTuple` of `Task<T>` (or non-generic `Task`) instances and destructure the results in a single line. In this repository, a Roslyn incremental source generator (`src/TaskTupleAwaiter.Generator`) generates the extension-method source during library build under `namespace System.Threading.Tasks`, and that generated code is compiled into `TaskTupleAwaiter.dll` for each target framework. Consumers install and reference the compiled package binaries; the generator is a private build-time implementation detail.

## Development Workflow — Use Superpowers

This repo has the `superpowers` plugin enabled (`.claude/settings.json`). It's not decorative —
use the skill tree for real work here, and it maps directly onto the spec-first, plan-second,
code-last workflow:

| Phase | Skill |
|---|---|
| Design/spec iteration | `brainstorming` |
| Plan once the spec settles | `writing-plans` |
| Implementation | `executing-plans` (pairs with `test-driven-development`) |
| Bug fixes (spec-first exception) | `systematic-debugging` |
| Before calling anything done | `verification-before-completion` |
| Opening/handling a PR | `requesting-code-review` / `receiving-code-review` |
| Wrapping up a branch | `finishing-a-development-branch` |

If a skill applies to what you're doing, invoke it — don't just read the table and proceed
manually. The transition points (spec → plan, plan → code) are still explicit human decisions,
per the global CLAUDE.md; the skills are how each phase gets executed, not a way around the
hand-off.

## Repository Layout

```
TaskTupleAwaiter/
├── src/
│   ├── TaskTupleAwaiter/                  # Main library shell (net10.0, net462, netstandard2.0)
│   │                                      #   No hand-authored .cs sources — code is generated at build and compiled into the library.
│   └── TaskTupleAwaiter.Generator/        # Roslyn incremental source generator (netstandard2.0)
│       └── TaskTupleExtensionsGenerator.cs
├── tests/
│   ├── unit/
│   │   └── TaskTupleAwaiter.Tests/        # xUnit v3 test project (net11.0, net10.0, net472; runtime-async=on for net11.0)
│   │       ├── TaskTupleAwaiterTests.cs
│   │       ├── BehaviorComparisonTests.cs
│   │       ├── Adapters/                  #   AwaiterAdapter partial classes
│   │       ├── DummyException.cs
│   │       ├── On.cs
│   │       ├── CopyableSynchronizationContext.cs
│   │       └── SpySynchronizationContext.cs
│   └── smoke/
│       └── TaskTupleAwaiter.AotSmokeTest/ # NativeAOT downstream-consumer smoke-test (net11.0, runtime-async=on)
│           ├── TaskTupleAwaiter.AotSmokeTest.csproj
│           └── Program.cs
├── benches/
│   └── TaskTupleAwaiter.Benchmarks/       # BenchmarkDotNet harness (net11.0)
│       ├── TaskTupleAwaiter.Benchmarks.csproj  # Sibling of tests/, so it doesn't inherit xUnit/Shouldly from tests/unit/Directory.Build.props.
│       ├── Program.cs                     #   BenchmarkSwitcher entry point.
│       ├── TypedTupleAwaitBenchmarks.cs
│       ├── NonGenericTupleAwaitBenchmarks.cs
│       ├── ConfigureAwaitBenchmarks.cs
│       └── README.md                      #   How to run; the runs are local-only, not CI.
├── docs/superpowers/                      # Specs and implementation plans
├── README.md
├── LICENSE.txt
├── test.sh                                # Linux/WSL2 test runner — see Build & Test below
└── CLAUDE.md                              # This file
```

## Technology Stack

| Concern | Choice |
|---|---|
| Language | C# 14.0 |
| Library TFMs | net10.0, net462, netstandard2.0 — deliberately **no** `net11.0` (see below) |
| Generator target | netstandard2.0 (Roslyn analyzer requirement) |
| AOT-compatible TFMs | net10.0 (`<IsAotCompatible>true</IsAotCompatible>` via `IsTargetFrameworkCompatible`) |
| Generator framework | Roslyn `IIncrementalGenerator` |
| Test framework | xUnit v3 |
| Assertion library | Shouldly |
| Max tuple arity | 16 |

## Key Design Decisions

### Target Frameworks
- The library ships exactly one modern TFM, `net10.0`, plus `net462` and `netstandard2.0`. **Do not add `net11.0`** (or later) — the maintainers want the package download kept minimal, and `net11.0` consumers resolve the `net10.0` asset, which already has the span `WhenAll` and `ConfigureAwaitOptions` overloads. Bump the single modern TFM only when its runtime goes out of support.
- Tests still run on `net11.0`, `net10.0`, and `net472`, so the `net10.0` asset is verified on the newest runtime too.
- `global.json` pins the .NET 11 RTM SDK (`11.0.100`, `rollForward: latestFeature`, `allowPrerelease: false`); CI, release, and CodeQL all read the SDK version from it.

### Source Generator (`TaskTupleExtensionsGenerator`)
- Implements `IIncrementalGenerator` (not the older `ISourceGenerator`).
- **Feature-detects** `ConfigureAwaitOptions` at compile time by resolving the type `System.Threading.Tasks.ConfigureAwaitOptions` from the target compilation — **do not use** `#if NET8_0_OR_GREATER` or preprocessor symbols.
- Emits `Task.WhenAll([tasks.Item1, ..., tasks.ItemN])`. Overload binding is determined by the library TFM: `netstandard2.0` / `net462` bind to `Task.WhenAll(params Task[])` (heap-allocated array), while `net10.0` (any `net9.0`+ TFM) binds to `Task.WhenAll(ReadOnlySpan<Task>)` and stack-allocates the buffer. No runtime feature detection needed for this — overload preference is purely a compiler/TFM concern.
- Emits a single file `TaskTupleExtensions.g.cs` into the `System.Threading.Tasks` namespace (suppressing `IDE0130`).
- Arity-1 typed tuples (`ValueTuple<Task<T1>>`) delegate directly to the inner task's awaiter — no custom awaiter struct is generated.
- Arities 2–16 emit `TupleTaskAwaiter<T1,...,TN>` and `TupleConfiguredTaskAwaitable<T1,...,TN>` `readonly struct` types per arity.
- Non-generic `Task` tuples (arity 1–16) are emitted in a separate `#region Task` section; they return `TaskAwaiter` / `ConfiguredTaskAwaitable` directly via `Task.WhenAll(...)`.

### Awaiter Pattern
- All custom awaiter structs implement `ICriticalNotifyCompletion`.
- `UnsafeOnCompleted` is annotated `[SecurityCritical]`.
- `GetResult()` calls `_whenAllAwaiter.GetResult()` first (to propagate exceptions), then returns a value tuple of `.Result` on each individual task.

## Build & Test

```sh
dotnet build
```

**On Windows:** bare `dotnet test` works fine and runs the full matrix, including the net472 leg.

**On Linux/WSL2:** bare `dotnet test` (and even `dotnet test -f net472`) fails immediately with
`Unhandled exception: ... Ensure you have a runnable project type ... A runnable project should
target a runnable TFM` the instant it hits the net472 leg — MTP's orchestrator enumerates every
TFM up front, and net472 isn't launchable through the `dotnet` muxer on Linux. Use the repo's
`test.sh` instead:

```sh
./test.sh
```

It runs the modern TFMs one at a time via `dotnet test -f <tfm>` (net11.0/net10.0),
then builds `TaskTupleAwaiter.Tests` for net472 and runs the resulting `.exe` directly under
**Mono** (`sudo dnf install -y mono-complete` or equivalent — see `TOOLCHAIN.md` in the
`buvinghausen` repo). Verified against the real MTP test host, not a build-only stand-in.
`set -euo pipefail` — any failure stops the script with a non-zero exit.

**Coverage:** the test project references `Microsoft.Testing.Extensions.CodeCoverage` (via
`tests/unit/Directory.Build.props`). CI runs `dotnet test --coverage --coverage-output-format
cobertura`, merges the per-host files with ReportGenerator, posts a sticky PR comment, and fails
below `MINIMUM_BRANCH_COVERAGE` in `.github/workflows/ci.yml`.

To run only a specific test class on a single TFM:

```sh
dotnet test -f net10.0 --filter "FullyQualifiedName~TaskTupleAwaiterTests"
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
