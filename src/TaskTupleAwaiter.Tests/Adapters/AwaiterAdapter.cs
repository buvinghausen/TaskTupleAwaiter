using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaskTupleAwaiter.Tests
{
	/// <summary>
	/// A common abstraction used to compare the behavior of awaiting <c>Task.WhenAll</c>
	/// versus awaiting a tuple.
	/// </summary>
	public abstract partial class AwaiterAdapter
	{
		private readonly string description;

		private AwaiterAdapter(string description)
		{
			this.description = description;
		}

		public abstract bool IsCompleted { get; }
		public abstract void OnCompleted(Action continuation);
		public abstract void UnsafeOnCompleted(Action continuation);
		public abstract object[] GetResult();

		public sealed override string ToString() => description;

		/// <summary>
		/// Enable tests to <see langword="await" /> the adapter itself.
		/// </summary>
		public AwaiterAdapterAwaiter GetAwaiter() => new AwaiterAdapterAwaiter(this);

		public static IReadOnlyCollection<AwaiterAdapter> CreateAllAdapters(Task<object>[] tasks) => new[]
		{
			CreateTaskTupleAwaiter(tasks),
			CreateTaskTupleAwaiter(tasks, continueOnCapturedContext: true),
			CreateTaskTupleAwaiter(tasks, continueOnCapturedContext: false),
			CreateVoidResultTaskTupleAwaiter(tasks),
			CreateVoidResultTaskTupleAwaiter(tasks, continueOnCapturedContext: true),
			CreateVoidResultTaskTupleAwaiter(tasks, continueOnCapturedContext: false),
			CreateTaskWhenAll(tasks),
			CreateTaskWhenAll(tasks, continueOnCapturedContext: true),
			CreateTaskWhenAll(tasks, continueOnCapturedContext: false),
			CreateVoidResultTaskWhenAll(tasks),
			CreateVoidResultTaskWhenAll(tasks, continueOnCapturedContext: true),
			CreateVoidResultTaskWhenAll(tasks, continueOnCapturedContext: false)
		};

		public static IReadOnlyCollection<AwaiterAdapter> CreateNonVoidResultAdapters(Task<object>[] tasks) => new[]
		{
			CreateTaskTupleAwaiter(tasks),
			CreateTaskTupleAwaiter(tasks, continueOnCapturedContext: true),
			CreateTaskTupleAwaiter(tasks, continueOnCapturedContext: false),
			CreateTaskWhenAll(tasks),
			CreateTaskWhenAll(tasks, continueOnCapturedContext: true),
			CreateTaskWhenAll(tasks, continueOnCapturedContext: false)
		};

		public static AwaiterAdapter CreateTaskWhenAll(Task<object>[] tasks, bool? continueOnCapturedContext = null)
		{
			if (continueOnCapturedContext != null)
			{
				return new ConfiguredTaskAwaiterAdapter(
					Task.WhenAll(tasks).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(),
					$"await Task.WhenAll(…).ConfigureAwait({continueOnCapturedContext.Value})");
			}

			return new TaskAwaiterAdapter(Task.WhenAll(tasks).GetAwaiter(), "await Task.WhenAll(…)");
		}

		public static AwaiterAdapter CreateVoidResultTaskWhenAll(Task[] tasks, bool? continueOnCapturedContext = null)
		{
			if (continueOnCapturedContext != null)
			{
				return new VoidResultConfiguredTaskAwaiterAdapter(
					Task.WhenAll(tasks).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(),
					$"await Task.WhenAll(…).ConfigureAwait({continueOnCapturedContext.Value})");
			}

			return new VoidResultTaskAwaiterAdapter(Task.WhenAll(tasks).GetAwaiter(), "await Task.WhenAll(…)");
		}

		public static AwaiterAdapter CreateTaskTupleAwaiter(Task<object>[] tasks, bool? continueOnCapturedContext = null)
		{
			if (continueOnCapturedContext != null)
			{
				var description = $"await (t, …).ConfigureAwait({continueOnCapturedContext.Value})";

				switch (tasks.Length)
				{
					case 1:
						return new ConfiguredTaskTupleAwaiter1Adapter(
							ValueTuple.Create(tasks[0]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(),
							$"await ValueTuple.Create(t).ConfigureAwait({continueOnCapturedContext.Value})");
					case 2:
						return new ConfiguredTaskTupleAwaiter2Adapter(
							(tasks[0], tasks[1]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(),
							description);
					case 3:
						return new ConfiguredTaskTupleAwaiter3Adapter(
							(tasks[0], tasks[1], tasks[2]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(),
							description);
					case 4:
						return new ConfiguredTaskTupleAwaiter4Adapter(
							(tasks[0], tasks[1], tasks[2], tasks[3]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(),
							description);
					case 5:
						return new ConfiguredTaskTupleAwaiter5Adapter(
							(tasks[0], tasks[1], tasks[2], tasks[3], tasks[4]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(),
							description);
					case 6:
						return new ConfiguredTaskTupleAwaiter6Adapter(
							(tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(),
							description);
					case 7:
						return new ConfiguredTaskTupleAwaiter7Adapter(
							(tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(),
							description);
					case 8:
						return new ConfiguredTaskTupleAwaiter8Adapter(
							(tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(),
							description);
					case 9:
						return new ConfiguredTaskTupleAwaiter9Adapter(
							(tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(),
							description);
					case 10:
						return new ConfiguredTaskTupleAwaiter10Adapter(
							(tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8], tasks[9]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(),
							description);
					default:
						throw new ArgumentException($"There is no awaiter for a tuple of {tasks.Length} tasks.", nameof(tasks));
				}
			}
			else
			{
				const string description = "await (t, …)";

				switch (tasks.Length)
				{
					case 1:
						return new TaskTupleAwaiter1Adapter(
							ValueTuple.Create(tasks[0]).GetAwaiter(),
							"await ValueTuple.Create(t)");
					case 2:
						return new TaskTupleAwaiter2Adapter(
							(tasks[0], tasks[1]).GetAwaiter(),
							description);
					case 3:
						return new TaskTupleAwaiter3Adapter(
							(tasks[0], tasks[1], tasks[2]).GetAwaiter(),
							description);
					case 4:
						return new TaskTupleAwaiter4Adapter(
							(tasks[0], tasks[1], tasks[2], tasks[3]).GetAwaiter(),
							description);
					case 5:
						return new TaskTupleAwaiter5Adapter(
							(tasks[0], tasks[1], tasks[2], tasks[3], tasks[4]).GetAwaiter(),
							description);
					case 6:
						return new TaskTupleAwaiter6Adapter(
							(tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5]).GetAwaiter(),
							description);
					case 7:
						return new TaskTupleAwaiter7Adapter(
							(tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6]).GetAwaiter(),
							description);
					case 8:
						return new TaskTupleAwaiter8Adapter(
							(tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7]).GetAwaiter(),
							description);
					case 9:
						return new TaskTupleAwaiter9Adapter(
							(tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8]).GetAwaiter(),
							description);
					case 10:
						return new TaskTupleAwaiter10Adapter(
							(tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8], tasks[9]).GetAwaiter(),
							description);
					default:
						throw new ArgumentException($"There is no awaiter for a tuple of {tasks.Length} tasks.", nameof(tasks));
				}
			}
		}

		public static AwaiterAdapter CreateVoidResultTaskTupleAwaiter(Task[] tasks, bool? continueOnCapturedContext = null)
		{
			if (continueOnCapturedContext != null)
			{
				var description = $"await (t, …).ConfigureAwait({continueOnCapturedContext.Value})";

				switch (tasks.Length)
				{
					case 1:
						return new VoidResultConfiguredTaskAwaiterAdapter(
							ValueTuple.Create(tasks[0]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(),
							$"await ValueTuple.Create(t).ConfigureAwait({continueOnCapturedContext.Value})");
					case 2:
						return new VoidResultConfiguredTaskAwaiterAdapter(
							(tasks[0], tasks[1]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(),
							description);
					case 3:
						return new VoidResultConfiguredTaskAwaiterAdapter(
							(tasks[0], tasks[1], tasks[2]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(),
							description);
					case 4:
						return new VoidResultConfiguredTaskAwaiterAdapter(
							(tasks[0], tasks[1], tasks[2], tasks[3]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(),
							description);
					case 5:
						return new VoidResultConfiguredTaskAwaiterAdapter(
							(tasks[0], tasks[1], tasks[2], tasks[3], tasks[4]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(),
							description);
					case 6:
						return new VoidResultConfiguredTaskAwaiterAdapter(
							(tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(),
							description);
					case 7:
						return new VoidResultConfiguredTaskAwaiterAdapter(
							(tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(),
							description);
					case 8:
						return new VoidResultConfiguredTaskAwaiterAdapter(
							(tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(),
							description);
					case 9:
						return new VoidResultConfiguredTaskAwaiterAdapter(
							(tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(),
							description);
					case 10:
						return new VoidResultConfiguredTaskAwaiterAdapter(
							(tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8], tasks[9]).ConfigureAwait(continueOnCapturedContext.Value).GetAwaiter(),
							description);
					default:
						throw new ArgumentException($"There is no awaiter for a tuple of {tasks.Length} tasks.", nameof(tasks));
				}
			}
			else
			{
				const string description = "await (t, …)";

				switch (tasks.Length)
				{
					case 1:
						return new VoidResultTaskAwaiterAdapter(
							ValueTuple.Create(tasks[0]).GetAwaiter(),
							"await ValueTuple.Create(t)");
					case 2:
						return new VoidResultTaskAwaiterAdapter(
							(tasks[0], tasks[1]).GetAwaiter(),
							description);
					case 3:
						return new VoidResultTaskAwaiterAdapter(
							(tasks[0], tasks[1], tasks[2]).GetAwaiter(),
							description);
					case 4:
						return new VoidResultTaskAwaiterAdapter(
							(tasks[0], tasks[1], tasks[2], tasks[3]).GetAwaiter(),
							description);
					case 5:
						return new VoidResultTaskAwaiterAdapter(
							(tasks[0], tasks[1], tasks[2], tasks[3], tasks[4]).GetAwaiter(),
							description);
					case 6:
						return new VoidResultTaskAwaiterAdapter(
							(tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5]).GetAwaiter(),
							description);
					case 7:
						return new VoidResultTaskAwaiterAdapter(
							(tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6]).GetAwaiter(),
							description);
					case 8:
						return new VoidResultTaskAwaiterAdapter(
							(tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7]).GetAwaiter(),
							description);
					case 9:
						return new VoidResultTaskAwaiterAdapter(
							(tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8]).GetAwaiter(),
							description);
					case 10:
						return new VoidResultTaskAwaiterAdapter(
							(tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6], tasks[7], tasks[8], tasks[9]).GetAwaiter(),
							description);
					default:
						throw new ArgumentException($"There is no awaiter for a tuple of {tasks.Length} tasks.", nameof(tasks));
				}
			}
		}
	}
}
