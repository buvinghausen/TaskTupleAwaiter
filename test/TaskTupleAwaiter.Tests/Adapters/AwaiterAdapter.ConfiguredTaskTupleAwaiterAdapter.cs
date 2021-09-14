using System.Runtime.CompilerServices;

namespace TaskTupleAwaiter.Tests.Adapters;

partial class AwaiterAdapter
{
	private sealed class ConfiguredTaskTupleAwaiter1Adapter : AwaiterAdapter
	{
		private readonly ConfiguredTaskAwaitable<object>.ConfiguredTaskAwaiter _awaiter;

		public ConfiguredTaskTupleAwaiter1Adapter(ConfiguredTaskAwaitable<object>.ConfiguredTaskAwaiter awaiter, string description) : base(description) =>
			_awaiter = awaiter;

		public override bool IsCompleted =>
			_awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			_awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			_awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult() =>
			new[] { _awaiter.GetResult() };
	}

	private sealed class ConfiguredTaskTupleAwaiter2Adapter : AwaiterAdapter
	{
		private readonly TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object>.Awaiter _awaiter;

		public ConfiguredTaskTupleAwaiter2Adapter(TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object>.Awaiter awaiter, string description) : base(description) =>
			_awaiter = awaiter;

		public override bool IsCompleted =>
			_awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			_awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			_awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult()
		{
			var result = _awaiter.GetResult();
			return new[] { result.Item1, result.Item2 };
		}
	}

	private sealed class ConfiguredTaskTupleAwaiter3Adapter : AwaiterAdapter
	{
		private readonly TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object>.Awaiter _awaiter;

		public ConfiguredTaskTupleAwaiter3Adapter(TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object>.Awaiter awaiter, string description) : base(description) =>
			_awaiter = awaiter;

		public override bool IsCompleted =>
			_awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			_awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			_awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult()
		{
			var (item1, item2, item3) = _awaiter.GetResult();
			return new[] { item1, item2, item3 };
		}
	}

	private sealed class ConfiguredTaskTupleAwaiter4Adapter : AwaiterAdapter
	{
		private readonly TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object>.Awaiter _awaiter;

		public ConfiguredTaskTupleAwaiter4Adapter(TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object>.Awaiter awaiter, string description) : base(description) =>
			_awaiter = awaiter;

		public override bool IsCompleted =>
			_awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			_awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			_awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult()
		{
			var (item1, item2, item3, item4) = _awaiter.GetResult();
			return new[] { item1, item2, item3, item4 };
		}
	}

	private sealed class ConfiguredTaskTupleAwaiter5Adapter : AwaiterAdapter
	{
		private readonly TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object>.Awaiter _awaiter;

		public ConfiguredTaskTupleAwaiter5Adapter(TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object>.Awaiter awaiter, string description) : base(description) =>
			_awaiter = awaiter;

		public override bool IsCompleted =>
			_awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			_awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			_awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult()
		{
			var (item1, item2, item3, item4, item5) = _awaiter.GetResult();
			return new[] { item1, item2, item3, item4, item5 };
		}
	}

	private sealed class ConfiguredTaskTupleAwaiter6Adapter : AwaiterAdapter
	{
		private readonly TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object, object>.Awaiter _awaiter;

		public ConfiguredTaskTupleAwaiter6Adapter(TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object, object>.Awaiter awaiter, string description) : base(description) =>
			_awaiter = awaiter;

		public override bool IsCompleted =>
			_awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			_awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			_awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult()
		{
			var (item1, item2, item3, item4, item5, item6) = _awaiter.GetResult();
			return new[] { item1, item2, item3, item4, item5, item6 };
		}
	}

	private sealed class ConfiguredTaskTupleAwaiter7Adapter : AwaiterAdapter
	{
		private readonly TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object>.Awaiter _awaiter;

		public ConfiguredTaskTupleAwaiter7Adapter(TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object>.Awaiter awaiter, string description) : base(description) =>
			_awaiter = awaiter;

		public override bool IsCompleted =>
			_awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			_awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			_awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult()
		{
			var result = _awaiter.GetResult();
			return new[]
			{
				result.Item1, result.Item2, result.Item3, result.Item4, result.Item5, result.Item6, result.Item7
			};
		}
	}

	private sealed class ConfiguredTaskTupleAwaiter8Adapter : AwaiterAdapter
	{
		private readonly TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object, object>.Awaiter _awaiter;

		public ConfiguredTaskTupleAwaiter8Adapter(TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object, object>.Awaiter awaiter, string description) : base(description) =>
			_awaiter = awaiter;

		public override bool IsCompleted =>
			_awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			_awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			_awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult()
		{
			var result = _awaiter.GetResult();
			return new[]
			{
				result.Item1, result.Item2, result.Item3, result.Item4, result.Item5, result.Item6,
				result.Item7, result.Item8
			};
		}
	}

	private sealed class ConfiguredTaskTupleAwaiter9Adapter : AwaiterAdapter
	{
		private readonly TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object, object, object>.Awaiter _awaiter;

		public ConfiguredTaskTupleAwaiter9Adapter(TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object, object, object>.Awaiter awaiter, string description) : base(description) =>
			_awaiter = awaiter;

		public override bool IsCompleted =>
			_awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			_awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			_awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult()
		{
			var result = _awaiter.GetResult();
			return new[]
			{
				result.Item1, result.Item2, result.Item3, result.Item4, result.Item5, result.Item6,
				result.Item7, result.Item8, result.Item9
			};
		}
	}

	private sealed class ConfiguredTaskTupleAwaiter10Adapter : AwaiterAdapter
	{
		private readonly TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object, object, object, object>.Awaiter _awaiter;

		public ConfiguredTaskTupleAwaiter10Adapter(TaskTupleExtensions.TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object, object, object, object>.Awaiter awaiter, string description) : base(description) =>
			_awaiter = awaiter;

		public override bool IsCompleted =>
			_awaiter.IsCompleted;

		public override void OnCompleted(Action continuation) =>
			_awaiter.OnCompleted(continuation);

		public override void UnsafeOnCompleted(Action continuation) =>
			_awaiter.UnsafeOnCompleted(continuation);

		public override object[] GetResult()
		{
			var result = _awaiter.GetResult();
			return new[]
			{
				result.Item1, result.Item2, result.Item3, result.Item4, result.Item5,
				result.Item6, result.Item7, result.Item8, result.Item9, result.Item10
			};
		}
	}
}
