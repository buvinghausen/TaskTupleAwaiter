namespace TaskTupleAwaiter.Tests.Adapters;

/// <summary>
///     A common abstraction used to compare the behavior of awaiting <c>Task.WhenAll</c>
///     versus awaiting a tuple.
/// </summary>
internal abstract partial class AwaiterAdapter
{
	private readonly string _description;

	private AwaiterAdapter(string description) =>
		_description = description;

	public abstract bool IsCompleted { get; }
	public abstract void OnCompleted(Action continuation);
	public abstract void UnsafeOnCompleted(Action continuation);
	public abstract object[] GetResult();

	public sealed override string ToString() =>
		_description;

	/// <summary>
	///     Enable tests to <see langword="await" /> the adapter itself.
	/// </summary>
	public AwaiterAdapterAwaiter GetAwaiter() =>
		new(this);

	public static IReadOnlyList<AwaiterAdapter> CreateAllAdapters(Task<object>[] tasks) => [
		CreateTaskTupleAwaiter(tasks),
		CreateTaskTupleAwaiter(tasks, true),
		CreateTaskTupleAwaiter(tasks, false),
		CreateVoidResultTaskTupleAwaiter(tasks),
		CreateVoidResultTaskTupleAwaiter(tasks, true),
		CreateVoidResultTaskTupleAwaiter(tasks, false),
		CreateTaskWhenAll(tasks),
		CreateTaskWhenAll(tasks, true),
		CreateTaskWhenAll(tasks, false),
		CreateVoidResultTaskWhenAll(tasks),
		CreateVoidResultTaskWhenAll(tasks, true),
		CreateVoidResultTaskWhenAll(tasks, false)
	];

	public static IReadOnlyList<AwaiterAdapter> CreateNonVoidResultAdapters(Task<object>[] tasks) => [
		CreateTaskTupleAwaiter(tasks),
		CreateTaskTupleAwaiter(tasks, true),
		CreateTaskTupleAwaiter(tasks, false),
		CreateTaskWhenAll(tasks),
		CreateTaskWhenAll(tasks, true),
		CreateTaskWhenAll(tasks, false)
	];

	public static IReadOnlyList<AwaiterAdapter>	CreateAdapters(Task<object>[] tasks, bool? continueOnCapturedContext) => [
		CreateTaskTupleAwaiter(tasks, continueOnCapturedContext),
		CreateVoidResultTaskTupleAwaiter(tasks, continueOnCapturedContext),
		CreateTaskWhenAll(tasks, continueOnCapturedContext),
		CreateVoidResultTaskWhenAll(tasks, continueOnCapturedContext)
	];

