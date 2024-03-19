using System.Runtime.CompilerServices;

namespace TaskTupleAwaiter.Tests.Adapters;

internal partial class AwaiterAdapter
{
	private sealed class ConfiguredTaskTupleAwaiter1Adapter(ConfiguredTaskAwaitable<object>.ConfiguredTaskAwaiter awaiter, string description) : AwaiterAdapter(description)
	{
		public override bool IsCompleted =>
			awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult() =>
			[awaiter.GetResult()];
	}

	private sealed class ConfiguredTaskTupleAwaiter2Adapter(TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object>.Awaiter awaiter, string description) : AwaiterAdapter(description)
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

	private sealed class ConfiguredTaskTupleAwaiter3Adapter(TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object>.Awaiter awaiter, string description) : AwaiterAdapter(description)
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

	private sealed class ConfiguredTaskTupleAwaiter4Adapter(TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object>.Awaiter awaiter, string description) : AwaiterAdapter(description)
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

	private sealed class ConfiguredTaskTupleAwaiter5Adapter(TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object>.Awaiter awaiter, string description) : AwaiterAdapter(description)
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

	private sealed class ConfiguredTaskTupleAwaiter6Adapter(TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object, object>.Awaiter awaiter, string description) : AwaiterAdapter(description)
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

	private sealed class ConfiguredTaskTupleAwaiter7Adapter(TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object>.Awaiter awaiter, string description) : AwaiterAdapter(description)
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

	private sealed class ConfiguredTaskTupleAwaiter8Adapter(TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object, object>.Awaiter awaiter, string description) : AwaiterAdapter(description)
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

	private sealed class ConfiguredTaskTupleAwaiter9Adapter(TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object, object, object>.Awaiter awaiter, string description) : AwaiterAdapter(description)
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

	private sealed class ConfiguredTaskTupleAwaiter10Adapter(TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object, object, object, object>.Awaiter awaiter, string description) : AwaiterAdapter(description)
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

	private sealed class ConfiguredTaskTupleAwaiter11Adapter(TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object, object, object, object, object>.Awaiter awaiter, string description) : AwaiterAdapter(description)
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

	private sealed class ConfiguredTaskTupleAwaiter12Adapter(TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object, object, object, object, object, object>.Awaiter awaiter, string description) : AwaiterAdapter(description)
	{
		public override bool IsCompleted =>
			awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult()
		{
			var (item1, item2, item3, item4, item5, item6, item7, item8, item9, item10, item11, item12) = awaiter.GetResult();
			return [item1, item2, item3, item4, item5, item6, item7, item8, item9, item10, item11, item12];
		}
	}

	private sealed class ConfiguredTaskTupleAwaiter13Adapter(TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object, object, object, object, object, object, object>.Awaiter awaiter, string description) : AwaiterAdapter(description)
	{
		public override bool IsCompleted =>
			awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult()
		{
			var (item1, item2, item3, item4, item5, item6, item7, item8, item9, item10, item11, item12, item13) = awaiter.GetResult();
			return [item1, item2, item3, item4, item5, item6, item7, item8, item9, item10, item11, item12, item13];
		}
	}

	private sealed class ConfiguredTaskTupleAwaiter14Adapter(TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object, object, object, object, object, object, object, object>.Awaiter awaiter, string description) : AwaiterAdapter(description)
	{
		public override bool IsCompleted =>
			awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult()
		{
			var (item1, item2, item3, item4, item5, item6, item7, item8, item9, item10, item11, item12, item13, item14) = awaiter.GetResult();
			return [item1, item2, item3, item4, item5, item6, item7, item8, item9, item10, item11, item12, item13, item14];
		}
	}

	private sealed class ConfiguredTaskTupleAwaiter15Adapter(TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>.Awaiter awaiter, string description) : AwaiterAdapter(description)
	{
		public override bool IsCompleted =>
			awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult()
		{
			var (item1, item2, item3, item4, item5, item6, item7, item8, item9, item10, item11, item12, item13, item14, item15) = awaiter.GetResult();
			return [item1, item2, item3, item4, item5, item6, item7, item8, item9, item10, item11, item12, item13, item14, item15];
		}
	}

	private sealed class ConfiguredTaskTupleAwaiter16Adapter(TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>.Awaiter awaiter, string description) : AwaiterAdapter(description)
	{
		public override bool IsCompleted =>
			awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult()
		{
			var (item1, item2, item3, item4, item5, item6, item7, item8, item9, item10, item11, item12, item13, item14, item15, item16) = awaiter.GetResult();
			return [item1, item2, item3, item4, item5, item6, item7, item8, item9, item10, item11, item12, item13, item14, item15, item16];
		}
	}
}
