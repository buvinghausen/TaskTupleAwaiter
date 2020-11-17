using System;
using System.Runtime.CompilerServices;

namespace TaskTupleAwaiter.Tests
{
	partial class AwaiterAdapter
	{
		private sealed class VoidResultConfiguredTaskAwaiterAdapter : AwaiterAdapter
		{
			private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _awaiter;

			public VoidResultConfiguredTaskAwaiterAdapter(ConfiguredTaskAwaitable.ConfiguredTaskAwaiter awaiter, string description) : base(description) => _awaiter = awaiter;

			public override bool IsCompleted => _awaiter.IsCompleted;

			public override void OnCompleted(Action continuation) => _awaiter.OnCompleted(continuation);

			public override void UnsafeOnCompleted(Action continuation) => _awaiter.UnsafeOnCompleted(continuation);

			public override object[] GetResult()
			{
				_awaiter.GetResult();
				return null;
			}
		}
	}
}