	public static AwaiterAdapter CreateTaskWhenAll(Task<object>[] tasks, bool? continueOnCapturedContext = null)
	{
		if (continueOnCapturedContext != null)
			return new ConfiguredTaskAwaiterAdapter(Task.WhenAll(tasks).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), $"await Task.WhenAll(…).ConfigureAwait({continueOnCapturedContext.Value})");
		return new TaskAwaiterAdapter(Task.WhenAll(tasks).GetAwaiter(), "await Task.WhenAll(…)");
	}

	public static AwaiterAdapter CreateVoidResultTaskWhenAll(Task[] tasks, bool? continueOnCapturedContext = null)
	{
		if (continueOnCapturedContext != null)
			return new VoidResultConfiguredTaskAwaiterAdapter(Task.WhenAll(tasks).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), $"await Task.WhenAll(…).ConfigureAwait({continueOnCapturedContext.Value})");

		return new VoidResultTaskAwaiterAdapter(Task.WhenAll(tasks).GetAwaiter(), "await Task.WhenAll(…)");
	}

	public static AwaiterAdapter CreateTaskTupleAwaiter(Task<object>[] tasks, bool? continueOnCapturedContext = null)
	{
		if (continueOnCapturedContext != null)
		{
			var description = $"await (t, …).ConfigureAwait({continueOnCapturedContext.Value})";

			return tasks.Length switch
			{
				1 => new ConfiguredTaskTupleAwaiter1Adapter(ValueTuple.Create(tasks[0]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), $"await ValueTuple.Create(t).ConfigureAwait({continueOnCapturedContext.Value})"),
				2 => new ConfiguredTaskTupleAwaiter2Adapter((tasks[0], tasks[1]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), description),
				3 => new ConfiguredTaskTupleAwaiter3Adapter((tasks[0], tasks[1], tasks[2]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), description),
				4 => new ConfiguredTaskTupleAwaiter4Adapter((tasks[0], tasks[1], tasks[2], tasks[3]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), description),
				5 => new ConfiguredTaskTupleAwaiter5Adapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), description),
				6 => new ConfiguredTaskTupleAwaiter6Adapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), description),
				7 => new ConfiguredTaskTupleAwaiter7Adapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), description),
				8 => new ConfiguredTaskTupleAwaiter8Adapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), description),
				9 => new ConfiguredTaskTupleAwaiter9Adapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), description),
				10 => new ConfiguredTaskTupleAwaiter10Adapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8], tasks[9]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), description),
				11 => new ConfiguredTaskTupleAwaiter11Adapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8], tasks[9], tasks[10]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), description),
				12 => new ConfiguredTaskTupleAwaiter12Adapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8], tasks[9], tasks[10], tasks[11]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), description),
				13 => new ConfiguredTaskTupleAwaiter13Adapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8], tasks[9], tasks[10], tasks[11], tasks[12]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), description),
				14 => new ConfiguredTaskTupleAwaiter14Adapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8], tasks[9], tasks[10], tasks[11], tasks[12], tasks[13]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), description),
				15 => new ConfiguredTaskTupleAwaiter15Adapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8], tasks[9], tasks[10], tasks[11], tasks[12], tasks[13], tasks[14]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), description),
				16 => new ConfiguredTaskTupleAwaiter16Adapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8], tasks[9], tasks[10], tasks[11], tasks[12], tasks[13], tasks[14], tasks[15]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), description),
				_ => throw new ArgumentException($"There is no awaiter for a tuple of {tasks.Length} tasks.", nameof(tasks))
			};
		}
		else
		{
			const string description = "await (t, …)";

			return tasks.Length switch
			{
				1 => new TaskTupleAwaiter1Adapter(ValueTuple.Create(tasks[0]).GetAwaiter(),	"await ValueTuple.Create(t)"),
				2 => new TaskTupleAwaiter2Adapter((tasks[0], tasks[1]).GetAwaiter(), description),
				3 => new TaskTupleAwaiter3Adapter((tasks[0], tasks[1], tasks[2]).GetAwaiter(), description),
				4 => new TaskTupleAwaiter4Adapter((tasks[0], tasks[1], tasks[2], tasks[3]).GetAwaiter(), description),
				5 => new TaskTupleAwaiter5Adapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4]).GetAwaiter(), description),
				6 => new TaskTupleAwaiter6Adapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5]).GetAwaiter(), description),
				7 => new TaskTupleAwaiter7Adapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6]).GetAwaiter(), description),
				8 => new TaskTupleAwaiter8Adapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7]).GetAwaiter(), description),
				9 => new TaskTupleAwaiter9Adapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8]).GetAwaiter(), description),
				10 => new TaskTupleAwaiter10Adapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8], tasks[9]).GetAwaiter(), description),
				11 => new TaskTupleAwaiter11Adapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8], tasks[9], tasks[10]).GetAwaiter(), description),
				12 => new TaskTupleAwaiter12Adapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8], tasks[9], tasks[10], tasks[11]).GetAwaiter(), description),
				13 => new TaskTupleAwaiter13Adapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8], tasks[9], tasks[10], tasks[11], tasks[12]).GetAwaiter(), description),
				14 => new TaskTupleAwaiter14Adapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8], tasks[9], tasks[10], tasks[11], tasks[12], tasks[13]).GetAwaiter(), description),
				15 => new TaskTupleAwaiter15Adapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8], tasks[9], tasks[10], tasks[11], tasks[12], tasks[13], tasks[14]).GetAwaiter(), description),
				16 => new TaskTupleAwaiter16Adapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8], tasks[9], tasks[10], tasks[11], tasks[12], tasks[13], tasks[14], tasks[15]).GetAwaiter(), description),
				_ => throw new ArgumentException($"There is no awaiter for a tuple of {tasks.Length} tasks.", nameof(tasks))
			};
		}
	}

	public static AwaiterAdapter CreateVoidResultTaskTupleAwaiter(Task[] tasks, bool? continueOnCapturedContext = null)
	{
		if (continueOnCapturedContext != null)
		{
			var description = $"await (t, …).ConfigureAwait({continueOnCapturedContext.Value})";
			return tasks.Length switch
			{
				1 => new VoidResultConfiguredTaskAwaiterAdapter(ValueTuple.Create(tasks[0]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), $"await ValueTuple.Create(t).ConfigureAwait({continueOnCapturedContext.Value})"),
				2 => new VoidResultConfiguredTaskAwaiterAdapter((tasks[0], tasks[1]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), description),
				3 => new VoidResultConfiguredTaskAwaiterAdapter((tasks[0], tasks[1], tasks[2]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), description),
				4 => new VoidResultConfiguredTaskAwaiterAdapter((tasks[0], tasks[1], tasks[2], tasks[3]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), description),
				5 => new VoidResultConfiguredTaskAwaiterAdapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), description),
				6 => new VoidResultConfiguredTaskAwaiterAdapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), description),
				7 => new VoidResultConfiguredTaskAwaiterAdapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), description),
				8 => new VoidResultConfiguredTaskAwaiterAdapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4],tasks[5], tasks[6], tasks[7]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), description),
				9 => new VoidResultConfiguredTaskAwaiterAdapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), description),
				10 => new VoidResultConfiguredTaskAwaiterAdapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8], tasks[9]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), description),
				11 => new VoidResultConfiguredTaskAwaiterAdapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8], tasks[9], tasks[10]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), description),
				12 => new VoidResultConfiguredTaskAwaiterAdapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8], tasks[9], tasks[10], tasks[11]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), description),
				13 => new VoidResultConfiguredTaskAwaiterAdapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8], tasks[9], tasks[10], tasks[11], tasks[12]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), description),
				14 => new VoidResultConfiguredTaskAwaiterAdapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8], tasks[9], tasks[10], tasks[11], tasks[12], tasks[13]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), description),
				15 => new VoidResultConfiguredTaskAwaiterAdapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8], tasks[9], tasks[10], tasks[11], tasks[12], tasks[13], tasks[14]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), description),
				16 => new VoidResultConfiguredTaskAwaiterAdapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8], tasks[9], tasks[10], tasks[11], tasks[12], tasks[13], tasks[14], tasks[15]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(), description),
				_ => throw new ArgumentException($"There is no awaiter for a tuple of {tasks.Length} tasks.", nameof(tasks))
			};
		}
		else
		{
			const string description = "await (t, …)";

			return tasks.Length switch
			{
				1 => new VoidResultTaskAwaiterAdapter(ValueTuple.Create(tasks[0]).GetAwaiter(), "await ValueTuple.Create(t)"),
				2 => new VoidResultTaskAwaiterAdapter((tasks[0], tasks[1]).GetAwaiter(), description),
				3 => new VoidResultTaskAwaiterAdapter((tasks[0], tasks[1], tasks[2]).GetAwaiter(), description),
				4 => new VoidResultTaskAwaiterAdapter((tasks[0], tasks[1], tasks[2], tasks[3]).GetAwaiter(), description),
				5 => new VoidResultTaskAwaiterAdapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4]) .GetAwaiter(), description),
				6 => new VoidResultTaskAwaiterAdapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5]).GetAwaiter(), description),
				7 => new VoidResultTaskAwaiterAdapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6]).GetAwaiter(), description),
				8 => new VoidResultTaskAwaiterAdapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7]).GetAwaiter(), description),
				9 => new VoidResultTaskAwaiterAdapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8]).GetAwaiter(), description),
				10 => new VoidResultTaskAwaiterAdapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8], tasks[9]).GetAwaiter(), description),
				11 => new VoidResultTaskAwaiterAdapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8], tasks[9], tasks[10]).GetAwaiter(), description),
				12 => new VoidResultTaskAwaiterAdapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8], tasks[9], tasks[10], tasks[11]).GetAwaiter(), description),
				13 => new VoidResultTaskAwaiterAdapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8], tasks[9], tasks[10], tasks[11], tasks[12]).GetAwaiter(), description),
				14 => new VoidResultTaskAwaiterAdapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8], tasks[9], tasks[10], tasks[11], tasks[12], tasks[13]).GetAwaiter(), description),
				15 => new VoidResultTaskAwaiterAdapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8], tasks[9], tasks[10], tasks[11], tasks[12], tasks[13], tasks[14]).GetAwaiter(), description),
				16 => new VoidResultTaskAwaiterAdapter((tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8], tasks[9], tasks[10], tasks[11], tasks[12], tasks[13], tasks[14], tasks[15]).GetAwaiter(), description),
				_ => throw new ArgumentException($"There is no awaiter for a tuple of {tasks.Length} tasks.", nameof(tasks))
			};
		}
	}
}
