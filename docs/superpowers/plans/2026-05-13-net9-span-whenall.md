# net9.0 Span-Based WhenAll + BenchmarkDotNet Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Cut one `Task[]` heap allocation per `await` of a task tuple for .NET 9+ consumers by adding a `net9.0` TFM so that the span-based `Task.WhenAll` overload is used; add a BenchmarkDotNet project to measure the win.

**Architecture:** Add `net9.0` as a fourth library TFM. Overload selection for `Task.WhenAll` is driven by the targeted library TFM: `netstandard2.0`/`net462`/`net8.0` bind to `params Task[]` (unchanged IL), while `net9.0` binds to `ReadOnlySpan<Task>` (stack-allocated buffer, zero heap allocation). New `test/TaskTupleAwaiter.Benchmarks` project measures the delta with `[MemoryDiagnoser]` across arities 2/4/8/16, both pre-completed and async completion modes.

**Tech Stack:** Roslyn `IIncrementalGenerator`, BenchmarkDotNet, xUnit v3, NativeAOT publish for smoke testing, MSBuild multi-TFM, C# 14 collection expressions.

**Spec:** `docs/superpowers/specs/2026-05-13-net9-span-whenall-design.md`

---

## File Structure

**Files created:**

| Path | Purpose |
|---|---|
| `test/TaskTupleAwaiter.Benchmarks/TaskTupleAwaiter.Benchmarks.csproj` | Exe project, net8.0;net9.0, BenchmarkDotNet, refs library |
| `test/TaskTupleAwaiter.Benchmarks/Program.cs` | `BenchmarkSwitcher` entry point |
| `test/TaskTupleAwaiter.Benchmarks/TypedTupleAwaitBenchmarks.cs` | `(Task<int>, Task<int>, ...)` tuple awaits, arities 2/4/8/16, pre-completed + async |
| `test/TaskTupleAwaiter.Benchmarks/NonGenericTupleAwaitBenchmarks.cs` | `(Task, Task, ...)` tuple awaits, same shape |
| `test/TaskTupleAwaiter.Benchmarks/ConfigureAwaitBenchmarks.cs` | `ConfigureAwait(bool)` and `ConfigureAwait(ConfigureAwaitOptions)` variants |
| `test/TaskTupleAwaiter.Benchmarks/README.md` | How to run, what to expect |
| `docs/superpowers/specs/2026-05-13-net9-span-whenall-benchmark-results.md` | Captured baseline + post-change numbers |

**Files modified:**

| Path | Change |
|---|---|
| `src/TaskTupleAwaiter/TaskTupleAwaiter.csproj` | Add `net9.0` to `TargetFrameworks` |
| `src/TaskTupleAwaiter.Generator/TaskTupleExtensionsGenerator.cs` | Change `Items` helper so output is wrapped `[...]` |
| `test/TaskTupleAwaiter.AotSmokeTest/TaskTupleAwaiter.AotSmokeTest.csproj` | Add `net9.0` to `TargetFrameworks` |
| `CLAUDE.md` | Note `net9.0` TFM behavior in design decisions |
| `README.md` | Short perf note for net9+ consumers |

**Files unchanged but referenced for context:**

- `test/Directory.Build.props` — already targets `net11.0;net9.0;net8.0;net472` for test projects; the benchmark project will need to opt out of the inherited xUnit/Shouldly bits like the AOT smoke test does.
- `Directory.Build.props` (root) — sets `LangVersion=latest`, no change needed.

---

## Task 1: Add `net9.0` TFM to the library

**Files:**
- Modify: `src/TaskTupleAwaiter/TaskTupleAwaiter.csproj`

- [ ] **Step 1: Confirm net9 SDK is installed**

Run: `dotnet --list-sdks`
Expected: a line starting with `10.` is present. If not present, install via `winget install Microsoft.DotNet.SDK.9` (or download the .NET 9 SDK) and rerun.

- [ ] **Step 2: Verify baseline build is clean before touching anything**

Run: `dotnet build -c Release`
Expected: build succeeds for all current TFMs (`netstandard2.0`, `net462`, `net8.0`). No warnings escalated to errors.

- [ ] **Step 3: Add `net9.0` to library `TargetFrameworks`**

