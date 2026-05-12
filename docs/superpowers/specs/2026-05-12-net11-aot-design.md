# TaskTupleAwaiter — .NET 11 compatibility + AOT support

**Status:** Approved design
**Date:** 2026-05-12
**Author:** Brian Buvinghausen (with Claude)

## 1. Scope and non-goals

### In scope

- Add `net10.0` and `net11.0` to the library package; drop `net8.0`.
- Mark the library AOT-compatible on the modern TFMs (`net10.0`, `net11.0`).
- Prove AOT publishing works end-to-end via a CI smoke-test project.
- Add `net11.0` to the test project and run the existing suite with `<Features>runtime-async=on</Features>` to actively exercise our awaiters under Runtime Async.
- Update CI to install the .NET 11 SDK and run the AOT publish.
- Fix stale documentation: `CLAUDE.md` references a hand-authored `TaskTupleExtensions.cs` that no longer exists, and `README.md` still lists ".NET 8.0+" as the modern TFM.

### Non-goals

- **No changes to generator output.** Generated code uses only `Task.WhenAll` plus `ICriticalNotifyCompletion` awaiter structs. This pattern is AOT-safe and Runtime-Async-compatible because the .NET 11 runtime consumes custom awaiters via `UnsafeAwaitAwaiter<TAwaiter>(TAwaiter) where TAwaiter : ICriticalNotifyCompletion`.
- **No new `ConfigureAwaitOptions` overloads.** .NET 11 (through Preview 3) introduces no new flags; the existing feature-detection in the generator continues to be correct.
- **No benchmarks.** Runtime Async's allocation/perf wins land in the *consumer's* `async` method state-machine, not in our awaiter. There is nothing on our side to benchmark.
- **No drop of `netstandard2.0` / `net462`.** Broad reach is a stated selling point in the README; preserving it.

## 2. Target framework changes

| Project | Current | New |
|---|---|---|
| `TaskTupleAwaiter` (library) | `netstandard2.0;net462;net8.0` | `netstandard2.0;net462;net10.0;net11.0` |
| `TaskTupleAwaiter.Generator` | `netstandard2.0` | (unchanged — Roslyn analyzer requirement) |
| `TaskTupleAwaiter.Tests` | `net10.0;net9.0;net8.0;net472` | `net11.0;net10.0;net9.0;net8.0;net472` |
| `TaskTupleAwaiter.AotSmokeTest` (new) | n/a | `net10.0;net11.0` |

**Rationale for the `net8.0` drop:** .NET 8 EOLs 2026-11-10 (per the comment already in `test/Directory.Build.props`), the day after .NET 11 GA. Consumers on .NET 8 still resolve to the `netstandard2.0` target — no behavioral break, just no AOT-tier optimization for that consumer.

## 3. AOT annotations

Add to `src/Directory.Build.props`:

```xml
<PropertyGroup Condition="'$(TargetFramework)' == 'net10.0' or '$(TargetFramework)' == 'net11.0'">
  <IsAotCompatible>true</IsAotCompatible>
</PropertyGroup>
```

This enables the Roslyn AOT analyzer on those TFMs. Because generated code does no reflection, no dynamic codegen, and only delegates to `Task.WhenAll` + struct awaiters, we expect zero warnings. Any warning becomes a build error via the existing `TreatWarningsAsErrors=true`.

## 4. AOT smoke-test project

Add `test/TaskTupleAwaiter.AotSmokeTest/` — a tiny console project that proves end-to-end AOT publishing works for a downstream consumer.

### Project shape

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFrameworks>net10.0;net11.0</TargetFrameworks>
    <PublishAot>true</PublishAot>
    <IsPackable>false</IsPackable>
    <SelfContained>true</SelfContained>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\src\TaskTupleAwaiter\TaskTupleAwaiter.csproj" />
  </ItemGroup>
</Project>
```

### `Program.cs` coverage

Must exercise every generator codepath that could leak reflection or analyzer warnings:

- Typed arity-1 (`ValueTuple<Task<T1>>`) — the special case that delegates directly to the inner task's awaiter.
- Typed arity-2 and arity-16 — the boundaries of the awaiter struct generation loop.
- Non-generic `Task` tuple arity-2 — exercises the non-generic section.
- `ConfigureAwait(false)` and `ConfigureAwait(ConfigureAwaitOptions.None)` — exercises both `bool` and `ConfigureAwaitOptions` overloads.

Implementation is a handful of `await` calls and a `Console.WriteLine` of destructured results. The project's value is binary: it either publishes successfully under AOT or it doesn't.

## 5. Runtime Async verification in test suite

In `test/TaskTupleAwaiter.Tests/TaskTupleAwaiter.Tests.csproj`:

```xml
<PropertyGroup Condition="'$(TargetFramework)' == 'net11.0'">
  <Features>runtime-async=on</Features>
