using System.Runtime.CompilerServices;
using System.Security;

// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace System.Threading.Tasks;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Ported from jnm2: https://gist.github.com/jnm2/3660db29457d391a34151f764bfe6ef7
/// </summary>
public static class TaskTupleExtensions
{
	#region (Task<T1>)
	/// <summary>
	/// Gets the awaiter for the specified task tuple.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the task.</typeparam>
	/// <param name="tasks">The task tuple.</param>
	/// <returns>The awaiter for the task.</returns>
	public static TaskAwaiter<T1> GetAwaiter<T1>(this ValueTuple<Task<T1>> tasks) =>
		tasks.Item1.GetAwaiter();

	/// <summary>
	/// Configures an awaiter for the specified task tuple.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the task.</typeparam>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="continueOnCapturedContext">A Boolean value that indicates whether to marshal the continuation back to the original context captured.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable<T1> ConfigureAwait<T1>(this ValueTuple<Task<T1>> tasks, bool continueOnCapturedContext) =>
		tasks.Item1.ConfigureAwait(continueOnCapturedContext);

#if NET8_0_OR_GREATER
	/// <summary>
	/// Configures an awaiter for the specified task tuple with the given options.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the task.</typeparam>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="options">The options to configure the awaiter.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable<T1> ConfigureAwait<T1>(this ValueTuple<Task<T1>> tasks, ConfigureAwaitOptions options) =>
		tasks.Item1.ConfigureAwait(options);
#endif
	#endregion

	#region (Task<T1>..Task<T2>)
	/// <summary>
	/// Gets the awaiter for the specified task tuple.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task.</typeparam>
	/// <param name="tasks">The task tuple.</param>
	/// <returns>The awaiter for the task tuple.</returns>
	public static TupleTaskAwaiter<T1, T2> GetAwaiter<T1, T2>(this (Task<T1>, Task<T2>) tasks) =>
		new(tasks);

	/// <summary>
	/// Configures an awaiter for the specified task tuple.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task.</typeparam>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="continueOnCapturedContext">A Boolean value that indicates whether to marshal the continuation back to the original context captured.</param>
	/// <returns>The configured task awaitable.</returns>
	public static TupleConfiguredTaskAwaitable<T1, T2> ConfigureAwait<T1, T2>(this (Task<T1>, Task<T2>) tasks, bool continueOnCapturedContext) =>
		new(tasks, continueOnCapturedContext
#if NET8_0_OR_GREATER
			? ConfigureAwaitOptions.ContinueOnCapturedContext : ConfigureAwaitOptions.None
#endif
		);

#if NET8_0_OR_GREATER
	/// <summary>
	/// Configures an awaiter for the specified task tuple with the given options.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task.</typeparam>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="options">The options to configure the awaiter.</param>
	/// <returns>The configured task awaitable.</returns>
	public static TupleConfiguredTaskAwaitable<T1, T2> ConfigureAwait<T1, T2>(this (Task<T1>, Task<T2>) tasks, ConfigureAwaitOptions options) =>
		new(tasks, options);
#endif

	/// <summary>
	/// Represents a configurable task awaiter for a tuple of two tasks.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task.</typeparam>
	public readonly record struct TupleTaskAwaiter<T1, T2> : ICriticalNotifyCompletion
	{
		private readonly (Task<T1>, Task<T2>) _tasks;
		private readonly TaskAwaiter _whenAllAwaiter;

		internal TupleTaskAwaiter((Task<T1>, Task<T2>) tasks)
		{
			_tasks = tasks;
			_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2).GetAwaiter();
		}

		/// <summary>
		/// Gets a value that indicates whether the awaiter has completed.
		/// </summary>
		public bool IsCompleted => _whenAllAwaiter.IsCompleted;

		/// <summary>
		/// Sets the action to perform when the awaiter completes.
		/// </summary>
		/// <param name="continuation">The action to perform when the awaiter completes.</param>
		public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

		/// <summary>
		/// Sets the action to perform when the awaiter completes.
		/// </summary>
		/// <param name="continuation">The action to perform when the awaiter completes.</param>
		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

		/// <summary>
		/// Gets the result of the completed tasks.
		/// </summary>
		/// <returns>A tuple containing the results of the completed tasks.</returns>
		public (T1, T2) GetResult()
		{
			_whenAllAwaiter.GetResult();
			return (_tasks.Item1.Result, _tasks.Item2.Result);
		}
	}

	/// <summary>
	/// Represents a configured task awaitable for a tuple of two tasks.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task.</typeparam>
	public readonly record struct TupleConfiguredTaskAwaitable<T1, T2>
	{
		private readonly (Task<T1>, Task<T2>) _tasks;
		private readonly
#if NET8_0_OR_GREATER
			ConfigureAwaitOptions
#else
			bool
#endif
				_options;

		internal TupleConfiguredTaskAwaitable((Task<T1>, Task<T2>) tasks,
#if NET8_0_OR_GREATER
			ConfigureAwaitOptions
#else
			bool
#endif
				options)
		{
			_tasks = tasks;
			_options = options;
		}

		/// <summary>
		/// Gets the awaiter for the configured task awaitable.
		/// </summary>
		/// <returns>The awaiter for the configured task awaitable.</returns>
		public Awaiter GetAwaiter() =>
			new(_tasks, _options);

		/// <summary>
		/// Represents an awaiter for a configured task awaitable for a tuple of two tasks.
		/// </summary>
		public readonly record struct Awaiter : ICriticalNotifyCompletion
		{
			private readonly (Task<T1>, Task<T2>) _tasks;
			private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _whenAllAwaiter;

			internal Awaiter((Task<T1>, Task<T2>) tasks,
#if NET8_0_OR_GREATER
				ConfigureAwaitOptions
#else
				bool
#endif
					options)
			{
				_tasks = tasks;
				_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2).ConfigureAwait(options).GetAwaiter();
			}

			/// <summary>
			/// Gets a value that indicates whether the awaiter has completed.
			/// </summary>
			public bool IsCompleted =>
				_whenAllAwaiter.IsCompleted;

			/// <summary>
			/// Sets the action to perform when the awaiter completes.
			/// </summary>
			/// <param name="continuation">The action to perform when the awaiter completes.</param>
			public void OnCompleted(Action continuation) =>
				_whenAllAwaiter.OnCompleted(continuation);

			/// <summary>
			/// Sets the action to perform when the awaiter completes.
			/// </summary>
			/// <param name="continuation">The action to perform when the awaiter completes.</param>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) =>
				_whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>
			/// Gets the result of the completed tasks.
			/// </summary>
			/// <returns>A tuple containing the results of the completed tasks.</returns>
			public (T1, T2) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result);
			}
		}
	}
	#endregion

	#region (Task<T1>..Task<T3>)
	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <param name="tasks"></param>
	/// <returns></returns>
	public static TupleTaskAwaiter<T1, T2, T3> GetAwaiter<T1, T2, T3>(this (Task<T1>, Task<T2>, Task<T3>) tasks) =>
		new(tasks);

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	public readonly record struct TupleTaskAwaiter<T1, T2, T3> : ICriticalNotifyCompletion
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>) _tasks;
		private readonly TaskAwaiter _whenAllAwaiter;

		internal TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>) tasks)
		{
			_tasks = tasks;
			_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3).GetAwaiter();
		}

		/// <summary>
		/// </summary>
		public bool IsCompleted =>
			_whenAllAwaiter.IsCompleted;

		/// <summary>
		/// </summary>
		/// <param name="continuation"></param>
		public void OnCompleted(Action continuation) =>
			_whenAllAwaiter.OnCompleted(continuation);

		/// <summary>
		/// </summary>
		/// <param name="continuation"></param>
		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation) =>
			_whenAllAwaiter.UnsafeOnCompleted(continuation);

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public (T1, T2, T3) GetResult()
		{
			_whenAllAwaiter.GetResult();
			return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result);
		}
	}

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <param name="tasks"></param>
	/// <param name="continueOnCapturedContext"></param>
	/// <returns></returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3> ConfigureAwait<T1, T2, T3>(this (Task<T1>, Task<T2>, Task<T3>) tasks, bool continueOnCapturedContext) =>
		new(tasks, continueOnCapturedContext
#if NET8_0_OR_GREATER
			? ConfigureAwaitOptions.ContinueOnCapturedContext : ConfigureAwaitOptions.None
#endif
		);

