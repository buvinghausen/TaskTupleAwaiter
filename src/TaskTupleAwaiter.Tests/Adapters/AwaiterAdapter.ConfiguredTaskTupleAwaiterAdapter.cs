using System;
using System.Runtime.CompilerServices;
using static TaskTupleAwaiter.TaskTupleExtensions;

namespace TaskTupleAwaiter.Tests
{
	partial class AwaiterAdapter
	{
		private sealed class ConfiguredTaskTupleAwaiter1Adapter : AwaiterAdapter
		{
			private readonly ConfiguredTaskAwaitable<object>.ConfiguredTaskAwaiter awaiter;

			public ConfiguredTaskTupleAwaiter1Adapter(ConfiguredTaskAwaitable<object>.ConfiguredTaskAwaiter awaiter, string description) : base(description)
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

		private sealed class ConfiguredTaskTupleAwaiter2Adapter : AwaiterAdapter
		{
			private readonly TupleConfiguredTaskAwaitable<object, object>.Awaiter awaiter;

			public ConfiguredTaskTupleAwaiter2Adapter(TupleConfiguredTaskAwaitable<object, object>.Awaiter awaiter, string description) : base(description)
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

		private sealed class ConfiguredTaskTupleAwaiter3Adapter : AwaiterAdapter
		{
			private readonly TupleConfiguredTaskAwaitable<object, object, object>.Awaiter awaiter;

			public ConfiguredTaskTupleAwaiter3Adapter(TupleConfiguredTaskAwaitable<object, object, object>.Awaiter awaiter, string description) : base(description)
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

		private sealed class ConfiguredTaskTupleAwaiter4Adapter : AwaiterAdapter
		{
			private readonly TupleConfiguredTaskAwaitable<object, object, object, object>.Awaiter awaiter;

			public ConfiguredTaskTupleAwaiter4Adapter(TupleConfiguredTaskAwaitable<object, object, object, object>.Awaiter awaiter, string description) : base(description)
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

		private sealed class ConfiguredTaskTupleAwaiter5Adapter : AwaiterAdapter
		{
			private readonly TupleConfiguredTaskAwaitable<object, object, object, object, object>.Awaiter awaiter;

			public ConfiguredTaskTupleAwaiter5Adapter(TupleConfiguredTaskAwaitable<object, object, object, object, object>.Awaiter awaiter, string description) : base(description)
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

		private sealed class ConfiguredTaskTupleAwaiter6Adapter : AwaiterAdapter
		{
			private readonly TupleConfiguredTaskAwaitable<object, object, object, object, object, object>.Awaiter awaiter;

			public ConfiguredTaskTupleAwaiter6Adapter(TupleConfiguredTaskAwaitable<object, object, object, object, object, object>.Awaiter awaiter, string description) : base(description)
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

		private sealed class ConfiguredTaskTupleAwaiter7Adapter : AwaiterAdapter
		{
			private readonly TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object>.Awaiter awaiter;

			public ConfiguredTaskTupleAwaiter7Adapter(TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object>.Awaiter awaiter, string description) : base(description)
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

		private sealed class ConfiguredTaskTupleAwaiter8Adapter : AwaiterAdapter
		{
			private readonly TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object, object>.Awaiter awaiter;

			public ConfiguredTaskTupleAwaiter8Adapter(TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object, object>.Awaiter awaiter, string description) : base(description)
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

		private sealed class ConfiguredTaskTupleAwaiter9Adapter : AwaiterAdapter
		{
			private readonly TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object, object, object>.Awaiter awaiter;

			public ConfiguredTaskTupleAwaiter9Adapter(TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object, object, object>.Awaiter awaiter, string description) : base(description)
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

		private sealed class ConfiguredTaskTupleAwaiter10Adapter : AwaiterAdapter
		{
			private readonly TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object, object, object, object>.Awaiter awaiter;

			public ConfiguredTaskTupleAwaiter10Adapter(TupleConfiguredTaskAwaitable<object, object, object, object, object, object, object, object, object, object>.Awaiter awaiter, string description) : base(description)
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
