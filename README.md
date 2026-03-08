# TaskTupleAwaiter

[![Continuous Integration](https://github.com/buvinghausen/TaskTupleAwaiter/workflows/Continuous%20Integration/badge.svg)](https://github.com/buvinghausen/TaskTupleAwaiter/actions)
[![NuGet](https://img.shields.io/nuget/v/TaskTupleAwaiter.svg)](https://www.nuget.org/packages/TaskTupleAwaiter/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/TaskTupleAwaiter.svg)](https://www.nuget.org/packages/TaskTupleAwaiter/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/buvinghausen/TaskTupleAwaiter/blob/master/LICENSE.txt)

**Await multiple tasks with different return types — elegantly.**

TaskTupleAwaiter lets you `await` a tuple of tasks and destructure the results in a single line. No more juggling `Task.WhenAll`, casting from `object`, or writing verbose boilerplate to run independent async operations in parallel.

## Before & After

```csharp
// ❌ Without TaskTupleAwaiter
var userTask = GetUserAsync(id);
var ordersTask = GetOrdersAsync(id);
await Task.WhenAll(userTask, ordersTask);
var user = userTask.Result;
var orders = ordersTask.Result;

// ✅ With TaskTupleAwaiter
var (user, orders) = await (GetUserAsync(id), GetOrdersAsync(id));
```

## Features

- **Tuple-based `await`** — fire multiple async calls in parallel and destructure results with a single `await`
- **Supports up to 16 tasks** — mix and match any combination of return types
- **`ConfigureAwait` support** — works with `ConfigureAwait(false)` and .NET 8+ `ConfigureAwaitOptions`
- **Non-generic `Task` support** — await tuples of `Task` (not just `Task<T>`) when you don't need return values
- **Zero dependencies** — a single file, no external packages (except `System.ValueTuple` on .NET Framework 4.6.2)
- **Broad compatibility** — targets .NET Standard 2.0, .NET Framework 4.6.2, and .NET 8+

## Installation

```shell
dotnet add package TaskTupleAwaiter
```

Or via the NuGet Package Manager:

```
Install-Package TaskTupleAwaiter
```

## Usage

### Basic — two tasks with different return types

```csharp
var (name, age) = await (GetNameAsync(), GetAgeAsync());
```

### ConfigureAwait(false)

```csharp
var (policy, preferences) = await (
    GetPolicyAsync(policyId, cancellationToken),
    GetPreferencesAsync(cancellationToken)
).ConfigureAwait(false);
```

### ConfigureAwaitOptions (.NET 8+)

```csharp
var (user, settings) = await (
    GetUserAsync(userId),
    GetSettingsAsync()
).ConfigureAwait(ConfigureAwaitOptions.None);
```

### Many tasks at once (up to 16)

```csharp
var (a, b, c, d, e) = await (
    GetAAsync(),
    GetBAsync(),
    GetCAsync(),
    GetDAsync(),
    GetEAsync()
);
```

### Non-generic tasks (fire-and-await)

```csharp
await (SendEmailAsync(), LogAuditAsync(), InvalidateCacheAsync());
```

## How It Works

TaskTupleAwaiter provides extension methods on `ValueTuple<Task<T1>, ..., Task<TN>>` (and `ValueTuple<Task, ..., Task>`) that implement the [awaitable pattern](https://learn.microsoft.com/dotnet/csharp/asynchronous-programming/task-asynchronous-programming-model). Under the hood each awaiter calls `Task.WhenAll` to run the tasks concurrently, then unwraps the individual results into a tuple — giving you the performance of parallel execution with the ergonomics of simple destructuring.

## Compatibility

| Target | Version |
|---|---|
| .NET Standard | 2.0 |
| .NET Framework | 4.6.2+ |
| .NET | 8.0+ |

## Credits

Based on the original work by [Joseph Musser (@jnm2)](https://github.com/jnm2) — [original gist](https://gist.github.com/jnm2/3660db29457d391a34151f764bfe6ef7).

## License

[MIT](LICENSE.txt) © Brian Buvinghausen