#if NET8_0_OR_GREATER
	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <param name="tasks"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3> ConfigureAwait<T1, T2, T3>(this (Task<T1>, Task<T2>, Task<T3>) tasks, ConfigureAwaitOptions options) =>
		new(tasks, options);
#endif

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	public readonly record struct TupleConfiguredTaskAwaitable<T1, T2, T3>
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>) _tasks;
		private readonly
#if NET8_0_OR_GREATER
			ConfigureAwaitOptions
#else
			bool
#endif
				_options;

		internal TupleConfiguredTaskAwaitable((Task<T1>, Task<T2>, Task<T3>) tasks,
#if NET8_0_OR_GREATER
			ConfigureAwaitOptions
#else
			bool
#endif
				options)
		{
			_tasks = tasks;
			_options = options;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public Awaiter GetAwaiter() =>
			new(_tasks, _options);

		/// <summary>
		/// </summary>
		public readonly record struct Awaiter : ICriticalNotifyCompletion
		{
			private readonly (Task<T1>, Task<T2>, Task<T3>) _tasks;
			private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _whenAllAwaiter;

			internal Awaiter((Task<T1>, Task<T2>, Task<T3>) tasks,
#if NET8_0_OR_GREATER
				ConfigureAwaitOptions
#else
				bool
#endif
					options)
			{
				_tasks = tasks;
				_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3).ConfigureAwait(options).GetAwaiter();
			}

			/// <summary>
			/// </summary>
			public bool IsCompleted =>
				_whenAllAwaiter.IsCompleted;

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			public void OnCompleted(Action continuation) =>
				_whenAllAwaiter.OnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) =>
				_whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <returns></returns>
			public (T1, T2, T3) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result);
			}
		}
	}
	#endregion

	#region (Task<T1>..Task<T4>)
	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <param name="tasks"></param>
	/// <returns></returns>
	public static TupleTaskAwaiter<T1, T2, T3, T4> GetAwaiter<T1, T2, T3, T4>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>) tasks) =>
		new(tasks);

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	public readonly record struct TupleTaskAwaiter<T1, T2, T3, T4> : ICriticalNotifyCompletion
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>) _tasks;
		private readonly TaskAwaiter _whenAllAwaiter;

		internal TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>) tasks)
		{
			_tasks = tasks;
			_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4).GetAwaiter();
		}

		/// <summary>
		/// </summary>
		public bool IsCompleted =>
			_whenAllAwaiter.IsCompleted;

		/// <summary>
		/// </summary>
		/// <param name="continuation"></param>
		public void OnCompleted(Action continuation) =>
			_whenAllAwaiter.OnCompleted(continuation);

		/// <summary>
		/// </summary>
		/// <param name="continuation"></param>
		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation) =>
			_whenAllAwaiter.UnsafeOnCompleted(continuation);

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public (T1, T2, T3, T4) GetResult()
		{
			_whenAllAwaiter.GetResult();
			return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result);
		}
	}

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <param name="tasks"></param>
	/// <param name="continueOnCapturedContext"></param>
	/// <returns></returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4> ConfigureAwait<T1, T2, T3, T4>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>) tasks, bool continueOnCapturedContext) =>
		new(tasks, continueOnCapturedContext
#if NET8_0_OR_GREATER
			? ConfigureAwaitOptions.ContinueOnCapturedContext : ConfigureAwaitOptions.None
#endif
		);

#if NET8_0_OR_GREATER
	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <param name="tasks"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4> ConfigureAwait<T1, T2, T3, T4>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>) tasks, ConfigureAwaitOptions options) =>
		new(tasks, options);
#endif

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	public readonly record struct TupleConfiguredTaskAwaitable<T1, T2, T3, T4>
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>) _tasks;
		private readonly
#if NET8_0_OR_GREATER
			ConfigureAwaitOptions
#else
			bool
#endif
			_options;

		internal TupleConfiguredTaskAwaitable((Task<T1>, Task<T2>, Task<T3>, Task<T4>) tasks,
#if NET8_0_OR_GREATER
			ConfigureAwaitOptions
#else
			bool
#endif
		options)
		{
			_tasks = tasks;
			_options = options;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public Awaiter GetAwaiter() =>
			new(_tasks, _options);

		/// <summary>
		/// </summary>
		public readonly record struct Awaiter : ICriticalNotifyCompletion
		{
			private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>) _tasks;
			private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _whenAllAwaiter;

			internal Awaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>) tasks,
#if NET8_0_OR_GREATER
				ConfigureAwaitOptions
#else
				bool
#endif
					options)
			{
				_tasks = tasks;
				_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4).ConfigureAwait(options).GetAwaiter();
			}

			/// <summary>
			/// </summary>
			public bool IsCompleted =>
				_whenAllAwaiter.IsCompleted;

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			public void OnCompleted(Action continuation) =>
				_whenAllAwaiter.OnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) =>
				_whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <returns></returns>
			public (T1, T2, T3, T4) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result);
			}
		}
	}
	#endregion

	#region (Task<T1>..Task<T5>)
	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <param name="tasks"></param>
	/// <returns></returns>
	public static TupleTaskAwaiter<T1, T2, T3, T4, T5> GetAwaiter<T1, T2, T3, T4, T5>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) tasks) =>
		new(tasks);

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	public readonly record struct TupleTaskAwaiter<T1, T2, T3, T4, T5> : ICriticalNotifyCompletion
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) _tasks;
		private readonly TaskAwaiter _whenAllAwaiter;

		internal TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) tasks)
		{
			_tasks = tasks;
			_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5).GetAwaiter();
		}

		/// <summary>
		/// </summary>
		public bool IsCompleted =>
			_whenAllAwaiter.IsCompleted;

		/// <summary>
		/// </summary>
		/// <param name="continuation"></param>
		public void OnCompleted(Action continuation) =>
			_whenAllAwaiter.OnCompleted(continuation);

		/// <summary>
		/// </summary>
		/// <param name="continuation"></param>
		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation) =>
			_whenAllAwaiter.UnsafeOnCompleted(continuation);

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public (T1, T2, T3, T4, T5) GetResult()
		{
			_whenAllAwaiter.GetResult();
			return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result);
		}
	}

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <param name="tasks"></param>
	/// <param name="continueOnCapturedContext"></param>
	/// <returns></returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5> ConfigureAwait<T1, T2, T3, T4, T5>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) tasks, bool continueOnCapturedContext) =>
		new(tasks, continueOnCapturedContext
#if NET8_0_OR_GREATER
			? ConfigureAwaitOptions.ContinueOnCapturedContext : ConfigureAwaitOptions.None
#endif
		);

#if NET8_0_OR_GREATER
	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <param name="tasks"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5> ConfigureAwait<T1, T2, T3, T4, T5>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) tasks, ConfigureAwaitOptions options) =>
		new(tasks, options);
#endif

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	public readonly record struct TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5>
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) _tasks;
		private readonly
#if NET8_0_OR_GREATER
			ConfigureAwaitOptions
#else
			bool
#endif
				_options;

		internal TupleConfiguredTaskAwaitable((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) tasks,
#if NET8_0_OR_GREATER
			ConfigureAwaitOptions
#else
			bool
#endif
				options)
		{
			_tasks = tasks;
			_options = options;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public Awaiter GetAwaiter() =>
			new(_tasks, _options);

		/// <summary>
		/// </summary>
		public readonly record struct Awaiter : ICriticalNotifyCompletion
		{
			private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) _tasks;
			private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _whenAllAwaiter;

			internal Awaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) tasks,
#if NET8_0_OR_GREATER
				ConfigureAwaitOptions
#else
				bool
