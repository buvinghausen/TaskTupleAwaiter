using System;
using System.Runtime.CompilerServices;

namespace TaskTupleAwaiter.Tests.Adapters;

partial class AwaiterAdapter
{
	public readonly struct AwaiterAdapterAwaiter : ICriticalNotifyCompletion
	{
		private readonly AwaiterAdapter _awaiterAdapter;

		public AwaiterAdapterAwaiter(AwaiterAdapter awaiterAdapter) =>
			_awaiterAdapter = awaiterAdapter;

		public bool IsCompleted =>
			_awaiterAdapter.IsCompleted;

		public void OnCompleted(Action continuation) =>
			_awaiterAdapter.OnCompleted(continuation);

		public void UnsafeOnCompleted(Action continuation) =>
			_awaiterAdapter.UnsafeOnCompleted(continuation);

		public object[] GetResult() =>
			_awaiterAdapter.GetResult();
	}
}
