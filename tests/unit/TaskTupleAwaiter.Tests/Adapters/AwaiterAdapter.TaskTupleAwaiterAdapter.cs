using System.Runtime.CompilerServices;

namespace TaskTupleAwaiter.Tests.Adapters;

partial class AwaiterAdapter
{
	sealed class TaskTupleAwaiter1Adapter(TaskAwaiter<object> awaiter, string description)
		: AwaiterAdapter(description)
	{
		readonly TaskAwaiter<object> _awaiter = awaiter;

		public override bool IsCompleted =>
			_awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			_awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			_awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult() =>
			[_awaiter.GetResult()];
	}

	sealed class TaskTupleAwaiter2Adapter(
		TaskTupleExtensions.TupleTaskAwaiter<object, object> awaiter,
		string description) : AwaiterAdapter(description)
	{
		public override bool IsCompleted =>
			awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult()
		{
			var (item1, item2) = awaiter.GetResult();
			return [item1, item2];
		}
	}

	sealed class TaskTupleAwaiter3Adapter(
		TaskTupleExtensions.TupleTaskAwaiter<object, object, object> awaiter,
		string description) : AwaiterAdapter(description)
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

	sealed class TaskTupleAwaiter4Adapter(
		TaskTupleExtensions.TupleTaskAwaiter<object, object, object, object> awaiter,
		string description) : AwaiterAdapter(description)
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

	sealed class TaskTupleAwaiter5Adapter(
		TaskTupleExtensions.TupleTaskAwaiter<object, object, object, object, object> awaiter,
		string description) : AwaiterAdapter(description)
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

	sealed class TaskTupleAwaiter6Adapter(
		TaskTupleExtensions.TupleTaskAwaiter<object, object, object, object, object, object> awaiter,
		string description) : AwaiterAdapter(description)
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

	sealed class TaskTupleAwaiter7Adapter(
		TaskTupleExtensions.TupleTaskAwaiter<object, object, object, object, object, object, object> awaiter,
		string description) : AwaiterAdapter(description)
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

	sealed class TaskTupleAwaiter8Adapter(
		TaskTupleExtensions.TupleTaskAwaiter<object, object, object, object, object, object, object, object> awaiter,
		string description) : AwaiterAdapter(description)
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

	sealed class TaskTupleAwaiter9Adapter(
		TaskTupleExtensions.TupleTaskAwaiter<object, object, object, object, object, object, object, object, object>
			awaiter,
		string description) : AwaiterAdapter(description)
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

	sealed class TaskTupleAwaiter10Adapter(
		TaskTupleExtensions.TupleTaskAwaiter<object, object, object, object, object, object, object, object, object,
			object> awaiter,
		string description) : AwaiterAdapter(description)
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

	sealed class TaskTupleAwaiter11Adapter(
		TaskTupleExtensions.TupleTaskAwaiter<object, object, object, object, object, object, object, object, object,
			object, object> awaiter,
		string description) : AwaiterAdapter(description)
	{
		public override bool IsCompleted =>
			awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult()
		{
			var (item1, item2, item3, item4, item5, item6, item7, item8, item9, item10, item11) = awaiter.GetResult();
			return [item1, item2, item3, item4, item5, item6, item7, item8, item9, item10, item11];
		}
	}

	sealed class TaskTupleAwaiter12Adapter(
		TaskTupleExtensions.TupleTaskAwaiter<object, object, object, object, object, object, object, object, object,
			object, object, object> awaiter,
		string description) : AwaiterAdapter(description)
	{
		public override bool IsCompleted =>
			awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult()
		{
			var (item1, item2, item3, item4, item5, item6, item7, item8, item9, item10, item11, item12) =
				awaiter.GetResult();
			return [item1, item2, item3, item4, item5, item6, item7, item8, item9, item10, item11, item12];
		}
	}

	sealed class TaskTupleAwaiter13Adapter(
		TaskTupleExtensions.TupleTaskAwaiter<object, object, object, object, object, object, object, object, object,
			object, object, object, object> awaiter,
		string description) : AwaiterAdapter(description)
	{
		public override bool IsCompleted =>
			awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult()
		{
			var (item1, item2, item3, item4, item5, item6, item7, item8, item9, item10, item11, item12, item13) =
				awaiter.GetResult();
			return [item1, item2, item3, item4, item5, item6, item7, item8, item9, item10, item11, item12, item13];
		}
	}

	sealed class TaskTupleAwaiter14Adapter(
		TaskTupleExtensions.TupleTaskAwaiter<object, object, object, object, object, object, object, object, object,
			object, object, object, object, object> awaiter,
		string description) : AwaiterAdapter(description)
	{
		public override bool IsCompleted =>
			awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult()
		{
			var (item1, item2, item3, item4, item5, item6, item7, item8, item9, item10, item11, item12, item13, item14
				) = awaiter.GetResult();
			return
			[
				item1, item2, item3, item4, item5, item6, item7, item8, item9, item10, item11, item12, item13, item14
			];
		}
	}

	sealed class TaskTupleAwaiter15Adapter(
		TaskTupleExtensions.TupleTaskAwaiter<object, object, object, object, object, object, object, object, object,
			object, object, object, object, object, object> awaiter,
		string description) : AwaiterAdapter(description)
	{
		public override bool IsCompleted =>
			awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult()
		{
			var (item1, item2, item3, item4, item5, item6, item7, item8, item9, item10, item11, item12, item13, item14,
				item15) = awaiter.GetResult();
			return
			[
				item1, item2, item3, item4, item5, item6, item7, item8, item9, item10, item11, item12, item13, item14,
				item15
			];
		}
	}

	sealed class TaskTupleAwaiter16Adapter(
		TaskTupleExtensions.TupleTaskAwaiter<object, object, object, object, object, object, object, object, object,
			object, object, object, object, object, object, object> awaiter,
		string description) : AwaiterAdapter(description)
	{
		public override bool IsCompleted =>
			awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult()
		{
			var (item1, item2, item3, item4, item5, item6, item7, item8, item9, item10, item11, item12, item13, item14,
				item15, item16) = awaiter.GetResult();
			return
			[
				item1, item2, item3, item4, item5, item6, item7, item8, item9, item10, item11, item12, item13, item14,
				item15, item16
			];
		}
	}
}
