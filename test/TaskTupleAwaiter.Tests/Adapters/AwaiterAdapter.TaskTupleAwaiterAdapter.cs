using System;
using System.Runtime.CompilerServices;
using static TaskTupleAwaiter.TaskTupleExtensions;

namespace TaskTupleAwaiter.Tests
{
	partial class AwaiterAdapter
	{
		private sealed class TaskTupleAwaiter1Adapter : AwaiterAdapter
		{
			private readonly TaskAwaiter<object> _awaiter;

			public TaskTupleAwaiter1Adapter(TaskAwaiter<object> awaiter, string description) : base(description) => _awaiter = awaiter;

			public override bool IsCompleted => _awaiter.IsCompleted;

			public override void OnCompleted(Action continuation) => _awaiter.OnCompleted(continuation);

			public override void UnsafeOnCompleted(Action continuation) => _awaiter.UnsafeOnCompleted(continuation);

			public override object[] GetResult() => new[] { _awaiter.GetResult() };
		}

		private sealed class TaskTupleAwaiter2Adapter : AwaiterAdapter
		{
			private readonly TupleTaskAwaiter<object, object> _awaiter;

			public TaskTupleAwaiter2Adapter(TupleTaskAwaiter<object, object> awaiter, string description) : base(description) => _awaiter = awaiter;

			public override bool IsCompleted => _awaiter.IsCompleted;

			public override void OnCompleted(Action continuation) => _awaiter.OnCompleted(continuation);

			public override void UnsafeOnCompleted(Action continuation) => _awaiter.UnsafeOnCompleted(continuation);

			public override object[] GetResult()
			{
				var result = _awaiter.GetResult();
				return new[] { result.Item1, result.Item2 };
			}
		}

		private sealed class TaskTupleAwaiter3Adapter : AwaiterAdapter
		{
			private readonly TupleTaskAwaiter<object, object, object> _awaiter;

			public TaskTupleAwaiter3Adapter(TupleTaskAwaiter<object, object, object> awaiter, string description) : base(description) => _awaiter = awaiter;

			public override bool IsCompleted => _awaiter.IsCompleted;

			public override void OnCompleted(Action continuation) => _awaiter.OnCompleted(continuation);

			public override void UnsafeOnCompleted(Action continuation) => _awaiter.UnsafeOnCompleted(continuation);

			public override object[] GetResult()
			{
				var (item1, item2, item3) = _awaiter.GetResult();
				return new[] { item1, item2, item3 };
			}
		}

		private sealed class TaskTupleAwaiter4Adapter : AwaiterAdapter
		{
			private readonly TupleTaskAwaiter<object, object, object, object> _awaiter;

			public TaskTupleAwaiter4Adapter(TupleTaskAwaiter<object, object, object, object> awaiter, string description) : base(description) => _awaiter = awaiter;

			public override bool IsCompleted => _awaiter.IsCompleted;

			public override void OnCompleted(Action continuation) => _awaiter.OnCompleted(continuation);

			public override void UnsafeOnCompleted(Action continuation) => _awaiter.UnsafeOnCompleted(continuation);

			public override object[] GetResult()
			{
				var (item1, item2, item3, item4) = _awaiter.GetResult();
				return new[] { item1, item2, item3, item4 };
			}
		}

		private sealed class TaskTupleAwaiter5Adapter : AwaiterAdapter
		{
			private readonly TupleTaskAwaiter<object, object, object, object, object> _awaiter;

			public TaskTupleAwaiter5Adapter(TupleTaskAwaiter<object, object, object, object, object> awaiter, string description) : base(description) => _awaiter = awaiter;

			public override bool IsCompleted => _awaiter.IsCompleted;

			public override void OnCompleted(Action continuation) => _awaiter.OnCompleted(continuation);

			public override void UnsafeOnCompleted(Action continuation) => _awaiter.UnsafeOnCompleted(continuation);

			public override object[] GetResult()
			{
				var (item1, item2, item3, item4, item5) = _awaiter.GetResult();
				return new[] { item1, item2, item3, item4, item5 };
			}
		}

		private sealed class TaskTupleAwaiter6Adapter : AwaiterAdapter
		{
			private readonly TupleTaskAwaiter<object, object, object, object, object, object> _awaiter;

