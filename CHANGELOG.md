# Changelog

All notable changes to TaskTupleAwaiter are documented here.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project adheres to
[Semantic Versioning](https://semver.org/spec/v2.0.0.html). The package version comes from the GitHub release tag;
see each [GitHub release](https://github.com/buvinghausen/TaskTupleAwaiter/releases) for full notes and benchmarks.

## [Unreleased]

Planned as **3.0.0**: removing a target framework is a breaking change. Implements
[discussion #46](https://github.com/buvinghausen/TaskTupleAwaiter/discussions/46).

### Removed
- **`net462` target.** .NET Framework 4.6.2 leaves support January 12, 2027. .NET Framework apps now get the
  `netstandard2.0` build. **.NET Framework 4.7.2+** is the tested, supported floor.
- **`System.ValueTuple` dependency.** It was needed only by `net462`, so the package now has no dependencies on any
  target. This also rules out the `System.ValueTuple` version mismatches behind
  [#34](https://github.com/buvinghausen/TaskTupleAwaiter/issues/34) (see 2.1.2 and 2.1.3).

### Changed
- The `netstandard2.0` build is now covered by the full test suite (run on .NET Framework 4.7.2). Previously that build
  shipped untested.

### Using 3.x on .NET Framework 4.6.2–4.7.1
These versions resolve the `netstandard2.0` build, which depends on .NET Standard facade assemblies (`netstandard.dll`,
`System.Runtime.dll`, …) copied next to your app. Those facades need **binding redirects**:

- **SDK-style or `PackageReference` projects (console, WinForms, WPF, services):** redirects are generated
  automatically. Make sure you haven't turned off `<AutoGenerateBindingRedirects>`.
- **Classic ASP.NET / web projects:** add this to the project file so the redirects go into `web.config`:
  ```xml
  <PropertyGroup>
    <AutoGenerateBindingRedirects>true</AutoGenerateBindingRedirects>
    <GenerateBindingRedirectsOutputType>true</GenerateBindingRedirectsOutputType>
  </PropertyGroup>
  ```
- **`packages.config` projects:** run `Add-BindingRedirect` in the Package Manager Console after upgrading, or migrate
  to `PackageReference`.

A `FileLoadException` or `FileNotFoundException` for `netstandard`, `System.Runtime` or a `System.*` facade at startup
almost always means a redirect is missing. .NET Framework 4.7.2+ has these facades built in and needs none of this.
Once your redirects are set up for 3.0, later 3.x releases won't need any new ones: the target frameworks and public
API are expected to stay the same across the major version.

## [2.3.0] - 2026-05-15

### Added
- `net9.0` target. The generated `Task.WhenAll(...)` binds to `Task.WhenAll(ReadOnlySpan<Task>)`, stack-allocating the
  task buffer (typed tuples allocate 20–57% less per `await` than on `net8.0`).
- BenchmarkDotNet harness in `benches/`.

### Changed
- Generated awaiters are plain `readonly struct` instead of `readonly record struct`; trivial forwarders are
  `[MethodImpl(AggressiveInlining)]`. Smaller DLL, no public API change.

## [2.2.0] - 2026-05-13

### Added
- Native AOT compatibility (`IsAotCompatible`) on `net8.0`+, verified by a published AOT smoke test in CI.

### Changed
- The extension methods are now emitted by a private Roslyn incremental source generator at build time. The IL is
  equivalent to 2.1.3.
- Test matrix includes `net11.0` with runtime async.

## [2.1.3] - 2026-03-07

### Changed
- Restored the `net462` target with a `System.ValueTuple` 4.6.2 dependency, which fixes the 4.6.1 package bug worked
  around in 2.1.2.

## [2.1.2] - 2026-01-06

### Fixed
- CS0012 compile errors on .NET Framework when a project also references `System.ValueTuple` 4.6.1 (for example via
  System.Text.Json 10) ([#34](https://github.com/buvinghausen/TaskTupleAwaiter/issues/34)). Worked around by dropping
  `net462` so .NET Framework uses the `netstandard2.0` build.

## [2.1.1] - 2025-08-14

### Changed
- Package license uses `<PackageLicenseExpression>`.

## [2.1.0] - 2024-03-20

### Added
- .NET 8 target with `ConfigureAwait(ConfigureAwaitOptions)` overloads.
- Tuple arity raised to 16.

## [2.0.3] - 2023-02-27

### Removed
- `System.Net.Http` reference on legacy .NET.

## [2.0.2] - 2022-10-25

### Changed
- Back to publishing symbols to a symbol server.

## [2.0.1] - 2022-10-05

### Added
- .NET 7, .NET Standard 2.0, and .NET Standard 2.1 targets; embedded symbols for SourceLink.

### Removed
- .NET 5 target.

### Changed
- Awaiter structs are `readonly record struct`.

## [2.0.0] - 2021-09-15

### Changed
- **Breaking:** extension methods moved to the `System.Threading.Tasks` namespace.
- **Breaking:** .NET Framework 4.6.2 is the minimum; .NET Core 2.1 dropped.

### Added
- .NET 6, .NET 5, and .NET Core 3.1 targets.

## [1.2.3] - 2020-11-11

### Fixed
- Duplicated package description ([#20](https://github.com/buvinghausen/TaskTupleAwaiter/issues/20)).

## [1.2.2] - 2020-07-31

### Changed
- Releases publish to NuGet automatically. No code changes.

[Unreleased]: https://github.com/buvinghausen/TaskTupleAwaiter/compare/2.3.0...HEAD
[2.3.0]: https://github.com/buvinghausen/TaskTupleAwaiter/compare/2.2.0...2.3.0
[2.2.0]: https://github.com/buvinghausen/TaskTupleAwaiter/compare/2.1.3...2.2.0
[2.1.3]: https://github.com/buvinghausen/TaskTupleAwaiter/compare/2.1.2...2.1.3
[2.1.2]: https://github.com/buvinghausen/TaskTupleAwaiter/compare/2.1.1...2.1.2
[2.1.1]: https://github.com/buvinghausen/TaskTupleAwaiter/compare/2.1.0...2.1.1
[2.1.0]: https://github.com/buvinghausen/TaskTupleAwaiter/compare/2.0.3...2.1.0
[2.0.3]: https://github.com/buvinghausen/TaskTupleAwaiter/compare/2.0.2...2.0.3
[2.0.2]: https://github.com/buvinghausen/TaskTupleAwaiter/compare/2.0.1...2.0.2
[2.0.1]: https://github.com/buvinghausen/TaskTupleAwaiter/compare/2.0.0...2.0.1
[2.0.0]: https://github.com/buvinghausen/TaskTupleAwaiter/compare/1.2.3...2.0.0
[1.2.3]: https://github.com/buvinghausen/TaskTupleAwaiter/compare/1.2.2...1.2.3
[1.2.2]: https://github.com/buvinghausen/TaskTupleAwaiter/releases/tag/1.2.2