In `src/TaskTupleAwaiter/TaskTupleAwaiter.csproj`, replace the existing `PropertyGroup` block to declare the TFMs explicitly. Current file has no `<TargetFrameworks>` element because it inherits `<TargetFrameworks>netstandard2.0;net462;net8.0</TargetFrameworks>` from a default — open the file and confirm where TFMs are declared, then add or extend that list to `netstandard2.0;net462;net8.0;net9.0`.

Concretely, the csproj should contain:

```xml
<PropertyGroup>
    <TargetFrameworks>netstandard2.0;net462;net8.0;net9.0</TargetFrameworks>
    <Title>TaskTupleAwaiter</Title>
    <Description>Enable using the new Value Tuple structure to write elegant code that allows async methods to be fired in parallel despite having different return types

var (result1, result2) = await (GetStringAsync(), GetGuidAsync());

Based on the work of Joseph Musser https://github.com/jnm2
    </Description>
</PropertyGroup>
```

If a `TargetFrameworks` element already exists elsewhere (`Directory.Build.props` of `src/`, for example), modify it there instead. Run `grep -r TargetFrameworks src/` to locate.

- [ ] **Step 4: Build all TFMs**

Run: `dotnet build -c Release`
Expected: build succeeds for all four TFMs. Inspect `src/TaskTupleAwaiter/bin/Release/` and confirm four subfolders exist: `netstandard2.0`, `net462`, `net8.0`, `net9.0`, each containing `TaskTupleAwaiter.dll`.

- [ ] **Step 5: Run tests on every test TFM**

Run: `dotnet test -c Release`
Expected: all tests pass on `net472`, `net8.0`, `net9.0`, `net9.0`, `net11.0` (the test project already targets these via `test/Directory.Build.props`).

- [ ] **Step 6: Commit**

```bash
git add src/TaskTupleAwaiter/TaskTupleAwaiter.csproj
git commit -m "Add net9.0 TFM to library"
```

---

## Task 2: Scaffold `TaskTupleAwaiter.Benchmarks` project

**Files:**
- Create: `test/TaskTupleAwaiter.Benchmarks/TaskTupleAwaiter.Benchmarks.csproj`

- [ ] **Step 1: Create directory**

Run: `mkdir test/TaskTupleAwaiter.Benchmarks`
Expected: directory exists, empty.

- [ ] **Step 2: Create `TaskTupleAwaiter.Benchmarks.csproj`**

Create `test/TaskTupleAwaiter.Benchmarks/TaskTupleAwaiter.Benchmarks.csproj` with:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <IsPackable>false</IsPackable>
    <OutputType>Exe</OutputType>
    <TargetFrameworks>net8.0;net9.0</TargetFrameworks>
    <TestingPlatformDotnetTestSupport>false</TestingPlatformDotnetTestSupport>
    <UseMicrosoftTestingPlatformRunner>false</UseMicrosoftTestingPlatformRunner>
    <ServerGarbageCollection>true</ServerGarbageCollection>
  </PropertyGroup>

  <!--
    test/Directory.Build.props injects Shouldly + xUnit ItemGroups for the
    test project. This benchmarks app is a plain console app and must not
    pull those in. Remove both inherited items and the testing platform
    packages they implicitly bring along.
  -->
  <ItemGroup>
    <Using Remove="Shouldly" />
    <Using Remove="Xunit" />
    <PackageReference Remove="Shouldly" />
    <PackageReference Remove="xunit.v3.mtp-v2" />
    <PackageReference Include="BenchmarkDotNet" Version="0.14.*" />
    <ProjectReference Include="..\..\src\TaskTupleAwaiter\TaskTupleAwaiter.csproj" />
  </ItemGroup>

