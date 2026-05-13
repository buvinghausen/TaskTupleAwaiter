using System.Runtime.CompilerServices;

namespace TaskTupleAwaiter.Tests.Adapters;

partial class AwaiterAdapter
{
	sealed class ConfiguredTaskAwaiterAdapter(
		ConfiguredTaskAwaitable<object[]>.ConfiguredTaskAwaiter awaiter,
		string description) : AwaiterAdapter(description)
	{
		readonly ConfiguredTaskAwaitable<object[]>.ConfiguredTaskAwaiter _awaiter = awaiter;

		public override bool IsCompleted =>
			_awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			_awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			_awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult() =>
			_awaiter.GetResult();
	}
}
