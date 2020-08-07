using System;
using System.Runtime.CompilerServices;

namespace TaskTupleAwaiter.Tests
{
	partial class AwaiterAdapter
	{
		public readonly struct AwaiterAdapterAwaiter : ICriticalNotifyCompletion
		{
			private readonly AwaiterAdapter awaiterAdapter;

			public AwaiterAdapterAwaiter(AwaiterAdapter awaiterAdapter)
			{
				this.awaiterAdapter = awaiterAdapter;
			}

			public bool IsCompleted => awaiterAdapter.IsCompleted;

			public void OnCompleted(Action continuation) => awaiterAdapter.OnCompleted(continuation);

			public void UnsafeOnCompleted(Action continuation) => awaiterAdapter.UnsafeOnCompleted(continuation);

			public object[] GetResult() => awaiterAdapter.GetResult();
		}
	}
}