</Project>
```

(Adjust `BenchmarkDotNet` version to the latest stable at implementation time. `0.14.*` is the current major as of 2026-05.)

- [ ] **Step 3: Add to solution**

Run: `dotnet sln TaskTupleAwaiter.slnx add test/TaskTupleAwaiter.Benchmarks/TaskTupleAwaiter.Benchmarks.csproj`
Expected: solution updated. If the `.slnx` format doesn't support `dotnet sln add` in the installed SDK, open `TaskTupleAwaiter.slnx` and add the project entry manually following the format of the existing entries.

- [ ] **Step 4: Restore to surface dependency issues early**

Run: `dotnet restore test/TaskTupleAwaiter.Benchmarks/TaskTupleAwaiter.Benchmarks.csproj`
Expected: restore succeeds.

- [ ] **Step 5: Commit**

```bash
git add test/TaskTupleAwaiter.Benchmarks/TaskTupleAwaiter.Benchmarks.csproj TaskTupleAwaiter.slnx
git commit -m "Scaffold TaskTupleAwaiter.Benchmarks project"
```

---

## Task 3: Add benchmark `Program.cs` entry point

**Files:**
- Create: `test/TaskTupleAwaiter.Benchmarks/Program.cs`

- [ ] **Step 1: Write `Program.cs`**

Create `test/TaskTupleAwaiter.Benchmarks/Program.cs` with:

```csharp
using BenchmarkDotNet.Running;

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
```

This top-level statement form makes `Program` an implicit class so `typeof(Program).Assembly` works.

- [ ] **Step 2: Build benchmarks project (smoke)**

Run: `dotnet build test/TaskTupleAwaiter.Benchmarks -c Release`
Expected: build succeeds for `net8.0` and `net9.0`. No warnings.

- [ ] **Step 3: Run the benchmarks runner with `--help`**

Run: `dotnet run -c Release --project test/TaskTupleAwaiter.Benchmarks -f net9.0 -- --help`
Expected: BenchmarkDotNet help text printed. No exception. (No benchmark classes yet so `--list` would show zero items — `--help` is enough to confirm the entry point compiles and BDN initializes.)

- [ ] **Step 4: Commit**

```bash
git add test/TaskTupleAwaiter.Benchmarks/Program.cs
git commit -m "Add BenchmarkSwitcher entry point"
```

---

## Task 4: Add `TypedTupleAwaitBenchmarks`

**Files:**
- Create: `test/TaskTupleAwaiter.Benchmarks/TypedTupleAwaitBenchmarks.cs`

- [ ] **Step 1: Write the benchmark class**

Create `test/TaskTupleAwaiter.Benchmarks/TypedTupleAwaitBenchmarks.cs` with:

```csharp
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
```

Note: tabs for indentation per repo convention.

- [ ] **Step 2: Build to verify all four library TFMs of generated extensions resolve**

Run: `dotnet build test/TaskTupleAwaiter.Benchmarks -c Release`
Expected: build succeeds for `net8.0` and `net9.0`. The benchmark uses `await (Task<int>, Task<int>, ...)` syntax which exercises the library's generated `GetAwaiter` extension methods.

- [ ] **Step 3: Smoke-run a single benchmark**

Run: `dotnet run -c Release --project test/TaskTupleAwaiter.Benchmarks -f net9.0 -- --filter "*Arity2_PreCompleted*"`
Expected: BenchmarkDotNet runs and reports `Mean`, `Allocated`, etc. for `Arity2_PreCompleted` on net9. Takes 30-60 seconds. No crashes.

- [ ] **Step 4: Commit**

```bash
git add test/TaskTupleAwaiter.Benchmarks/TypedTupleAwaitBenchmarks.cs
git commit -m "Add typed-tuple await benchmarks"
```

---

## Task 5: Add `NonGenericTupleAwaitBenchmarks`

**Files:**
- Create: `test/TaskTupleAwaiter.Benchmarks/NonGenericTupleAwaitBenchmarks.cs`

- [ ] **Step 1: Write the benchmark class**

Create `test/TaskTupleAwaiter.Benchmarks/NonGenericTupleAwaitBenchmarks.cs` with:

```csharp
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
```

The leading `(Task)` cast on the first tuple element forces the tuple to be `(Task, Task, ...)` rather than `(Task, Task, ...)` being inferred as a homogeneous structure that may bind to a typed extension method. (`Task.CompletedTask` returns `Task`, so the cast is redundant for type but explicit for the reader.)

- [ ] **Step 2: Build**

Run: `dotnet build test/TaskTupleAwaiter.Benchmarks -c Release`
Expected: build succeeds.

- [ ] **Step 3: Smoke-run a single benchmark**

Run: `dotnet run -c Release --project test/TaskTupleAwaiter.Benchmarks -f net9.0 -- --filter "*NonGenericTupleAwaitBenchmarks.Arity2_PreCompleted*"`
Expected: BDN reports results for `Arity2_PreCompleted` on net9. No crashes.

- [ ] **Step 4: Commit**

```bash
git add test/TaskTupleAwaiter.Benchmarks/NonGenericTupleAwaitBenchmarks.cs
git commit -m "Add non-generic-tuple await benchmarks"
```

---

## Task 6: Add `ConfigureAwaitBenchmarks`

**Files:**
- Create: `test/TaskTupleAwaiter.Benchmarks/ConfigureAwaitBenchmarks.cs`

- [ ] **Step 1: Write the benchmark class**

Create `test/TaskTupleAwaiter.Benchmarks/ConfigureAwaitBenchmarks.cs` with:

```csharp
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
```

`ConfigureAwaitOptions` exists on net8.0+, so this compiles cleanly under both benchmark TFMs.

- [ ] **Step 2: Build**

Run: `dotnet build test/TaskTupleAwaiter.Benchmarks -c Release`
Expected: build succeeds.

- [ ] **Step 3: Smoke-run a single benchmark**

Run: `dotnet run -c Release --project test/TaskTupleAwaiter.Benchmarks -f net9.0 -- --filter "*ConfigureAwaitBenchmarks.Typed_Arity4_Bool_False*"`
Expected: BDN reports results. No crashes.

- [ ] **Step 4: Commit**

```bash
git add test/TaskTupleAwaiter.Benchmarks/ConfigureAwaitBenchmarks.cs
git commit -m "Add ConfigureAwait benchmarks"
```

---

## Task 7: Add benchmark `README.md`

**Files:**
- Create: `test/TaskTupleAwaiter.Benchmarks/README.md`

- [ ] **Step 1: Write the README**

Create `test/TaskTupleAwaiter.Benchmarks/README.md` with:

````markdown
# TaskTupleAwaiter.Benchmarks

BenchmarkDotNet harness measuring the allocation and time profile of awaiting `ValueTuple` of `Task` / `Task<T>` across:

- Arities 2, 4, 8, 16
- Typed (`Task<T>`) and non-generic (`Task`) tuples
- Pre-completed (`Task.FromResult`) and async (`Task.Yield`) completion modes
- `ConfigureAwait(bool)` and `ConfigureAwait(ConfigureAwaitOptions)` paths

The point of these benchmarks is to compare allocation profiles between `net8.0` (where the generated `Task.WhenAll(...)` call binds to `params Task[]` and heap-allocates the array) and `net9.0` (where it binds to `Task.WhenAll(ReadOnlySpan<Task>)` and stack-allocates).

## Running

Run all benchmarks on net9.0:

```sh
dotnet run -c Release --project test/TaskTupleAwaiter.Benchmarks -f net9.0
```

Run all benchmarks on net8.0:

```sh
dotnet run -c Release --project test/TaskTupleAwaiter.Benchmarks -f net8.0
```

Filter to one class:

```sh
dotnet run -c Release --project test/TaskTupleAwaiter.Benchmarks -f net9.0 -- --filter "*TypedTupleAwaitBenchmarks*"
```

## Expected outcome

With the `net9.0` library target in place:

- **net8.0:** allocations and timing unchanged from baseline. Same IL as today.
- **net9.0:** `Allocated` per op drops by approximately `24 + 8·N` bytes (the `Task[N]` array we no longer allocate). Mean time per op is flat or slightly improved due to reduced GC pressure.

## Not run in CI

Benchmark runs are slow (multi-minute) and have enough run-to-run variance that they are unsuitable for CI gating. They are a manual local check before tagging a release or when validating perf-sensitive changes.
````

- [ ] **Step 2: Commit**

```bash
git add test/TaskTupleAwaiter.Benchmarks/README.md
git commit -m "Document how to run the benchmarks"
```

---

## Task 8: Capture baseline benchmark numbers

**Files:**
- Create: `docs/superpowers/specs/2026-05-13-net9-span-whenall-benchmark-results.md`

- [ ] **Step 1: Run full benchmark suite on net8.0**

Run: `dotnet run -c Release --project test/TaskTupleAwaiter.Benchmarks -f net8.0 --no-build`
(Run `dotnet build -c Release` first if you skipped the smoke runs above.)
Expected: full suite completes. Takes several minutes. Per-method `Mean` and `Allocated` columns appear in the summary table.

- [ ] **Step 2: Run full benchmark suite on net9.0**

Run: `dotnet run -c Release --project test/TaskTupleAwaiter.Benchmarks -f net9.0 --no-build`
Expected: full suite completes. Per-method `Mean` and `Allocated` columns appear.

- [ ] **Step 3: Capture summary tables**

BenchmarkDotNet writes Markdown reports to `BenchmarkDotNet.Artifacts/results/*.md` in the project directory. Locate the most recent `*-report-github.md` files (one per benchmark class per TFM).

Create `docs/superpowers/specs/2026-05-13-net9-span-whenall-benchmark-results.md` with this structure:

```markdown
# Benchmark Results

Captured on [date], machine [name], BenchmarkDotNet [version], .NET SDK [version].

## Baseline (before generator change)

Generator emits `Task.WhenAll(tasks.Item1, ..., tasks.ItemN)`.

### net8.0

[paste TypedTupleAwaitBenchmarks-report-github.md table here]

[paste NonGenericTupleAwaitBenchmarks-report-github.md table here]

[paste ConfigureAwaitBenchmarks-report-github.md table here]

### net9.0

[paste the same three tables for net9.0 here]

## After generator change

[Filled in by Task 10.]
```

Fill in the bracketed placeholders with the actual report contents. Keep the per-arity, per-TFM tables exactly as BDN renders them.

- [ ] **Step 4: Commit**

```bash
git add docs/superpowers/specs/2026-05-13-net9-span-whenall-benchmark-results.md
git commit -m "Capture baseline benchmark numbers"
```

---

## Task 9: Update generator to emit bracket-form syntax (stylistic)

**Files:**
- Modify: `src/TaskTupleAwaiter.Generator/TaskTupleExtensionsGenerator.cs` — line ~308, the `Items` helper

- [ ] **Step 1: Confirm baseline tests pass**

Run: `dotnet test -c Release`
Expected: all tests pass on every test TFM. This locks in the "before" behavior so any regression introduced by the generator change is detected.

- [ ] **Step 2: Modify the `Items` helper**

In `src/TaskTupleAwaiter.Generator/TaskTupleExtensionsGenerator.cs`, locate the existing helper:

```csharp
static string Items(int arity) =>
	string.Join(", ", Enumerable.Range(1, arity).Select(i => $"tasks.Item{i}"));
```

Replace with:

```csharp
static string Items(int arity) =>
	$"[{string.Join(", ", Enumerable.Range(1, arity).Select(i => $"tasks.Item{i}"))}]";
```

Effect: every generated `Task.WhenAll({Items(arity)})` call site (5 locations in this file, all already inspecting `Task.WhenAll(...)`) emits `Task.WhenAll([tasks.Item1, ..., tasks.ItemN])`. Overload selection comes from target framework availability and compiler preference: TFMs without the span overload bind to `params Task[]` (identical IL to today), while `net9.0` binds to `ReadOnlySpan<Task>` (stack-allocated buffer).

- [ ] **Step 3: Build the library**

Run: `dotnet build -c Release src/TaskTupleAwaiter`
Expected: build succeeds for all four TFMs. No warnings.

- [ ] **Step 4: Inspect generated source**

Open `src/TaskTupleAwaiter/obj/Release/net9.0/generated/TaskTupleAwaiter.Generator/TaskTupleAwaiter.Generator.TaskTupleExtensionsGenerator/TaskTupleExtensions.g.cs` and confirm at least one `WhenAll` call uses brackets, e.g.:

```csharp
_whenAllAwaiter = Task.WhenAll([tasks.Item1, tasks.Item2]).GetAwaiter();
```

(The exact path may vary slightly depending on the analyzer's `RootNamespace`; search for `TaskTupleExtensions.g.cs` under `obj/` if unsure.)

- [ ] **Step 5: Run tests on every test TFM**

Run: `dotnet test -c Release`
Expected: all tests pass on every test TFM. Semantics are unchanged — only the IL shape of the `WhenAll` call differs per target.

- [ ] **Step 6: Commit**

```bash
git add src/TaskTupleAwaiter.Generator/TaskTupleExtensionsGenerator.cs
git commit -m "Emit bracket-form WhenAll calls in generated extensions"
```

---

## Task 10: Re-run benchmarks and record post-change numbers

**Files:**
- Modify: `docs/superpowers/specs/2026-05-13-net9-span-whenall-benchmark-results.md`

- [ ] **Step 1: Re-run net8.0 suite**

Run: `dotnet run -c Release --project test/TaskTupleAwaiter.Benchmarks -f net8.0`
Expected: completes. `Allocated` columns should be **identical** to baseline (within run-to-run noise). If they differ materially, the source change has affected the older TFM and Task 9's change needs revisiting.

- [ ] **Step 2: Re-run net9.0 suite**

Run: `dotnet run -c Release --project test/TaskTupleAwaiter.Benchmarks -f net9.0`
Expected: completes. `Allocated` columns should be **lower** than baseline net9.0 for every benchmark, by approximately `24 + 8·N` bytes (typical `Task[N]` array overhead on 64-bit) per op for arity-N benchmarks.

- [ ] **Step 3: Update the results doc**

Open `docs/superpowers/specs/2026-05-13-net9-span-whenall-benchmark-results.md` and fill in the "After generator change" section with the new tables, mirroring the baseline structure.

Add a "Delta summary" subsection at the bottom showing per-arity allocation reduction on net9, e.g.:

```markdown
## Delta summary (net9.0 only)

| Benchmark | Baseline allocated | After allocated | Reduction |
|---|---|---|---|
| TypedTuple Arity2_PreCompleted | XXX B | YYY B | ZZZ B |
| ... | | | |
```

Fill the cells with actual numbers from the reports.

- [ ] **Step 4: Commit**

```bash
git add docs/superpowers/specs/2026-05-13-net9-span-whenall-benchmark-results.md
git commit -m "Record post-change benchmark numbers"
```

---

## Task 11: Add `net9.0` to AOT smoke test

**Files:**
- Modify: `test/TaskTupleAwaiter.AotSmokeTest/TaskTupleAwaiter.AotSmokeTest.csproj`

- [ ] **Step 1: Modify TFMs**

In `test/TaskTupleAwaiter.AotSmokeTest/TaskTupleAwaiter.AotSmokeTest.csproj`, change:

```xml
<TargetFrameworks>net8.0;net11.0</TargetFrameworks>
```

to:

```xml
<TargetFrameworks>net8.0;net9.0;net11.0</TargetFrameworks>
```

- [ ] **Step 2: Publish for net9.0**

Run: `dotnet publish test/TaskTupleAwaiter.AotSmokeTest -c Release -f net9.0`
Expected: publish succeeds. The published exe is in `test/TaskTupleAwaiter.AotSmokeTest/bin/Release/net9.0/<rid>/publish/`. AOT analyzer emits no errors or warnings.

- [ ] **Step 3: Run the published exe**

Run: locate the published exe (the project's `<rid>` is host-specific; on Windows x64 it'd be `win-x64`) and run it.

For Windows x64: `test\TaskTupleAwaiter.AotSmokeTest\bin\Release\net9.0\win-x64\publish\TaskTupleAwaiter.AotSmokeTest.exe`

Expected: program prints the expected smoke output (matches the existing net8.0 / net11.0 runs) and exits 0.

- [ ] **Step 4: Confirm net8.0 and net11.0 still publish cleanly**

Run: `dotnet publish test/TaskTupleAwaiter.AotSmokeTest -c Release`
Expected: publishes for all three TFMs without error.

- [ ] **Step 5: Commit**

```bash
git add test/TaskTupleAwaiter.AotSmokeTest/TaskTupleAwaiter.AotSmokeTest.csproj
git commit -m "Add net9.0 to AOT smoke test TFMs"
```

---

## Task 12: Update docs

**Files:**
- Modify: `CLAUDE.md`
- Modify: `README.md`

- [ ] **Step 1: Update `CLAUDE.md` Technology Stack table**

In `CLAUDE.md`, find the Technology Stack table. Change the "Library TFMs" row from:

```
| Library TFMs | netstandard2.0, net462, net8.0 |
```

to:

```
| Library TFMs | netstandard2.0, net462, net8.0, net9.0 |
```

- [ ] **Step 2: Update `CLAUDE.md` Key Design Decisions section**

Under the "Source Generator (`TaskTupleExtensionsGenerator`)" subsection, add a new bullet after the existing `Feature-detects ConfigureAwaitOptions...` bullet:

```markdown
- Emits `Task.WhenAll([tasks.Item1, ..., tasks.ItemN])`. Overload selection comes from the targeted TFM: `params Task[]` on `netstandard2.0`/`net462`/`net8.0` (same IL as before) and `Task.WhenAll(ReadOnlySpan<Task>)` on `net9.0`+, eliminating the per-await `Task[]` heap allocation. No runtime feature detection needed.
```

- [ ] **Step 3: Update `README.md`**

In `README.md`, find a suitable place (after the basic usage section, or under a "Targets" / "Performance" heading if one exists; otherwise add a short new section).

Add:

```markdown
## Target frameworks

- `netstandard2.0` — broadest compatibility
- `net462` — .NET Framework consumers
- `net8.0` — LTS
- `net9.0` — STS; uses `Task.WhenAll(ReadOnlySpan<Task>)` to eliminate the per-await `Task[]` heap allocation
```

(Adjust the wording to fit the existing README tone — keep the bullet about net9's allocation win.)

- [ ] **Step 4: Verify both docs render correctly**

Run: `dotnet build -c Release` (sanity: ensure no markdown changes broke build via some hook).
Open `CLAUDE.md` and `README.md` and confirm formatting looks right.

- [ ] **Step 5: Commit**

```bash
git add CLAUDE.md README.md
git commit -m "Document net9.0 TFM and span WhenAll emission"
```

---

## Task 13: Final verification

- [ ] **Step 1: Clean build from scratch**

Run: `dotnet clean && dotnet build -c Release`
Expected: full clean build succeeds. All four library TFMs and all test/aot/benchmark TFMs build without error.

- [ ] **Step 2: Full test pass**

Run: `dotnet test -c Release`
Expected: all tests pass on every test TFM.

- [ ] **Step 3: AOT publish for every smoke-test TFM**

Run: `dotnet publish test/TaskTupleAwaiter.AotSmokeTest -c Release`
Expected: publishes for `net8.0`, `net9.0`, `net11.0`. No AOT warnings.

- [ ] **Step 4: Confirm package layout**

Run: `dotnet pack src/TaskTupleAwaiter -c Release`
Expected: produces `TaskTupleAwaiter.<version>.nupkg` containing `lib/netstandard2.0/`, `lib/net462/`, `lib/net8.0/`, `lib/net9.0/`. Inspect with `dotnet tool run dotnet-pack-check` or open the `.nupkg` as a zip and verify the four lib folders.

- [ ] **Step 5: Confirm benchmark results doc is complete**

Open `docs/superpowers/specs/2026-05-13-net9-span-whenall-benchmark-results.md` and verify:
- Both "Baseline" and "After generator change" sections are populated.
- "Delta summary" table exists with non-empty rows.
- net8.0 delta values are ≈ 0 (within noise).
- net9.0 delta values show consistent allocation reduction.

If any of those is missing, return to Task 8 or Task 10 to capture the missing data.

- [ ] **Step 6: Review final commit history**

Run: `git log --oneline origin/master..HEAD`
Expected: a clean linear sequence of ~12 commits, one per task, each with a descriptive message.

- [ ] **Step 7: Plan complete — ready to push or open PR**

No automatic push. Hand back to the user with the commit list and offer to open a PR.

---

## Notes for the implementer

- **Tab indentation everywhere** per repo convention. The C# code blocks in this plan use tabs.
- **No emojis** in any file modified or created.
- **Don't `--no-verify` past failing hooks.** If a pre-commit hook fails, diagnose and fix rather than bypass.
- **Generator output location** depends on the project's `obj/` structure and the analyzer's namespace. If you can't find `TaskTupleExtensions.g.cs` in the path given, run `dotnet build` with `-bl` to produce a binlog and inspect, or `find obj -name TaskTupleExtensions.g.cs`.
- **BenchmarkDotNet artifacts** land in `BenchmarkDotNet.Artifacts/` adjacent to the project directory. Don't commit them — add to `.gitignore` if not already excluded (the repo's existing `.gitignore` likely covers `bin/` and `obj/` but not `BenchmarkDotNet.Artifacts/`; add an entry if missing).
- **If a `dotnet --list-sdks` doesn't show .NET 9** at Task 1, install it via your platform's installer or `winget install Microsoft.DotNet.SDK.9` before continuing. Don't try to suppress the missing-TFM error.
- **If the collection expression fails to compile** on netstandard2.0/net462 at Task 9 (e.g., because of a `LangVersion` quirk), check `Directory.Build.props` — it sets `LangVersion=latest` which should handle C# 14 collection expressions on all TFMs. If not, the fallback is to feature-detect at the generator level (the spec's "Risks" section calls this out as the contingency path).