			public TaskTupleAwaiter6Adapter(TupleTaskAwaiter<object, object, object, object, object, object> awaiter, string description) : base(description) => _awaiter = awaiter;

			public override bool IsCompleted => _awaiter.IsCompleted;

			public override void OnCompleted(Action continuation) => _awaiter.OnCompleted(continuation);

			public override void UnsafeOnCompleted(Action continuation) => _awaiter.UnsafeOnCompleted(continuation);

			public override object[] GetResult()
			{
				var (item1, item2, item3, item4, item5, item6) = _awaiter.GetResult();
				return new[] { item1, item2, item3, item4, item5, item6 };
			}
		}

		private sealed class TaskTupleAwaiter7Adapter : AwaiterAdapter
		{
			private readonly TupleTaskAwaiter<object, object, object, object, object, object, object> _awaiter;

			public TaskTupleAwaiter7Adapter(TupleTaskAwaiter<object, object, object, object, object, object, object> awaiter, string description) : base(description) => _awaiter = awaiter;

			public override bool IsCompleted => _awaiter.IsCompleted;

			public override void OnCompleted(Action continuation) => _awaiter.OnCompleted(continuation);

			public override void UnsafeOnCompleted(Action continuation) => _awaiter.UnsafeOnCompleted(continuation);

			public override object[] GetResult()
			{
				var (item1, item2, item3, item4, item5, item6, item7) = _awaiter.GetResult();
				return new[] { item1, item2, item3, item4, item5, item6, item7 };
			}
		}

		private sealed class TaskTupleAwaiter8Adapter : AwaiterAdapter
		{
			private readonly TupleTaskAwaiter<object, object, object, object, object, object, object, object> _awaiter;

			public TaskTupleAwaiter8Adapter(TupleTaskAwaiter<object, object, object, object, object, object, object, object> awaiter, string description) : base(description) => _awaiter = awaiter;

			public override bool IsCompleted => _awaiter.IsCompleted;

			public override void OnCompleted(Action continuation) => _awaiter.OnCompleted(continuation);

			public override void UnsafeOnCompleted(Action continuation) => _awaiter.UnsafeOnCompleted(continuation);

			public override object[] GetResult()
			{
				var (item1, item2, item3, item4, item5, item6, item7, item8) = _awaiter.GetResult();
				return new[] { item1, item2, item3, item4, item5, item6, item7, item8 };
			}
		}

		private sealed class TaskTupleAwaiter9Adapter : AwaiterAdapter
		{
			private readonly TupleTaskAwaiter<object, object, object, object, object, object, object, object, object> _awaiter;

			public TaskTupleAwaiter9Adapter(TupleTaskAwaiter<object, object, object, object, object, object, object, object, object> awaiter, string description) : base(description) => _awaiter = awaiter;

			public override bool IsCompleted => _awaiter.IsCompleted;

			public override void OnCompleted(Action continuation) => _awaiter.OnCompleted(continuation);

			public override void UnsafeOnCompleted(Action continuation) => _awaiter.UnsafeOnCompleted(continuation);

			public override object[] GetResult()
			{
				var (item1, item2, item3, item4, item5, item6, item7, item8, item9) = _awaiter.GetResult();
				return new[] { item1, item2, item3, item4, item5, item6, item7, item8, item9 };
			}
		}

		private sealed class TaskTupleAwaiter10Adapter : AwaiterAdapter
		{
			private readonly TupleTaskAwaiter<object, object, object, object, object, object, object, object, object, object> _awaiter;

			public TaskTupleAwaiter10Adapter(TupleTaskAwaiter<object, object, object, object, object, object, object, object, object, object> awaiter, string description) : base(description) => _awaiter = awaiter;

			public override bool IsCompleted => _awaiter.IsCompleted;

			public override void OnCompleted(Action continuation) => _awaiter.OnCompleted(continuation);

			public override void UnsafeOnCompleted(Action continuation) => _awaiter.UnsafeOnCompleted(continuation);

			public override object[] GetResult()
			{
				var (item1, item2, item3, item4, item5, item6, item7, item8, item9, item10) = _awaiter.GetResult();
				return new[] { item1, item2, item3, item4, item5, item6, item7, item8, item9, item10 };
			}
		}
	}
}
