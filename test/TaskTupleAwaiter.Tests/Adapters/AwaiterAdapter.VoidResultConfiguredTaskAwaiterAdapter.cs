using System.Runtime.CompilerServices;

namespace TaskTupleAwaiter.Tests.Adapters;

partial class AwaiterAdapter
{
	private sealed class VoidResultConfiguredTaskAwaiterAdapter(
		ConfiguredTaskAwaitable.ConfiguredTaskAwaiter awaiter,
		string description) : AwaiterAdapter(description)
	{
		private ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _awaiter = awaiter;

		public override bool IsCompleted =>
			_awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			_awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			_awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult()
		{
			_awaiter.GetResult();
			return null;
		}
	}
}
