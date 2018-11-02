using System;
using System.Runtime.CompilerServices;
using static TaskTupleAwaiter.TaskTupleExtensions;

namespace TaskTupleAwaiter.Tests
{
	partial class AwaiterAdapter
	{
		private sealed class TaskTupleAwaiter1Adapter : AwaiterAdapter
		{
			private readonly TaskAwaiter<object> awaiter;

			public TaskTupleAwaiter1Adapter(TaskAwaiter<object> awaiter, string description) : base(description)
			{
				this.awaiter = awaiter;
			}

			public override bool IsCompleted => awaiter.IsCompleted;

			public override void OnCompleted(Action continuation) => awaiter.OnCompleted(continuation);

			public override void UnsafeOnCompleted(Action continuation) => awaiter.UnsafeOnCompleted(continuation);

			public override object[] GetResult()
			{
				return new[] { awaiter.GetResult() };
			}
		}

		private sealed class TaskTupleAwaiter2Adapter : AwaiterAdapter
		{
			private readonly TupleTaskAwaiter<object, object> awaiter;

			public TaskTupleAwaiter2Adapter(TupleTaskAwaiter<object, object> awaiter, string description) : base(description)
			{
				this.awaiter = awaiter;
			}

			public override bool IsCompleted => awaiter.IsCompleted;

			public override void OnCompleted(Action continuation) => awaiter.OnCompleted(continuation);

			public override void UnsafeOnCompleted(Action continuation) => awaiter.UnsafeOnCompleted(continuation);

			public override object[] GetResult()
			{
				var result = awaiter.GetResult();
				return new[] { result.Item1, result.Item2 };
			}
		}

		private sealed class TaskTupleAwaiter3Adapter : AwaiterAdapter
		{
			private readonly TupleTaskAwaiter<object, object, object> awaiter;

			public TaskTupleAwaiter3Adapter(TupleTaskAwaiter<object, object, object> awaiter, string description) : base(description)
			{
				this.awaiter = awaiter;
			}

			public override bool IsCompleted => awaiter.IsCompleted;

			public override void OnCompleted(Action continuation) => awaiter.OnCompleted(continuation);

			public override void UnsafeOnCompleted(Action continuation) => awaiter.UnsafeOnCompleted(continuation);

			public override object[] GetResult()
			{
				var result = awaiter.GetResult();
				return new[] { result.Item1, result.Item2, result.Item3 };
			}
		}

		private sealed class TaskTupleAwaiter4Adapter : AwaiterAdapter
		{
			private readonly TupleTaskAwaiter<object, object, object, object> awaiter;

			public TaskTupleAwaiter4Adapter(TupleTaskAwaiter<object, object, object, object> awaiter, string description) : base(description)
			{
				this.awaiter = awaiter;
			}

			public override bool IsCompleted => awaiter.IsCompleted;

			public override void OnCompleted(Action continuation) => awaiter.OnCompleted(continuation);

			public override void UnsafeOnCompleted(Action continuation) => awaiter.UnsafeOnCompleted(continuation);

			public override object[] GetResult()
			{
				var result = awaiter.GetResult();
				return new[] { result.Item1, result.Item2, result.Item3, result.Item4 };
			}
		}

		private sealed class TaskTupleAwaiter5Adapter : AwaiterAdapter
		{
			private readonly TupleTaskAwaiter<object, object, object, object, object> awaiter;

			public TaskTupleAwaiter5Adapter(TupleTaskAwaiter<object, object, object, object, object> awaiter, string description) : base(description)
			{
				this.awaiter = awaiter;
			}

			public override bool IsCompleted => awaiter.IsCompleted;

			public override void OnCompleted(Action continuation) => awaiter.OnCompleted(continuation);

			public override void UnsafeOnCompleted(Action continuation) => awaiter.UnsafeOnCompleted(continuation);

			public override object[] GetResult()
			{
				var result = awaiter.GetResult();
				return new[] { result.Item1, result.Item2, result.Item3, result.Item4, result.Item5 };
			}
		}

		private sealed class TaskTupleAwaiter6Adapter : AwaiterAdapter
		{
			private readonly TupleTaskAwaiter<object, object, object, object, object, object> awaiter;