#endif
					options)
			{
				_tasks = tasks;
				_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5).ConfigureAwait(options).GetAwaiter();
			}

			/// <summary>
			/// </summary>
			public bool IsCompleted =>
				_whenAllAwaiter.IsCompleted;

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			public void OnCompleted(Action continuation) =>
				_whenAllAwaiter.OnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) =>
				_whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <returns></returns>
			public (T1, T2, T3, T4, T5) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result);
			}
		}
	}
	#endregion

	#region (Task<T1>..Task<T6>)
	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <param name="tasks"></param>
	/// <returns></returns>
	public static TupleTaskAwaiter<T1, T2, T3, T4, T5, T6> GetAwaiter<T1, T2, T3, T4, T5, T6>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>) tasks) =>
		new(tasks);

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	public readonly record struct TupleTaskAwaiter<T1, T2, T3, T4, T5, T6> : ICriticalNotifyCompletion
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>) _tasks;
		private readonly TaskAwaiter _whenAllAwaiter;

		internal TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>) tasks)
		{
			_tasks = tasks;
			_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6).GetAwaiter();
		}

		/// <summary>
		/// </summary>
		public bool IsCompleted =>
			_whenAllAwaiter.IsCompleted;

		/// <summary>
		/// </summary>
		/// <param name="continuation"></param>
		public void OnCompleted(Action continuation) =>
			_whenAllAwaiter.OnCompleted(continuation);

		/// <summary>
		/// </summary>
		/// <param name="continuation"></param>
		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation) =>
			_whenAllAwaiter.UnsafeOnCompleted(continuation);

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public (T1, T2, T3, T4, T5, T6) GetResult()
		{
			_whenAllAwaiter.GetResult();
			return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result);
		}
	}

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <param name="tasks"></param>
	/// <param name="continueOnCapturedContext"></param>
	/// <returns></returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6> ConfigureAwait<T1, T2, T3, T4, T5, T6>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>) tasks, bool continueOnCapturedContext) =>
		new(tasks, continueOnCapturedContext
#if NET8_0_OR_GREATER
			? ConfigureAwaitOptions.ContinueOnCapturedContext : ConfigureAwaitOptions.None
#endif
		);

#if NET8_0_OR_GREATER
	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <param name="tasks"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6> ConfigureAwait<T1, T2, T3, T4, T5, T6>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>) tasks, ConfigureAwaitOptions options) =>
		new(tasks, options);
#endif

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	public readonly record struct TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6>
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>) _tasks;
		private readonly
#if NET8_0_OR_GREATER
			ConfigureAwaitOptions
#else
			bool
#endif
				_options;

		internal TupleConfiguredTaskAwaitable((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>) tasks,
#if NET8_0_OR_GREATER
			ConfigureAwaitOptions
#else
			bool
#endif
				options)
		{
			_tasks = tasks;
			_options = options;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public Awaiter GetAwaiter() =>
			new(_tasks, _options);

		/// <summary>
		/// </summary>
		public readonly record struct Awaiter : ICriticalNotifyCompletion
		{
			private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>) _tasks;
			private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _whenAllAwaiter;

			internal Awaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>) tasks,
#if NET8_0_OR_GREATER
				ConfigureAwaitOptions
#else
				bool
#endif
					options)
			{
				_tasks = tasks;
				_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6).ConfigureAwait(options).GetAwaiter();
			}

			/// <summary>
			/// </summary>
			public bool IsCompleted =>
				_whenAllAwaiter.IsCompleted;

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			public void OnCompleted(Action continuation) =>
				_whenAllAwaiter.OnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) =>
				_whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <returns></returns>
			public (T1, T2, T3, T4, T5, T6) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result);
			}
		}
	}
	#endregion

	#region (Task<T1>..Task<T7>)
	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <param name="tasks"></param>
	/// <returns></returns>
	public static TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7> GetAwaiter<T1, T2, T3, T4, T5, T6, T7>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>) tasks) =>
		new(tasks);

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	public readonly record struct TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7> : ICriticalNotifyCompletion
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>) _tasks;
		private readonly TaskAwaiter _whenAllAwaiter;

		internal TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>) tasks)
		{
			_tasks = tasks;
			_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7).GetAwaiter();
		}

		/// <summary>
		/// </summary>
		public bool IsCompleted =>
			_whenAllAwaiter.IsCompleted;

		/// <summary>
		/// </summary>
		/// <param name="continuation"></param>
		public void OnCompleted(Action continuation) =>
			_whenAllAwaiter.OnCompleted(continuation);

		/// <summary>
		/// </summary>
		/// <param name="continuation"></param>
		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation) =>
			_whenAllAwaiter.UnsafeOnCompleted(continuation);

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public (T1, T2, T3, T4, T5, T6, T7) GetResult()
		{
			_whenAllAwaiter.GetResult();
			return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result);
		}
	}

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <param name="tasks"></param>
	/// <param name="continueOnCapturedContext"></param>
	/// <returns></returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>) tasks, bool continueOnCapturedContext) =>
		new(tasks, continueOnCapturedContext
#if NET8_0_OR_GREATER
			? ConfigureAwaitOptions.ContinueOnCapturedContext : ConfigureAwaitOptions.None
#endif
		);

#if NET8_0_OR_GREATER
	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <param name="tasks"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>) tasks, ConfigureAwaitOptions options) =>
		new(tasks, options);
#endif

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	public readonly record struct TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7>
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>) _tasks;
		private readonly
#if NET8_0_OR_GREATER
			ConfigureAwaitOptions
#else
			bool
#endif
				_options;

		internal TupleConfiguredTaskAwaitable((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>) tasks,
#if NET8_0_OR_GREATER
			ConfigureAwaitOptions
#else
			bool
#endif
				options)
		{
			_tasks = tasks;
			_options = options;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public Awaiter GetAwaiter() =>
			new(_tasks, _options);

		/// <summary>
		/// </summary>
		public readonly record struct Awaiter : ICriticalNotifyCompletion
		{
			private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>) _tasks;
			private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _whenAllAwaiter;

			internal Awaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>) tasks,
#if NET8_0_OR_GREATER
				ConfigureAwaitOptions
#else
				bool
#endif
					options)
			{
				_tasks = tasks;
				_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7).ConfigureAwait(options).GetAwaiter();
			}

			/// <summary>
			/// </summary>
			public bool IsCompleted =>
				_whenAllAwaiter.IsCompleted;

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			public void OnCompleted(Action continuation) =>
				_whenAllAwaiter.OnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) =>
				_whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <returns></returns>
			public (T1, T2, T3, T4, T5, T6, T7) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result);
			}
		}
	}
	#endregion

	#region (Task<T1>..Task<T8>)
	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <param name="tasks"></param>
	/// <returns></returns>
	public static TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8> GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>) tasks) =>
		new(tasks);

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	public readonly record struct TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8> : ICriticalNotifyCompletion
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>) _tasks;
		private readonly TaskAwaiter _whenAllAwaiter;

		internal TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>) tasks)
		{
			_tasks = tasks;
			_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8).GetAwaiter();
		}

		/// <summary>
		/// </summary>
		public bool IsCompleted =>
			_whenAllAwaiter.IsCompleted;

		/// <summary>
		/// </summary>
		/// <param name="continuation"></param>
		public void OnCompleted(Action continuation) =>
			_whenAllAwaiter.OnCompleted(continuation);

		/// <summary>
		/// </summary>
		/// <param name="continuation"></param>
		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation) =>
			_whenAllAwaiter.UnsafeOnCompleted(continuation);

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public (T1, T2, T3, T4, T5, T6, T7, T8) GetResult()
		{
			_whenAllAwaiter.GetResult();
			return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result);
		}
	}

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <param name="tasks"></param>
	/// <param name="continueOnCapturedContext"></param>
	/// <returns></returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>) tasks, bool continueOnCapturedContext) =>
		new(tasks, continueOnCapturedContext
#if NET8_0_OR_GREATER
			? ConfigureAwaitOptions.ContinueOnCapturedContext : ConfigureAwaitOptions.None
#endif
		);

#if NET8_0_OR_GREATER
	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <param name="tasks"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>) tasks, ConfigureAwaitOptions options) =>
		new(tasks, options);
#endif

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	public readonly record struct TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8>
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>) _tasks;
		private readonly
#if NET8_0_OR_GREATER
			ConfigureAwaitOptions
#else
			bool
#endif
				_options;

		internal TupleConfiguredTaskAwaitable((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>) tasks,
#if NET8_0_OR_GREATER
			ConfigureAwaitOptions
#else
			bool
#endif
				options)
		{
			_tasks = tasks;
			_options = options;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public Awaiter GetAwaiter() =>
			new(_tasks, _options);

		/// <summary>
		/// </summary>
		public readonly record struct Awaiter : ICriticalNotifyCompletion
		{
			private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>) _tasks;
			private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _whenAllAwaiter;

			internal Awaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>) tasks,
#if NET8_0_OR_GREATER
				ConfigureAwaitOptions
#else
				bool
