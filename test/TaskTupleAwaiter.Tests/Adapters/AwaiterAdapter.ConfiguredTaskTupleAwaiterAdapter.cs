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
			new[] { awaiter.GetResult() };
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
			var result = awaiter.GetResult();
			return [result.Item1, result.Item2];
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
			var result = awaiter.GetResult();
			return
			[
				result.Item1, result.Item2, result.Item3, result.Item4, result.Item5, result.Item6, result.Item7
			];
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
			var result = awaiter.GetResult();
			return
			[
				result.Item1, result.Item2, result.Item3, result.Item4, result.Item5, result.Item6,
				result.Item7, result.Item8
			];
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
			var result = awaiter.GetResult();
			return
			[
				result.Item1, result.Item2, result.Item3, result.Item4, result.Item5, result.Item6,
				result.Item7, result.Item8, result.Item9
			];
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
			var result = awaiter.GetResult();
			return
			[
				result.Item1, result.Item2, result.Item3, result.Item4, result.Item5,
				result.Item6, result.Item7, result.Item8, result.Item9, result.Item10
			];
		}
	}
}
