using System;
using System.Runtime.CompilerServices;

namespace TaskTupleAwaiter.Tests
{
	partial class AwaiterAdapter
	{
		private sealed class TaskAwaiterAdapter : AwaiterAdapter
		{
			private readonly TaskAwaiter<object[]> _awaiter;

			public TaskAwaiterAdapter(TaskAwaiter<object[]> awaiter, string description) : base(description) => _awaiter = awaiter;

			public override bool IsCompleted => _awaiter.IsCompleted;

			public override void OnCompleted(Action continuation) => _awaiter.OnCompleted(continuation);

			public override void UnsafeOnCompleted(Action continuation) => _awaiter.UnsafeOnCompleted(continuation);

			public override object[] GetResult() => _awaiter.GetResult();
		}
	}
}