#endif
					options)
			{
				_tasks = tasks;
				_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8).ConfigureAwait(options).GetAwaiter();
			}

			/// <summary>
			/// </summary>
			public bool IsCompleted =>
				_whenAllAwaiter.IsCompleted;

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			public void OnCompleted(Action continuation) =>
				_whenAllAwaiter.OnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) =>
				_whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <returns></returns>
			public (T1, T2, T3, T4, T5, T6, T7, T8) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result);
			}
		}
	}
	#endregion

	#region (Task<T1>..Task<T9>)
	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <param name="tasks"></param>
	/// <returns></returns>
	public static TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9> GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>) tasks) =>
		new(tasks);

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	public readonly record struct TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9> : ICriticalNotifyCompletion
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>) _tasks;
		private readonly TaskAwaiter _whenAllAwaiter;

		internal TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>) tasks)
		{
			_tasks = tasks;
			_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9).GetAwaiter();
		}

		/// <summary>
		/// </summary>
		public bool IsCompleted =>
			_whenAllAwaiter.IsCompleted;

		/// <summary>
		/// </summary>
		/// <param name="continuation"></param>
		public void OnCompleted(Action continuation) =>
			_whenAllAwaiter.OnCompleted(continuation);

		/// <summary>
		/// </summary>
		/// <param name="continuation"></param>
		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation) =>
			_whenAllAwaiter.UnsafeOnCompleted(continuation);

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public (T1, T2, T3, T4, T5, T6, T7, T8, T9) GetResult()
		{
			_whenAllAwaiter.GetResult();
			return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result, _tasks.Item9.Result);
		}
	}

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <param name="tasks"></param>
	/// <param name="continueOnCapturedContext"></param>
	/// <returns></returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>) tasks, bool continueOnCapturedContext) =>
		new(tasks, continueOnCapturedContext
#if NET8_0_OR_GREATER
			? ConfigureAwaitOptions.ContinueOnCapturedContext : ConfigureAwaitOptions.None
#endif
		);

#if NET8_0_OR_GREATER
	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <param name="tasks"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>) tasks, ConfigureAwaitOptions options) =>
		new(tasks, options);
#endif

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	public readonly record struct TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9>
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>) _tasks;
		private readonly
#if NET8_0_OR_GREATER
			ConfigureAwaitOptions
#else
			bool
#endif
				_options;

		internal TupleConfiguredTaskAwaitable((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>) tasks,
#if NET8_0_OR_GREATER
			ConfigureAwaitOptions
#else
			bool
#endif
				options)
		{
			_tasks = tasks;
			_options = options;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public Awaiter GetAwaiter() =>
			new(_tasks, _options);

		/// <summary>
		/// </summary>
		public readonly record struct Awaiter : ICriticalNotifyCompletion
		{
			private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>) _tasks;
			private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _whenAllAwaiter;

			internal Awaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>) tasks,
#if NET8_0_OR_GREATER
				ConfigureAwaitOptions
#else
				bool
#endif
					options)
			{
				_tasks = tasks;
				_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9).ConfigureAwait(options).GetAwaiter();
			}

			/// <summary>
			/// </summary>
			public bool IsCompleted =>
				_whenAllAwaiter.IsCompleted;

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			public void OnCompleted(Action continuation) =>
				_whenAllAwaiter.OnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) =>
				_whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <returns></returns>
			public (T1, T2, T3, T4, T5, T6, T7, T8, T9) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result, _tasks.Item9.Result);
			}
		}
	}
	#endregion

	#region (Task<T1>..Task<T10>)
	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <param name="tasks"></param>
	/// <returns></returns>
	public static TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>) tasks) =>
		new(tasks);

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	public readonly record struct TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : ICriticalNotifyCompletion
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>) _tasks;
		private readonly TaskAwaiter _whenAllAwaiter;

		internal TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>) tasks)
		{
			_tasks = tasks;
			_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10).GetAwaiter();
		}

		/// <summary>
		/// </summary>
		public bool IsCompleted =>
			_whenAllAwaiter.IsCompleted;

		/// <summary>
		/// </summary>
		/// <param name="continuation"></param>
		public void OnCompleted(Action continuation) =>
			_whenAllAwaiter.OnCompleted(continuation);

		/// <summary>
		/// </summary>
		/// <param name="continuation"></param>
		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation) =>
			_whenAllAwaiter.UnsafeOnCompleted(continuation);

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10) GetResult()
		{
			_whenAllAwaiter.GetResult();
			return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result, _tasks.Item9.Result, _tasks.Item10.Result);
		}
	}

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <param name="tasks"></param>
	/// <param name="continueOnCapturedContext"></param>
	/// <returns></returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>) tasks, bool continueOnCapturedContext) =>
		new(tasks, continueOnCapturedContext
#if NET8_0_OR_GREATER
			? ConfigureAwaitOptions.ContinueOnCapturedContext : ConfigureAwaitOptions.None
#endif
		);

#if NET8_0_OR_GREATER
	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <param name="tasks"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>) tasks, ConfigureAwaitOptions options) =>
		new(tasks, options);
#endif

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	public readonly record struct TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>) _tasks;
		private readonly
#if NET8_0_OR_GREATER
			ConfigureAwaitOptions
#else
			bool
#endif
				_options;

		internal TupleConfiguredTaskAwaitable((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>) tasks,
#if NET8_0_OR_GREATER
			ConfigureAwaitOptions
#else
			bool
#endif
				options)
		{
			_tasks = tasks;
			_options = options;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public Awaiter GetAwaiter() =>
			new(_tasks, _options);

		/// <summary>
		/// </summary>
		public readonly record struct Awaiter : ICriticalNotifyCompletion
		{
			private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>) _tasks;
			private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _whenAllAwaiter;

			internal Awaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>) tasks,
#if NET8_0_OR_GREATER
				ConfigureAwaitOptions
#else
				bool
#endif
					options)
			{
				_tasks = tasks;
				_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10).ConfigureAwait(options).GetAwaiter();
			}

			/// <summary>
			/// </summary>
			public bool IsCompleted =>
				_whenAllAwaiter.IsCompleted;

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			public void OnCompleted(Action continuation) =>
				_whenAllAwaiter.OnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) =>
				_whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <returns></returns>
			public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result, _tasks.Item9.Result, _tasks.Item10.Result);
			}
		}
	}
	#endregion

	#region (Task<T1>..Task<T11>)
	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <typeparam name="T11"></typeparam>
	/// <param name="tasks"></param>
	/// <returns></returns>
	public static TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>) tasks) =>
		new(tasks);

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <typeparam name="T11"></typeparam>
	public readonly record struct TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : ICriticalNotifyCompletion
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>) _tasks;
		private readonly TaskAwaiter _whenAllAwaiter;

		internal TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>) tasks)
		{
			_tasks = tasks;
			_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11).GetAwaiter();
		}

		/// <summary>
		/// </summary>
		public bool IsCompleted =>
			_whenAllAwaiter.IsCompleted;

		/// <summary>
		/// </summary>
		/// <param name="continuation"></param>
		public void OnCompleted(Action continuation) =>
			_whenAllAwaiter.OnCompleted(continuation);

		/// <summary>
		/// </summary>
		/// <param name="continuation"></param>
		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation) =>
			_whenAllAwaiter.UnsafeOnCompleted(continuation);

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11) GetResult()
		{
			_whenAllAwaiter.GetResult();
			return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result, _tasks.Item9.Result, _tasks.Item10.Result, _tasks.Item11.Result);
		}
	}

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <typeparam name="T11"></typeparam>
	/// <param name="tasks"></param>
	/// <param name="continueOnCapturedContext"></param>
	/// <returns></returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>) tasks, bool continueOnCapturedContext) =>
		new(tasks, continueOnCapturedContext
#if NET8_0_OR_GREATER
			? ConfigureAwaitOptions.ContinueOnCapturedContext : ConfigureAwaitOptions.None
#endif
		);

#if NET8_0_OR_GREATER
	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <typeparam name="T11"></typeparam>
	/// <param name="tasks"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>) tasks, ConfigureAwaitOptions options) =>
		new(tasks, options);
#endif

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <typeparam name="T11"></typeparam>
	public readonly record struct TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>) _tasks;
		private readonly
#if NET8_0_OR_GREATER
			ConfigureAwaitOptions
#else
			bool
