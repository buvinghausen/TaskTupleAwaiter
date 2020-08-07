# TaskTupleAwaiter
![Continuous Integration](https://github.com/buvinghausen/TaskTupleAwaiter/workflows/Continuous%20Integration/badge.svg)[![NuGet](https://img.shields.io/nuget/v/TaskTupleAwaiter.svg)](https://www.nuget.org/packages/TaskTupleAwaiter/)[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/buvinghausen/TaskTupleAwaiter/blob/master/LICENSE.txt)

Async helper library to allow leveraging the new ValueTuple data types in C# 7.0 to thread and run tasks with disparate return types.

```csharp
var (result1, result2) = await (GetStringAsync(), GetGuidAsync());

var (policy, preferences) = await (
    GetPolicyAsync(policyId, cancellationToken),
    GetPreferencesAsync(cancellationToken)
).ConfigureAwait(false);
```


