using System;
using System.Runtime.CompilerServices;

namespace TaskTupleAwaiter.Tests
{
	partial class AwaiterAdapter
	{
		private sealed class TaskAwaiterAdapter : AwaiterAdapter
		{
			private readonly TaskAwaiter<object[]> awaiter;

			public TaskAwaiterAdapter(TaskAwaiter<object[]> awaiter, string description) : base(description)
			{
				this.awaiter = awaiter;
			}

			public override bool IsCompleted => awaiter.IsCompleted;

			public override void OnCompleted(Action continuation) => awaiter.OnCompleted(continuation);

			public override void UnsafeOnCompleted(Action continuation) => awaiter.UnsafeOnCompleted(continuation);

			public override object[] GetResult() => awaiter.GetResult();
		}
	}
}