#endif
				_options;

		internal TupleConfiguredTaskAwaitable((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>) tasks,
#if NET8_0_OR_GREATER
			ConfigureAwaitOptions
#else
			bool
#endif
				options)
		{
			_tasks = tasks;
			_options = options;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public Awaiter GetAwaiter() =>
			new(_tasks, _options);

		/// <summary>
		/// </summary>
		public readonly record struct Awaiter : ICriticalNotifyCompletion
		{
			private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>) _tasks;
			private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _whenAllAwaiter;

			internal Awaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>) tasks,
#if NET8_0_OR_GREATER
				ConfigureAwaitOptions
#else
				bool
#endif
					options)
			{
				_tasks = tasks;
				_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11).ConfigureAwait(options).GetAwaiter();
			}

			/// <summary>
			/// </summary>
			public bool IsCompleted =>
				_whenAllAwaiter.IsCompleted;

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			public void OnCompleted(Action continuation) =>
				_whenAllAwaiter.OnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) =>
				_whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <returns></returns>
			public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result, _tasks.Item9.Result, _tasks.Item10.Result, _tasks.Item11.Result);
			}
		}
	}
	#endregion

	#region (Task<T1>..Task<T12>)
	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <typeparam name="T11"></typeparam>
	/// <typeparam name="T12"></typeparam>
	/// <param name="tasks"></param>
	/// <returns></returns>
	public static TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>) tasks) =>
		new(tasks);

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <typeparam name="T11"></typeparam>
	/// <typeparam name="T12"></typeparam>
	public readonly record struct TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : ICriticalNotifyCompletion
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>) _tasks;
		private readonly TaskAwaiter _whenAllAwaiter;

		internal TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>) tasks)
		{
			_tasks = tasks;
			_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12).GetAwaiter();
		}

		/// <summary>
		/// </summary>
		public bool IsCompleted =>
			_whenAllAwaiter.IsCompleted;

		/// <summary>
		/// </summary>
		/// <param name="continuation"></param>
		public void OnCompleted(Action continuation) =>
			_whenAllAwaiter.OnCompleted(continuation);

		/// <summary>
		/// </summary>
		/// <param name="continuation"></param>
		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation) =>
			_whenAllAwaiter.UnsafeOnCompleted(continuation);

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12) GetResult()
		{
			_whenAllAwaiter.GetResult();
			return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result, _tasks.Item9.Result, _tasks.Item10.Result, _tasks.Item11.Result, _tasks.Item12.Result);
		}
	}

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <typeparam name="T11"></typeparam>
	/// <typeparam name="T12"></typeparam>
	/// <param name="tasks"></param>
	/// <param name="continueOnCapturedContext"></param>
	/// <returns></returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>) tasks, bool continueOnCapturedContext) =>
		new(tasks, continueOnCapturedContext
#if NET8_0_OR_GREATER
			? ConfigureAwaitOptions.ContinueOnCapturedContext : ConfigureAwaitOptions.None
#endif
		);

#if NET8_0_OR_GREATER
	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <typeparam name="T11"></typeparam>
	/// <typeparam name="T12"></typeparam>
	/// <param name="tasks"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>) tasks, ConfigureAwaitOptions options) =>
		new(tasks, options);
#endif

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <typeparam name="T11"></typeparam>
	/// <typeparam name="T12"></typeparam>
	public readonly record struct TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>) _tasks;
		private readonly
#if NET8_0_OR_GREATER
			ConfigureAwaitOptions
#else
			bool
#endif
				_options;

		internal TupleConfiguredTaskAwaitable((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>) tasks,
#if NET8_0_OR_GREATER
			ConfigureAwaitOptions
#else
			bool
#endif
				options)
		{
			_tasks = tasks;
			_options = options;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public Awaiter GetAwaiter() =>
			new(_tasks, _options);

		/// <summary>
		/// </summary>
		public readonly record struct Awaiter : ICriticalNotifyCompletion
		{
			private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>) _tasks;
			private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _whenAllAwaiter;

			internal Awaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>) tasks,
#if NET8_0_OR_GREATER
				ConfigureAwaitOptions
#else
				bool
#endif
					options)
			{
				_tasks = tasks;
				_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12).ConfigureAwait(options).GetAwaiter();
			}

			/// <summary>
			/// </summary>
			public bool IsCompleted =>
				_whenAllAwaiter.IsCompleted;

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			public void OnCompleted(Action continuation) =>
				_whenAllAwaiter.OnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) =>
				_whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <returns></returns>
			public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result, _tasks.Item9.Result, _tasks.Item10.Result, _tasks.Item11.Result, _tasks.Item12.Result);
			}
		}
	}
	#endregion

	#region (Task<T1>..Task<T13>)
	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <typeparam name="T11"></typeparam>
	/// <typeparam name="T12"></typeparam>
	/// <typeparam name="T13"></typeparam>
	/// <param name="tasks"></param>
	/// <returns></returns>
	public static TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>) tasks) =>
		new(tasks);

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <typeparam name="T11"></typeparam>
	/// <typeparam name="T12"></typeparam>
	/// <typeparam name="T13"></typeparam>
	public readonly record struct TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : ICriticalNotifyCompletion
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>) _tasks;
		private readonly TaskAwaiter _whenAllAwaiter;

		internal TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>) tasks)
		{
			_tasks = tasks;
			_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13).GetAwaiter();
		}

		/// <summary>
		/// </summary>
		public bool IsCompleted =>
			_whenAllAwaiter.IsCompleted;

		/// <summary>
		/// </summary>
		/// <param name="continuation"></param>
		public void OnCompleted(Action continuation) =>
			_whenAllAwaiter.OnCompleted(continuation);

		/// <summary>
		/// </summary>
		/// <param name="continuation"></param>
		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation) =>
			_whenAllAwaiter.UnsafeOnCompleted(continuation);

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13) GetResult()
		{
			_whenAllAwaiter.GetResult();
			return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result, _tasks.Item9.Result, _tasks.Item10.Result, _tasks.Item11.Result, _tasks.Item12.Result, _tasks.Item13.Result);
		}
	}

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <typeparam name="T11"></typeparam>
	/// <typeparam name="T12"></typeparam>
	/// <typeparam name="T13"></typeparam>
	/// <param name="tasks"></param>
	/// <param name="continueOnCapturedContext"></param>
	/// <returns></returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>) tasks, bool continueOnCapturedContext) =>
		new(tasks, continueOnCapturedContext
#if NET8_0_OR_GREATER
			? ConfigureAwaitOptions.ContinueOnCapturedContext : ConfigureAwaitOptions.None
#endif
		);

#if NET8_0_OR_GREATER
	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <typeparam name="T11"></typeparam>
	/// <typeparam name="T12"></typeparam>
	/// <typeparam name="T13"></typeparam>
	/// <param name="tasks"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>) tasks, ConfigureAwaitOptions options) =>
		new(tasks, options);
#endif

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <typeparam name="T11"></typeparam>
	/// <typeparam name="T12"></typeparam>
	/// <typeparam name="T13"></typeparam>
	public readonly record struct TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>) _tasks;
		private readonly
#if NET8_0_OR_GREATER
			ConfigureAwaitOptions
#else
			bool
#endif
				_options;

		internal TupleConfiguredTaskAwaitable((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>) tasks,
#if NET8_0_OR_GREATER
			ConfigureAwaitOptions
#else
			bool
#endif
				options)
		{
			_tasks = tasks;
			_options = options;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public Awaiter GetAwaiter() =>
			new(_tasks, _options);

		/// <summary>
		/// </summary>
		public readonly record struct Awaiter : ICriticalNotifyCompletion
		{
			private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>) _tasks;
			private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _whenAllAwaiter;

			internal Awaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>) tasks,
#if NET8_0_OR_GREATER
				ConfigureAwaitOptions
#else
				bool
