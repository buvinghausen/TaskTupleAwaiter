# TaskTupleAwaiter
Async helper library to allow leveraging the new ValueTuple data types in C# 7.0 to thread and run tasks with disparate return types.

```csharp
var (result1, result2) = await (GetStringAsync(), GetGuidAsync());

var (policy, preferences) = await (
    GetPolicyAsync(policyId, cancellationToken),
    GetPreferencesAsync(cancellationToken)
).ConfigureAwait(false);
```

[![NuGet](https://img.shields.io/nuget/v/TaskTupleAwaiter.svg)](https://www.nuget.org/packages/TaskTupleAwaiter/)