			public TaskTupleAwaiter6Adapter(TupleTaskAwaiter<object, object, object, object, object, object> awaiter, string description) : base(description)
			{
				this.awaiter = awaiter;
			}

			public override bool IsCompleted => awaiter.IsCompleted;

			public override void OnCompleted(Action continuation) => awaiter.OnCompleted(continuation);

			public override void UnsafeOnCompleted(Action continuation) => awaiter.UnsafeOnCompleted(continuation);

			public override object[] GetResult()
			{
				var result = awaiter.GetResult();
				return new[] { result.Item1, result.Item2, result.Item3, result.Item4, result.Item5, result.Item6 };
			}
		}

		private sealed class TaskTupleAwaiter7Adapter : AwaiterAdapter
		{
			private readonly TupleTaskAwaiter<object, object, object, object, object, object, object> awaiter;

			public TaskTupleAwaiter7Adapter(TupleTaskAwaiter<object, object, object, object, object, object, object> awaiter, string description) : base(description)
			{
				this.awaiter = awaiter;
			}

			public override bool IsCompleted => awaiter.IsCompleted;

			public override void OnCompleted(Action continuation) => awaiter.OnCompleted(continuation);

			public override void UnsafeOnCompleted(Action continuation) => awaiter.UnsafeOnCompleted(continuation);

			public override object[] GetResult()
			{
				var result = awaiter.GetResult();
				return new[] { result.Item1, result.Item2, result.Item3, result.Item4, result.Item5, result.Item6, result.Item7 };
			}
		}

		private sealed class TaskTupleAwaiter8Adapter : AwaiterAdapter
		{
			private readonly TupleTaskAwaiter<object, object, object, object, object, object, object, object> awaiter;

			public TaskTupleAwaiter8Adapter(TupleTaskAwaiter<object, object, object, object, object, object, object, object> awaiter, string description) : base(description)
			{
				this.awaiter = awaiter;
			}

			public override bool IsCompleted => awaiter.IsCompleted;

			public override void OnCompleted(Action continuation) => awaiter.OnCompleted(continuation);

			public override void UnsafeOnCompleted(Action continuation) => awaiter.UnsafeOnCompleted(continuation);

			public override object[] GetResult()
			{
				var result = awaiter.GetResult();
				return new[] { result.Item1, result.Item2, result.Item3, result.Item4, result.Item5, result.Item6, result.Item7, result.Item8 };
			}
		}

		private sealed class TaskTupleAwaiter9Adapter : AwaiterAdapter
		{
			private readonly TupleTaskAwaiter<object, object, object, object, object, object, object, object, object> awaiter;

			public TaskTupleAwaiter9Adapter(TupleTaskAwaiter<object, object, object, object, object, object, object, object, object> awaiter, string description) : base(description)
			{
				this.awaiter = awaiter;
			}

			public override bool IsCompleted => awaiter.IsCompleted;

			public override void OnCompleted(Action continuation) => awaiter.OnCompleted(continuation);

			public override void UnsafeOnCompleted(Action continuation) => awaiter.UnsafeOnCompleted(continuation);

			public override object[] GetResult()
			{
				var result = awaiter.GetResult();
				return new[] { result.Item1, result.Item2, result.Item3, result.Item4, result.Item5, result.Item6, result.Item7, result.Item8, result.Item9 };
			}
		}

		private sealed class TaskTupleAwaiter10Adapter : AwaiterAdapter
		{
			private readonly TupleTaskAwaiter<object, object, object, object, object, object, object, object, object, object> awaiter;

			public TaskTupleAwaiter10Adapter(TupleTaskAwaiter<object, object, object, object, object, object, object, object, object, object> awaiter, string description) : base(description)
			{
				this.awaiter = awaiter;
			}

			public override bool IsCompleted => awaiter.IsCompleted;

			public override void OnCompleted(Action continuation) => awaiter.OnCompleted(continuation);

			public override void UnsafeOnCompleted(Action continuation) => awaiter.UnsafeOnCompleted(continuation);

			public override object[] GetResult()
			{
				var result = awaiter.GetResult();
				return new[] { result.Item1, result.Item2, result.Item3, result.Item4, result.Item5, result.Item6, result.Item7, result.Item8, result.Item9, result.Item10 };
			}
		}
	}
}
