using System.Runtime.CompilerServices;

namespace TaskTupleAwaiter.Tests.Adapters;

partial class AwaiterAdapter
{
	sealed class ConfiguredTaskTupleAwaiter1Adapter(
		ConfiguredTaskAwaitable<object>.ConfiguredTaskAwaiter awaiter,
		string description) : AwaiterAdapter(description)
	{
		readonly ConfiguredTaskAwaitable<object>.ConfiguredTaskAwaiter _awaiter = awaiter;

		public override bool IsCompleted =>
			_awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			_awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			_awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult() =>
			[_awaiter.GetResult()];
	}

	sealed class ConfiguredTaskTupleAwaiter2Adapter(
		TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object>.Awaiter awaiter,
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

	sealed class ConfiguredTaskTupleAwaiter3Adapter(
		TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object>.Awaiter awaiter,
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

	sealed class ConfiguredTaskTupleAwaiter4Adapter(
		TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object>.Awaiter awaiter,
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

	sealed class ConfiguredTaskTupleAwaiter5Adapter(
		TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object>.Awaiter awaiter,
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

	sealed class ConfiguredTaskTupleAwaiter6Adapter(
		TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object, object>.Awaiter
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
			var (item1, item2, item3, item4, item5, item6) = awaiter.GetResult();
			return [item1, item2, item3, item4, item5, item6];
		}
	}

	sealed class ConfiguredTaskTupleAwaiter7Adapter(
		TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object>.Awaiter
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
			var (item1, item2, item3, item4, item5, item6, item7) = awaiter.GetResult();
			return [item1, item2, item3, item4, item5, item6, item7];
		}
	}

	sealed class ConfiguredTaskTupleAwaiter8Adapter(
		TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object, object>
			.Awaiter awaiter,
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

	sealed class ConfiguredTaskTupleAwaiter9Adapter(
		TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object, object,
			object>.Awaiter awaiter,
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

	sealed class ConfiguredTaskTupleAwaiter10Adapter(
		TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object, object,
			object, object>.Awaiter awaiter,
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

	sealed class ConfiguredTaskTupleAwaiter11Adapter(
		TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object, object,
			object, object, object>.Awaiter awaiter,
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

	sealed class ConfiguredTaskTupleAwaiter12Adapter(
		TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object, object,
			object, object, object, object>.Awaiter awaiter,
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

	sealed class ConfiguredTaskTupleAwaiter13Adapter(
		TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object, object,
			object, object, object, object, object>.Awaiter awaiter,
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

	sealed class ConfiguredTaskTupleAwaiter14Adapter(
		TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object, object,
			object, object, object, object, object, object>.Awaiter awaiter,
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

	sealed class ConfiguredTaskTupleAwaiter15Adapter(
		TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object, object,
			object, object, object, object, object, object, object>.Awaiter awaiter,
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

	sealed class ConfiguredTaskTupleAwaiter16Adapter(
		TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object, object,
			object, object, object, object, object, object, object, object>.Awaiter awaiter,
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