#endif
					options)
			{
				_tasks = tasks;
				_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13).ConfigureAwait(options).GetAwaiter();
			}

			/// <summary>
			/// </summary>
			public bool IsCompleted =>
				_whenAllAwaiter.IsCompleted;

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			public void OnCompleted(Action continuation) =>
				_whenAllAwaiter.OnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) =>
				_whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <returns></returns>
			public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result, _tasks.Item9.Result, _tasks.Item10.Result, _tasks.Item11.Result, _tasks.Item12.Result, _tasks.Item13.Result);
			}
		}
	}
	#endregion

	#region (Task<T1>..Task<T14>)
	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <typeparam name="T11"></typeparam>
	/// <typeparam name="T12"></typeparam>
	/// <typeparam name="T13"></typeparam>
	/// <typeparam name="T14"></typeparam>
	/// <param name="tasks"></param>
	/// <returns></returns>
	public static TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>) tasks) =>
		new(tasks);

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <typeparam name="T11"></typeparam>
	/// <typeparam name="T12"></typeparam>
	/// <typeparam name="T13"></typeparam>
	/// <typeparam name="T14"></typeparam>
	public readonly record struct TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : ICriticalNotifyCompletion
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>) _tasks;
		private readonly TaskAwaiter _whenAllAwaiter;

		internal TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>) tasks)
		{
			_tasks = tasks;
			_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13, tasks.Item14).GetAwaiter();
		}

		/// <summary>
		/// </summary>
		public bool IsCompleted =>
			_whenAllAwaiter.IsCompleted;

		/// <summary>
		/// </summary>
		/// <param name="continuation"></param>
		public void OnCompleted(Action continuation) =>
			_whenAllAwaiter.OnCompleted(continuation);

		/// <summary>
		/// </summary>
		/// <param name="continuation"></param>
		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation) =>
			_whenAllAwaiter.UnsafeOnCompleted(continuation);

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14) GetResult()
		{
			_whenAllAwaiter.GetResult();
			return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result, _tasks.Item9.Result, _tasks.Item10.Result, _tasks.Item11.Result, _tasks.Item12.Result, _tasks.Item13.Result, _tasks.Item14.Result);
		}
	}

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <typeparam name="T11"></typeparam>
	/// <typeparam name="T12"></typeparam>
	/// <typeparam name="T13"></typeparam>
	/// <typeparam name="T14"></typeparam>
	/// <param name="tasks"></param>
	/// <param name="continueOnCapturedContext"></param>
	/// <returns></returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>) tasks, bool continueOnCapturedContext) =>
		new(tasks, continueOnCapturedContext
#if NET8_0_OR_GREATER
			? ConfigureAwaitOptions.ContinueOnCapturedContext : ConfigureAwaitOptions.None
#endif
		);

#if NET8_0_OR_GREATER
	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <typeparam name="T11"></typeparam>
	/// <typeparam name="T12"></typeparam>
	/// <typeparam name="T13"></typeparam>
	/// <typeparam name="T14"></typeparam>
	/// <param name="tasks"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>) tasks, ConfigureAwaitOptions options) =>
		new(tasks, options);
#endif

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <typeparam name="T11"></typeparam>
	/// <typeparam name="T12"></typeparam>
	/// <typeparam name="T13"></typeparam>
	/// <typeparam name="T14"></typeparam>
	public readonly record struct TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>) _tasks;
		private readonly
#if NET8_0_OR_GREATER
			ConfigureAwaitOptions
#else
			bool
#endif
				_options;

		internal TupleConfiguredTaskAwaitable((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>) tasks,
#if NET8_0_OR_GREATER
			ConfigureAwaitOptions
#else
			bool
#endif
				options)
		{
			_tasks = tasks;
			_options = options;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public Awaiter GetAwaiter() =>
			new(_tasks, _options);

		/// <summary>
		/// </summary>
		public readonly record struct Awaiter : ICriticalNotifyCompletion
		{
			private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>) _tasks;
			private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _whenAllAwaiter;

			internal Awaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>) tasks,
#if NET8_0_OR_GREATER
				ConfigureAwaitOptions
#else
				bool
#endif
					options)
			{
				_tasks = tasks;
				_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13, tasks.Item14).ConfigureAwait(options).GetAwaiter();
			}

			/// <summary>
			/// </summary>
			public bool IsCompleted =>
				_whenAllAwaiter.IsCompleted;

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			public void OnCompleted(Action continuation) =>
				_whenAllAwaiter.OnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) =>
				_whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <returns></returns>
			public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result, _tasks.Item9.Result, _tasks.Item10.Result, _tasks.Item11.Result, _tasks.Item12.Result, _tasks.Item13.Result, _tasks.Item14.Result);
			}
		}
	}
	#endregion

	#region (Task<T1>..Task<T15>)
	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <typeparam name="T11"></typeparam>
	/// <typeparam name="T12"></typeparam>
	/// <typeparam name="T13"></typeparam>
	/// <typeparam name="T14"></typeparam>
	/// <typeparam name="T15"></typeparam>
	/// <param name="tasks"></param>
	/// <returns></returns>
	public static TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>, Task<T15>) tasks) =>
		new(tasks);

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <typeparam name="T11"></typeparam>
	/// <typeparam name="T12"></typeparam>
	/// <typeparam name="T13"></typeparam>
	/// <typeparam name="T14"></typeparam>
	/// <typeparam name="T15"></typeparam>
	public readonly record struct TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : ICriticalNotifyCompletion
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>, Task<T15>) _tasks;
		private readonly TaskAwaiter _whenAllAwaiter;

		internal TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>, Task<T15>) tasks)
		{
			_tasks = tasks;
			_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13, tasks.Item14, tasks.Item15).GetAwaiter();
		}

		/// <summary>
		/// </summary>
		public bool IsCompleted =>
			_whenAllAwaiter.IsCompleted;

		/// <summary>
		/// </summary>
		/// <param name="continuation"></param>
		public void OnCompleted(Action continuation) =>
			_whenAllAwaiter.OnCompleted(continuation);

		/// <summary>
		/// </summary>
		/// <param name="continuation"></param>
		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation) =>
			_whenAllAwaiter.UnsafeOnCompleted(continuation);

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15) GetResult()
		{
			_whenAllAwaiter.GetResult();
			return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result, _tasks.Item9.Result, _tasks.Item10.Result, _tasks.Item11.Result, _tasks.Item12.Result, _tasks.Item13.Result, _tasks.Item14.Result, _tasks.Item15.Result);
		}
	}

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <typeparam name="T11"></typeparam>
	/// <typeparam name="T12"></typeparam>
	/// <typeparam name="T13"></typeparam>
	/// <typeparam name="T14"></typeparam>
	/// <typeparam name="T15"></typeparam>
	/// <param name="tasks"></param>
	/// <param name="continueOnCapturedContext"></param>
	/// <returns></returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>, Task<T15>) tasks, bool continueOnCapturedContext) =>
		new(tasks, continueOnCapturedContext
#if NET8_0_OR_GREATER
			? ConfigureAwaitOptions.ContinueOnCapturedContext : ConfigureAwaitOptions.None
#endif
		);

#if NET8_0_OR_GREATER
	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <typeparam name="T11"></typeparam>
	/// <typeparam name="T12"></typeparam>
	/// <typeparam name="T13"></typeparam>
	/// <typeparam name="T14"></typeparam>
	/// <typeparam name="T15"></typeparam>
	/// <param name="tasks"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>, Task<T15>) tasks, ConfigureAwaitOptions options) =>
		new(tasks, options);
#endif

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <typeparam name="T11"></typeparam>
	/// <typeparam name="T12"></typeparam>
	/// <typeparam name="T13"></typeparam>
	/// <typeparam name="T14"></typeparam>
	/// <typeparam name="T15"></typeparam>
	public readonly record struct TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>, Task<T15>) _tasks;
		private readonly
#if NET8_0_OR_GREATER
			ConfigureAwaitOptions
#else
			bool
#endif
				_options;

		internal TupleConfiguredTaskAwaitable((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>, Task<T15>) tasks,
#if NET8_0_OR_GREATER
			ConfigureAwaitOptions
#else
			bool
#endif
				options)
		{
			_tasks = tasks;
			_options = options;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public Awaiter GetAwaiter() =>
			new(_tasks, _options);

		/// <summary>
		/// </summary>
		public readonly record struct Awaiter : ICriticalNotifyCompletion
		{
			private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>, Task<T15>) _tasks;
			private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _whenAllAwaiter;

			internal Awaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>, Task<T15>) tasks,
#if NET8_0_OR_GREATER
				ConfigureAwaitOptions
#else
				bool
