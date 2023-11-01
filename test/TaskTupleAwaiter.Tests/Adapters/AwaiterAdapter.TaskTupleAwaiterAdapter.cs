using System.Runtime.CompilerServices;

namespace TaskTupleAwaiter.Tests.Adapters;

internal partial class AwaiterAdapter
{
	private sealed class TaskTupleAwaiter1Adapter(TaskAwaiter<object> awaiter, string description) : AwaiterAdapter(description)
	{
		public override bool IsCompleted =>
			awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult() =>
			new[] { awaiter.GetResult() };
	}

	private sealed class TaskTupleAwaiter2Adapter(TaskTupleExtensions.TupleTaskAwaiter<object, object> awaiter, string description) : AwaiterAdapter(description)
	{
		public override bool IsCompleted =>
			awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult()
		{
			var result = awaiter.GetResult();
			return [result.Item1, result.Item2];
		}
	}

	private sealed class TaskTupleAwaiter3Adapter(TaskTupleExtensions.TupleTaskAwaiter<object, object, object> awaiter, string description) : AwaiterAdapter(description)
	{
		public override bool IsCompleted =>
			awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult()
		{
			var (item1, item2, item3) = awaiter.GetResult();
			return [item1, item2, item3];
		}
	}

	private sealed class TaskTupleAwaiter4Adapter(TaskTupleExtensions.TupleTaskAwaiter<object, object, object, object> awaiter, string description) : AwaiterAdapter(description)
	{
		public override bool IsCompleted =>
			awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult()
		{
			var (item1, item2, item3, item4) = awaiter.GetResult();
			return [item1, item2, item3, item4];
		}
	}

	private sealed class TaskTupleAwaiter5Adapter(TaskTupleExtensions.TupleTaskAwaiter<object, object, object, object, object> awaiter, string description) : AwaiterAdapter(description)
	{
		public override bool IsCompleted =>
			awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult()
		{
			var (item1, item2, item3, item4, item5) = awaiter.GetResult();
			return [item1, item2, item3, item4, item5];
		}
	}

	private sealed class TaskTupleAwaiter6Adapter(TaskTupleExtensions.TupleTaskAwaiter<object, object, object, object, object, object> awaiter, string description) : AwaiterAdapter(description)
	{
		public override bool IsCompleted =>
			awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult()
		{
			var (item1, item2, item3, item4, item5, item6) = awaiter.GetResult();
			return [item1, item2, item3, item4, item5, item6];
		}
	}

	private sealed class TaskTupleAwaiter7Adapter(TaskTupleExtensions.TupleTaskAwaiter<object, object, object, object, object, object, object> awaiter, string description) : AwaiterAdapter(description)
	{
		public override bool IsCompleted =>
			awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult()
		{
			var (item1, item2, item3, item4, item5, item6, item7) = awaiter.GetResult();
			return [item1, item2, item3, item4, item5, item6, item7];
		}
	}

	private sealed class TaskTupleAwaiter8Adapter(TaskTupleExtensions.TupleTaskAwaiter<object, object, object, object, object, object, object, object> awaiter, string description) : AwaiterAdapter(description)
	{
		public override bool IsCompleted =>
			awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult()
		{
			var (item1, item2, item3, item4, item5, item6, item7, item8) = awaiter.GetResult();
			return [item1, item2, item3, item4, item5, item6, item7, item8];
		}
	}

	private sealed class TaskTupleAwaiter9Adapter(TaskTupleExtensions.TupleTaskAwaiter<object, object, object, object, object, object, object, object, object> awaiter, string description) : AwaiterAdapter(description)
	{
		public override bool IsCompleted =>
			awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult()
		{
			var (item1, item2, item3, item4, item5, item6, item7, item8, item9) = awaiter.GetResult();
			return [item1, item2, item3, item4, item5, item6, item7, item8, item9];
		}
	}

	private sealed class TaskTupleAwaiter10Adapter(TaskTupleExtensions.TupleTaskAwaiter<object, object, object, object, object, object, object, object, object, object> awaiter, string description) : AwaiterAdapter(description)
	{
		public override bool IsCompleted =>
			awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult()
		{
			var (item1, item2, item3, item4, item5, item6, item7, item8, item9, item10) = awaiter.GetResult();
			return [item1, item2, item3, item4, item5, item6, item7, item8, item9, item10];
		}
	}
}
