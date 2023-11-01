using System.Runtime.CompilerServices;

namespace TaskTupleAwaiter.Tests.Adapters;

internal partial class AwaiterAdapter
{
	private sealed class VoidResultConfiguredTaskAwaiterAdapter(ConfiguredTaskAwaitable.ConfiguredTaskAwaiter awaiter, string description) : AwaiterAdapter(description)
	{
		public override bool IsCompleted =>
			awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult()
		{
			awaiter.GetResult();
			return null;
		}
	}
}