#endif
					options)
			{
				_tasks = tasks;
				_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13, tasks.Item14, tasks.Item15).ConfigureAwait(options).GetAwaiter();
			}

			/// <summary>
			/// </summary>
			public bool IsCompleted =>
				_whenAllAwaiter.IsCompleted;

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			public void OnCompleted(Action continuation) =>
				_whenAllAwaiter.OnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) =>
				_whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <returns></returns>
			public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result, _tasks.Item9.Result, _tasks.Item10.Result, _tasks.Item11.Result, _tasks.Item12.Result, _tasks.Item13.Result, _tasks.Item14.Result, _tasks.Item15.Result);
			}
		}
	}
	#endregion

	#region (Task<T1>..Task<T16>)
	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <typeparam name="T11"></typeparam>
	/// <typeparam name="T12"></typeparam>
	/// <typeparam name="T13"></typeparam>
	/// <typeparam name="T14"></typeparam>
	/// <typeparam name="T15"></typeparam>
	/// <typeparam name="T16"></typeparam>
	/// <param name="tasks"></param>
	/// <returns></returns>
	public static TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>, Task<T15>, Task<T16>) tasks) =>
		new(tasks);

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <typeparam name="T11"></typeparam>
	/// <typeparam name="T12"></typeparam>
	/// <typeparam name="T13"></typeparam>
	/// <typeparam name="T14"></typeparam>
	/// <typeparam name="T15"></typeparam>
	/// <typeparam name="T16"></typeparam>
	public readonly record struct TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> : ICriticalNotifyCompletion
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>, Task<T15>, Task<T16>) _tasks;
		private readonly TaskAwaiter _whenAllAwaiter;

		internal TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>, Task<T15>, Task<T16>) tasks)
		{
			_tasks = tasks;
			_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13, tasks.Item14, tasks.Item15, tasks.Item16).GetAwaiter();
		}

		/// <summary>
		/// </summary>
		public bool IsCompleted =>
			_whenAllAwaiter.IsCompleted;

		/// <summary>
		/// </summary>
		/// <param name="continuation"></param>
		public void OnCompleted(Action continuation) =>
			_whenAllAwaiter.OnCompleted(continuation);

		/// <summary>
		/// </summary>
		/// <param name="continuation"></param>
		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation) =>
			_whenAllAwaiter.UnsafeOnCompleted(continuation);

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16) GetResult()
		{
			_whenAllAwaiter.GetResult();
			return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result, _tasks.Item9.Result, _tasks.Item10.Result, _tasks.Item11.Result, _tasks.Item12.Result, _tasks.Item13.Result, _tasks.Item14.Result, _tasks.Item15.Result, _tasks.Item16.Result);
		}
	}

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <typeparam name="T11"></typeparam>
	/// <typeparam name="T12"></typeparam>
	/// <typeparam name="T13"></typeparam>
	/// <typeparam name="T14"></typeparam>
	/// <typeparam name="T15"></typeparam>
	/// <typeparam name="T16"></typeparam>
	/// <param name="tasks"></param>
	/// <param name="continueOnCapturedContext"></param>
	/// <returns></returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>, Task<T15>, Task<T16>) tasks, bool continueOnCapturedContext) =>
		new(tasks, continueOnCapturedContext
#if NET8_0_OR_GREATER
			? ConfigureAwaitOptions.ContinueOnCapturedContext : ConfigureAwaitOptions.None
#endif
		);

#if NET8_0_OR_GREATER
	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <typeparam name="T11"></typeparam>
	/// <typeparam name="T12"></typeparam>
	/// <typeparam name="T13"></typeparam>
	/// <typeparam name="T14"></typeparam>
	/// <typeparam name="T15"></typeparam>
	/// <typeparam name="T16"></typeparam>
	/// <param name="tasks"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>, Task<T15>, Task<T16>) tasks, ConfigureAwaitOptions options) =>
		new(tasks, options);
#endif

	/// <summary>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="T4"></typeparam>
	/// <typeparam name="T5"></typeparam>
	/// <typeparam name="T6"></typeparam>
	/// <typeparam name="T7"></typeparam>
	/// <typeparam name="T8"></typeparam>
	/// <typeparam name="T9"></typeparam>
	/// <typeparam name="T10"></typeparam>
	/// <typeparam name="T11"></typeparam>
	/// <typeparam name="T12"></typeparam>
	/// <typeparam name="T13"></typeparam>
	/// <typeparam name="T14"></typeparam>
	/// <typeparam name="T15"></typeparam>
	/// <typeparam name="T16"></typeparam>
	public readonly record struct TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>, Task<T15>, Task<T16>) _tasks;
		private readonly
#if NET8_0_OR_GREATER
			ConfigureAwaitOptions
#else
			bool
#endif
				_options;

		internal TupleConfiguredTaskAwaitable((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>, Task<T15>, Task<T16>) tasks,
#if NET8_0_OR_GREATER
			ConfigureAwaitOptions
#else
			bool
#endif
				options)
		{
			_tasks = tasks;
			_options = options;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public Awaiter GetAwaiter() =>
			new(_tasks, _options);

		/// <summary>
		/// </summary>
		public readonly record struct Awaiter : ICriticalNotifyCompletion
		{
			private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>, Task<T15>, Task<T16>) _tasks;
			private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _whenAllAwaiter;

			internal Awaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>, Task<T15>, Task<T16>) tasks,
#if NET8_0_OR_GREATER
				ConfigureAwaitOptions
#else
				bool