</PropertyGroup>
```

Every existing xUnit test method contains `async`/`await`. When compiled with `runtime-async=on`, those tests become runtime-managed async methods that exercise our awaiters via the new lowering path. The full `BehaviorComparisonTests` matrix (cancellation, exception aggregation, completion ordering, synchronization-context propagation across all 16 arities) becomes the proof.

Add `net11.0` to the `<TargetFrameworks>` list in `test/Directory.Build.props`. The existing EOL comment (`<!-- .NET 8 & 9 end on November 10, 2026 -->`) remains accurate.

## 6. CI changes

`.github/workflows/ci.yml` `setup-dotnet` block:

```yaml
dotnet-version: |
  8.0.*
  9.0.*
  10.0.*
  11.0.*
```

Add AOT publish steps after `dotnet test`:

```yaml
- name: AOT smoke-test (net10.0)
  run: dotnet publish test/TaskTupleAwaiter.AotSmokeTest -c Release -f net10.0
- name: AOT smoke-test (net11.0)
  run: dotnet publish test/TaskTupleAwaiter.AotSmokeTest -c Release -f net11.0
```

`.github/workflows/release.yml`: update `dotnet-version` to `11.0.*` (matches the new release-bearing TFM).

## 7. Documentation updates

- **`README.md`** — update the compatibility table to list .NET 10 and .NET 11; drop .NET 8. Add a short "AOT support" bullet under Features. Mention Runtime Async compatibility explicitly.
- **`CLAUDE.md`** — remove the stale reference to `TaskTupleExtensions.cs` as a "hand-authored fallback / reference implementation" (the file is gone; the source generator is the only source). Update the file-tree section to match reality.

## 8. Risks and mitigations

| Risk | Mitigation |
|---|---|
| `actions/setup-dotnet@v5` does not yet recognize `11.0.*` channel on `windows-latest`. | Fall back to a pinned preview SDK build number until GA; gate AOT step behind SDK presence if necessary. |
| `PublishAot=true` requires the target rid's native toolchain (`link.exe` on Windows). | `windows-latest` already has VS Build Tools installed; should work out of the box. |
| Source generator emits code that the AOT analyzer flags only when consumed by a downstream AOT publish. | The smoke-test project IS the downstream consumer — its publish step catches this. |
| `IDE0130` namespace-suppression pragma in generated output triggers an analyzer under AOT. | Unlikely; `IDE0130` is style-only. Will verify via the smoke-test. |
| Generator's compile-time feature detection for `ConfigureAwaitOptions` misbehaves under .NET 11 compilations. | Detection already inspects the compilation's referenced assemblies (not preprocessor symbols), so it's unaffected by .NET 11. |

## 9. Order of work (input to the implementation plan)

1. Bump `src/Directory.Build.props` TFMs (drop `net8.0`, add `net10.0`, `net11.0`); add `IsAotCompatible` block.
2. Add `net11.0` to test TFMs in `test/Directory.Build.props`; add `runtime-async=on` for `net11.0` in `TaskTupleAwaiter.Tests.csproj`.
3. Create `test/TaskTupleAwaiter.AotSmokeTest/` project (`csproj` + `Program.cs`).
4. Build locally on .NET 11 Preview 3+ SDK; resolve any analyzer warnings (expected: zero).
5. Update `ci.yml` (SDK matrix + AOT publish steps) and `release.yml` (SDK version).
6. Update `README.md` and `CLAUDE.md` for accuracy.

## 10. References

- [What's new in .NET 11 runtime — Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-11/runtime)
- [What's new in .NET 11 — Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-11/overview)
- [Laurent Kempé — Exploring .NET 11 Preview 1 Runtime Async](https://laurentkempe.com/2026/02/14/exploring-net-11-preview-1-runtime-async-a-dive-into-the-future-of-async-in-net/)
- [.NET 11 Preview 1 Arrives with Runtime Async — InfoQ](https://www.infoq.com/news/2026/02/dotnet-11-preview1/)
- [.NET 11 Preview 3 — InfoQ](https://www.infoq.com/news/2026/04/dotnet-11-preview-3/)
- [ConfigureAwait FAQ — .NET Blog](https://devblogs.microsoft.com/dotnet/configureawait-faq/)