#endif
					options)
			{
				_tasks = tasks;
				_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13, tasks.Item14, tasks.Item15, tasks.Item16).ConfigureAwait(options).GetAwaiter();
			}

			/// <summary>
			/// </summary>
			public bool IsCompleted =>
				_whenAllAwaiter.IsCompleted;

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			public void OnCompleted(Action continuation) =>
				_whenAllAwaiter.OnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) =>
				_whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <returns></returns>
			public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result, _tasks.Item9.Result, _tasks.Item10.Result, _tasks.Item11.Result, _tasks.Item12.Result, _tasks.Item13.Result, _tasks.Item14.Result, _tasks.Item15.Result, _tasks.Item16.Result);
			}
		}
	}
	#endregion

	#region Task
	/// <summary>
	/// Gets the awaiter for the specified task tuple.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <returns>The awaiter for the task.</returns>
	public static TaskAwaiter GetAwaiter(this ValueTuple<Task> tasks) =>
		tasks.Item1.GetAwaiter();

	/// <summary>
	/// Configures an awaiter for the specified task tuple.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="continueOnCapturedContext">A Boolean value that indicates whether to marshal the continuation back to the original context captured.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable ConfigureAwait(this ValueTuple<Task> tasks, bool continueOnCapturedContext) =>
		tasks.Item1.ConfigureAwait(continueOnCapturedContext);

	/// <summary>
	/// Gets the awaiter for the specified task tuple.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <returns>The awaiter for the task.</returns>
	public static TaskAwaiter GetAwaiter(this (Task, Task) tasks) =>
		Task.WhenAll(tasks.Item1, tasks.Item2).GetAwaiter();

	/// <summary>
	/// Configures an awaiter for the specified task tuple.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="continueOnCapturedContext">A Boolean value that indicates whether to marshal the continuation back to the original context captured.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task) tasks, bool continueOnCapturedContext) =>
		Task.WhenAll(tasks.Item1, tasks.Item2).ConfigureAwait(continueOnCapturedContext);

	/// <summary>
	/// Gets the awaiter for the specified task tuple.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <returns>The awaiter for the task.</returns>
	public static TaskAwaiter GetAwaiter(this (Task, Task, Task) tasks) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3).GetAwaiter();

	/// <summary>
	/// Gets the awaiter for the specified task tuple.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <returns>The awaiter for the task.</returns>
	public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task) tasks) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4).GetAwaiter();

	/// <summary>
	/// Configures an awaiter for the specified task tuple.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="continueOnCapturedContext">A Boolean value that indicates whether to marshal the continuation back to the original context captured.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task) tasks, bool continueOnCapturedContext) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4).ConfigureAwait(continueOnCapturedContext);

	/// <summary>
	/// Gets the awaiter for the specified task tuple.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <returns>The awaiter for the task.</returns>
	public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task) tasks) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5).GetAwaiter();

	/// <summary>
	/// Configures an awaiter for the specified task tuple.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="continueOnCapturedContext">A Boolean value that indicates whether to marshal the continuation back to the original context captured.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task) tasks, bool continueOnCapturedContext) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5).ConfigureAwait(continueOnCapturedContext);

	/// <summary>
	/// Gets the awaiter for the specified task tuple.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <returns>The awaiter for the task.</returns>
	public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task) tasks) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6).GetAwaiter();

	/// <summary>
	/// Configures an awaiter for the specified task tuple.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="continueOnCapturedContext">A Boolean value that indicates whether to marshal the continuation back to the original context captured.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task) tasks, bool continueOnCapturedContext) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6).ConfigureAwait(continueOnCapturedContext);

	/// <summary>
	/// Gets the awaiter for the specified task tuple.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <returns>The awaiter for the task.</returns>
	public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task, Task) tasks) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7).GetAwaiter();

	/// <summary>
	/// Configures an awaiter for the specified task tuple.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="continueOnCapturedContext">A Boolean value that indicates whether to marshal the continuation back to the original context captured.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task) tasks, bool continueOnCapturedContext) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7).ConfigureAwait(continueOnCapturedContext);

	/// <summary>
	/// Gets the awaiter for the specified task tuple.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <returns>The awaiter for the task.</returns>
	public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task, Task, Task) tasks) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8).GetAwaiter();

	/// <summary>
	/// Configures an awaiter for the specified task tuple.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="continueOnCapturedContext">A Boolean value that indicates whether to marshal the continuation back to the original context captured.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task) tasks, bool continueOnCapturedContext) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8).ConfigureAwait(continueOnCapturedContext);

	/// <summary>
	/// Gets the awaiter for the specified task tuple.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <returns>The awaiter for the task.</returns>
	public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9).GetAwaiter();

	/// <summary>
	/// Configures an awaiter for the specified task tuple.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="continueOnCapturedContext">A Boolean value that indicates whether to marshal the continuation back to the original context captured.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, bool continueOnCapturedContext) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9).ConfigureAwait(continueOnCapturedContext);

	/// <summary>
	/// Gets the awaiter for the specified task tuple.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <returns>The awaiter for the task.</returns>
	public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10).GetAwaiter();

	/// <summary>
	/// Configures an awaiter for the specified task tuple.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="continueOnCapturedContext">A Boolean value that indicates whether to marshal the continuation back to the original context captured.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, bool continueOnCapturedContext) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10).ConfigureAwait(continueOnCapturedContext);

	/// <summary>
	/// Gets the awaiter for the specified task tuple.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <returns>The awaiter for the task.</returns>
	public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11).GetAwaiter();

	/// <summary>
	/// Configures an awaiter for the specified task tuple.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="continueOnCapturedContext">A Boolean value that indicates whether to marshal the continuation back to the original context captured.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, bool continueOnCapturedContext) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11).ConfigureAwait(continueOnCapturedContext);

	/// <summary>
	/// Gets the awaiter for the specified task tuple.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <returns>The awaiter for the task.</returns>
	public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12).GetAwaiter();

	/// <summary>
	/// Configures an awaiter for the specified task tuple.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="continueOnCapturedContext">A Boolean value that indicates whether to marshal the continuation back to the original context captured.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, bool continueOnCapturedContext) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12).ConfigureAwait(continueOnCapturedContext);

	/// <summary>
	/// Gets the awaiter for the specified task tuple.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <returns>The awaiter for the task.</returns>
	public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13).GetAwaiter();

	/// <summary>
	/// Configures an awaiter for the specified task tuple.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="continueOnCapturedContext">A Boolean value that indicates whether to marshal the continuation back to the original context captured.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, bool continueOnCapturedContext) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13).ConfigureAwait(continueOnCapturedContext);

	/// <summary>
	/// Gets the awaiter for the specified task tuple.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <returns>The awaiter for the task.</returns>
	public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13, tasks.Item14).GetAwaiter();

	/// <summary>
	/// Configures an awaiter for the specified task tuple.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="continueOnCapturedContext">A Boolean value that indicates whether to marshal the continuation back to the original context captured.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, bool continueOnCapturedContext) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13, tasks.Item14).ConfigureAwait(continueOnCapturedContext);

	/// <summary>
	/// Gets the awaiter for the specified task tuple.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <returns>The awaiter for the task.</returns>
	public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13, tasks.Item14, tasks.Item15).GetAwaiter();

	/// <summary>
	/// Configures an awaiter for the specified task tuple.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="continueOnCapturedContext">A Boolean value that indicates whether to marshal the continuation back to the original context captured.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, bool continueOnCapturedContext) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13, tasks.Item14, tasks.Item15).ConfigureAwait(continueOnCapturedContext);

	/// <summary>
	/// Gets the awaiter for the specified task tuple.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <returns>The awaiter for the task.</returns>
	public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13, tasks.Item14, tasks.Item15, tasks.Item16).GetAwaiter();

	/// <summary>
	/// Configures an awaiter for the specified task tuple.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="continueOnCapturedContext">A Boolean value that indicates whether to marshal the continuation back to the original context captured.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, bool continueOnCapturedContext) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13, tasks.Item14, tasks.Item15, tasks.Item16).ConfigureAwait(continueOnCapturedContext);

#if NET8_0_OR_GREATER
	/// <summary>
	/// Configures an awaiter for the specified task tuple with the given options.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="options">The options to configure the awaiter.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable ConfigureAwait(this ValueTuple<Task> tasks, ConfigureAwaitOptions options) =>
		tasks.Item1.ConfigureAwait(options);

	/// <summary>
	/// Configures an awaiter for the specified task tuple with the given options.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="options">The options to configure the awaiter.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task) tasks, ConfigureAwaitOptions options) =>
		Task.WhenAll(tasks.Item1, tasks.Item2).ConfigureAwait(options);

	/// <summary>
	/// Configures an awaiter for the specified task tuple with the given options.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="options">The options to configure the awaiter.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task) tasks, ConfigureAwaitOptions options) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3).ConfigureAwait(options);

	/// <summary>
	/// Configures an awaiter for the specified task tuple with the given options.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="options">The options to configure the awaiter.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task) tasks, ConfigureAwaitOptions options) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4).ConfigureAwait(options);

	/// <summary>
	/// Configures an awaiter for the specified task tuple with the given options.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="options">The options to configure the awaiter.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task) tasks, ConfigureAwaitOptions options) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5).ConfigureAwait(options);

	/// <summary>
	/// Configures an awaiter for the specified task tuple with the given options.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="options">The options to configure the awaiter.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task) tasks, ConfigureAwaitOptions options) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6).ConfigureAwait(options);

	/// <summary>
	/// Configures an awaiter for the specified task tuple with the given options.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="options">The options to configure the awaiter.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task) tasks, ConfigureAwaitOptions options) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7).ConfigureAwait(options);

	/// <summary>
	/// Configures an awaiter for the specified task tuple with the given options.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="options">The options to configure the awaiter.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task) tasks, ConfigureAwaitOptions options) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8).ConfigureAwait(options);

	/// <summary>
	/// Configures an awaiter for the specified task tuple with the given options.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="options">The options to configure the awaiter.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, ConfigureAwaitOptions options) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9).ConfigureAwait(options);

	/// <summary>
	/// Configures an awaiter for the specified task tuple with the given options.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="options">The options to configure the awaiter.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, ConfigureAwaitOptions options) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10).ConfigureAwait(options);

	/// <summary>
	/// Configures an awaiter for the specified task tuple with the given options.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="options">The options to configure the awaiter.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, ConfigureAwaitOptions options) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11).ConfigureAwait(options);

	/// <summary>
	/// Configures an awaiter for the specified task tuple with the given options.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="options">The options to configure the awaiter.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, ConfigureAwaitOptions options) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12).ConfigureAwait(options);

	/// <summary>
	/// Configures an awaiter for the specified task tuple with the given options.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="options">The options to configure the awaiter.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, ConfigureAwaitOptions options) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13).ConfigureAwait(options);

	/// <summary>
	/// Configures an awaiter for the specified task tuple with the given options.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="options">The options to configure the awaiter.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, ConfigureAwaitOptions options) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13, tasks.Item14).ConfigureAwait(options);

	/// <summary>
	/// Configures an awaiter for the specified task tuple with the given options.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="options">The options to configure the awaiter.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, ConfigureAwaitOptions options) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13, tasks.Item14, tasks.Item15).ConfigureAwait(options);

	/// <summary>
	/// Configures an awaiter for the specified task tuple with the given options.
	/// </summary>
	/// <param name="tasks">The task tuple.</param>
	/// <param name="options">The options to configure the awaiter.</param>
	/// <returns>The configured task awaitable.</returns>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, ConfigureAwaitOptions options) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13, tasks.Item14, tasks.Item15, tasks.Item16).ConfigureAwait(options);
#endif
	#endregion
}
