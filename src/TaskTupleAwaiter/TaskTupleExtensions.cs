using System.Runtime.CompilerServices;
using System.Security;

// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace System.Threading.Tasks;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Provides extension methods for working with tuples of tasks, enabling awaiting and configuring await behavior
/// for multiple tasks in a tuple.
/// </summary>
/// <remarks>
/// This class includes methods to retrieve awaiters and configure await behavior for tuples containing up to
/// sixteen tasks. These methods simplify the process of awaiting multiple tasks and customizing their execution
/// context behavior.
///
/// Ported from jnm2: https://gist.github.com/jnm2/3660db29457d391a34151f764bfe6ef7
/// </remarks>
public static class TaskTupleExtensions
{
	#region (Task<T1>)
	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public static TaskAwaiter<T1> GetAwaiter<T1>(this ValueTuple<Task<T1>> tasks) =>
		tasks.Item1.GetAwaiter();
	
	/// <summary>
	/// Configures an awaiter used to await the specified task tuple.
	/// </summary>
	/// <typeparam name="T1">The type of the result produced by the task in the tuple.</typeparam>
	/// <param name="tasks">A tuple containing a single task to be awaited.</param>
	/// <param name="continueOnCapturedContext">
	/// A Boolean value indicating whether to attempt to marshal the continuation back to the original context captured.
	/// </param>
	/// <returns>
	/// A <see cref="ConfiguredTaskAwaitable{TResult}"/> instance that can be awaited.
	/// </returns>
	/// <remarks>
	/// This method is useful for configuring how the continuation of the task should be executed, 
	/// such as whether it should continue on the captured synchronization context.
	/// </remarks>
	public static ConfiguredTaskAwaitable<T1> ConfigureAwait<T1>(this ValueTuple<Task<T1>> tasks, bool continueOnCapturedContext) =>
		tasks.Item1.ConfigureAwait(continueOnCapturedContext);

#if NET8_0_OR_GREATER
	/// <summary>
	/// Configures an awaiter for the specified tuple of tasks with the provided <see cref="ConfigureAwaitOptions"/>.
	/// </summary>
	/// <typeparam name="T1">The type of the result produced by the task in the tuple.</typeparam>
	/// <param name="tasks">A tuple containing a single task whose awaiter is to be configured.</param>
	/// <param name="options">The <see cref="ConfigureAwaitOptions"/> to use for configuring the awaiter.</param>
	/// <returns>A <see cref="ConfiguredTaskAwaitable{TResult}"/> that can be awaited.</returns>
	/// <remarks>
	/// This method allows you to configure how the continuation of the await operation is executed.
	/// </remarks>
	public static ConfiguredTaskAwaitable<T1> ConfigureAwait<T1>(this ValueTuple<Task<T1>> tasks, ConfigureAwaitOptions options) =>
		tasks.Item1.ConfigureAwait(options);
#endif
	#endregion

	#region (Task<T1>..Task<T2>)
	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public static TupleTaskAwaiter<T1, T2> GetAwaiter<T1, T2>(this (Task<T1>, Task<T2>) tasks) =>
		new(tasks);
	
	/// <summary>
	/// Configures an awaiter for a tuple of two tasks, specifying whether to marshal the continuation back to the original context.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task in the tuple.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task in the tuple.</typeparam>
	/// <param name="tasks">A tuple containing two tasks to be awaited.</param>
	/// <param name="continueOnCapturedContext">
	/// A Boolean value indicating whether to marshal the continuation back to the original context captured.
	/// </param>
	/// <returns>
	/// A <see cref="TupleConfiguredTaskAwaitable{T1, T2}"/> instance that can be awaited.
	/// </returns>
	/// <remarks>
	/// This method allows you to configure the behavior of awaiting a tuple of two tasks, 
	/// enabling control over whether the continuation runs on the captured synchronization context.
	/// </remarks>
	public static TupleConfiguredTaskAwaitable<T1, T2> ConfigureAwait<T1, T2>(this (Task<T1>, Task<T2>) tasks, bool continueOnCapturedContext) =>
		new(tasks, continueOnCapturedContext
#if NET8_0_OR_GREATER
			? ConfigureAwaitOptions.ContinueOnCapturedContext : ConfigureAwaitOptions.None
#endif
		);
	
#if NET8_0_OR_GREATER
	/// <summary>
	/// Configures an awaiter for a tuple of two tasks with the specified <see cref="ConfigureAwaitOptions"/>.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task.</typeparam>
	/// <param name="tasks">A tuple containing two tasks to be awaited.</param>
	/// <param name="options">The <see cref="ConfigureAwaitOptions"/> to use for configuring the awaiter.</param>
	/// <returns>A <see cref="TupleConfiguredTaskAwaitable{T1, T2}"/> that can be awaited.</returns>
	public static TupleConfiguredTaskAwaitable<T1, T2> ConfigureAwait<T1, T2>(this (Task<T1>, Task<T2>) tasks, ConfigureAwaitOptions options) =>
		new(tasks, options);
#endif

	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public readonly record struct TupleTaskAwaiter<T1, T2> : ICriticalNotifyCompletion
	{
		private readonly (Task<T1>, Task<T2>) _tasks;
		private readonly TaskAwaiter _whenAllAwaiter;

		internal TupleTaskAwaiter((Task<T1>, Task<T2>) tasks)
		{
			_tasks = tasks;
			_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2).GetAwaiter();
		}

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public bool IsCompleted => _whenAllAwaiter.IsCompleted;

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public (T1, T2) GetResult()
		{
			_whenAllAwaiter.GetResult();
			return (_tasks.Item1.Result, _tasks.Item2.Result);
		}
	}

	/// <summary>This type and its members are intended for use by the compiler.</summary>
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

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public Awaiter GetAwaiter() =>
			new(_tasks, _options);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
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

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public bool IsCompleted => _whenAllAwaiter.IsCompleted;

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public (T1, T2) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result);
			}
		}
	}
	#endregion

	#region (Task<T1>..Task<T3>)
	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public static TupleTaskAwaiter<T1, T2, T3> GetAwaiter<T1, T2, T3>(this (Task<T1>, Task<T2>, Task<T3>) tasks) =>
		new(tasks);

	/// <summary>
	/// Configures an awaiter for a tuple of three tasks, specifying whether to continue on the captured context.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task.</typeparam>
	/// <typeparam name="T3">The type of the result of the third task.</typeparam>
	/// <param name="tasks">A tuple containing three tasks to be awaited.</param>
	/// <param name="continueOnCapturedContext">
	/// A boolean value indicating whether to attempt to marshal the continuation back to the captured context.
	/// </param>
	/// <returns>
	/// A <see cref="TupleConfiguredTaskAwaitable{T1, T2, T3}"/> instance that can be awaited to retrieve the results of the tasks.
	/// </returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3> ConfigureAwait<T1, T2, T3>(this (Task<T1>, Task<T2>, Task<T3>) tasks, bool continueOnCapturedContext) =>
		new(tasks, continueOnCapturedContext
#if NET8_0_OR_GREATER
				? ConfigureAwaitOptions.ContinueOnCapturedContext : ConfigureAwaitOptions.None
#endif
		);

#if NET8_0_OR_GREATER
	/// <summary>
	/// Configures an awaiter for a tuple of three tasks, specifying how to continue on the captured context.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task.</typeparam>
	/// <typeparam name="T3">The type of the result of the third task.</typeparam>
	/// <param name="tasks">A tuple containing three tasks to be awaited.</param>
	/// <param name="options">
	/// A <see cref="ConfigureAwaitOptions"/> value that specifies how to continue on the captured context.
	/// </param>
	/// <returns>
	/// A <see cref="TupleConfiguredTaskAwaitable{T1, T2, T3}"/> instance that can be awaited.
	/// </returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3> ConfigureAwait<T1, T2, T3>(this (Task<T1>, Task<T2>, Task<T3>) tasks, ConfigureAwaitOptions options) =>
		new(tasks, options);
#endif

	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public readonly record struct TupleTaskAwaiter<T1, T2, T3> : ICriticalNotifyCompletion
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>) _tasks;
		private readonly TaskAwaiter _whenAllAwaiter;

		internal TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>) tasks)
		{
			_tasks = tasks;
			_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3).GetAwaiter();
		}

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public bool IsCompleted => _whenAllAwaiter.IsCompleted;

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public (T1, T2, T3) GetResult()
		{
			_whenAllAwaiter.GetResult();
			return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result);
		}
	}

	/// <summary>This type and its members are intended for use by the compiler.</summary>
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

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public Awaiter GetAwaiter() =>
			new(_tasks, _options);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
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

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public bool IsCompleted => _whenAllAwaiter.IsCompleted;

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public (T1, T2, T3) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result);
			}
		}
	}
	#endregion

	#region (Task<T1>..Task<T4>)
	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public static TupleTaskAwaiter<T1, T2, T3, T4> GetAwaiter<T1, T2, T3, T4>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>) tasks) =>
		new(tasks);

	/// <summary>
	/// Configures an awaitable tuple of four tasks to await their completion with the specified behavior 
	/// regarding the continuation context.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task in the tuple.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task in the tuple.</typeparam>
	/// <typeparam name="T3">The type of the result of the third task in the tuple.</typeparam>
	/// <typeparam name="T4">The type of the result of the fourth task in the tuple.</typeparam>
	/// <param name="tasks">A tuple containing four tasks to be awaited.</param>
	/// <param name="continueOnCapturedContext">
	/// A boolean value indicating whether to attempt to marshal the continuation back to the original context captured.
	/// </param>
	/// <returns>
	/// A <see cref="TupleConfiguredTaskAwaitable{T1, T2, T3, T4}"/> that can be awaited to asynchronously wait for the completion of all tasks in the tuple.
	/// </returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4> ConfigureAwait<T1, T2, T3, T4>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>) tasks, bool continueOnCapturedContext) =>
		new(tasks, continueOnCapturedContext
#if NET8_0_OR_GREATER
				? ConfigureAwaitOptions.ContinueOnCapturedContext : ConfigureAwaitOptions.None
#endif
		);

#if NET8_0_OR_GREATER
	/// <summary>
	/// Configures an awaiter used to await the specified tuple of tasks with the specified options.
	/// </summary>
	/// <typeparam name="T1">The type of the result produced by the first task.</typeparam>
	/// <typeparam name="T2">The type of the result produced by the second task.</typeparam>
	/// <typeparam name="T3">The type of the result produced by the third task.</typeparam>
	/// <typeparam name="T4">The type of the result produced by the fourth task.</typeparam>
	/// <param name="tasks">A tuple containing the tasks to be awaited.</param>
	/// <param name="options">
	/// The <see cref="System.Threading.Tasks.ConfigureAwaitOptions"/> to use for awaiting the tasks.
	/// </param>
	/// <returns>
	/// A <see cref="System.Threading.Tasks.TaskTupleExtensions.TupleConfiguredTaskAwaitable{T1, T2, T3, T4}"/> 
	/// that can be used to await the specified tasks.
	/// </returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4> ConfigureAwait<T1, T2, T3, T4>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>) tasks, ConfigureAwaitOptions options) =>
		new(tasks, options);
#endif

	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public readonly record struct TupleTaskAwaiter<T1, T2, T3, T4> : ICriticalNotifyCompletion
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>) _tasks;
		private readonly TaskAwaiter _whenAllAwaiter;

		internal TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>) tasks)
		{
			_tasks = tasks;
			_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4).GetAwaiter();
		}

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public bool IsCompleted => _whenAllAwaiter.IsCompleted;

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public (T1, T2, T3, T4) GetResult()
		{
			_whenAllAwaiter.GetResult();
			return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result);
		}
	}

	/// <summary>This type and its members are intended for use by the compiler.</summary>
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

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public Awaiter GetAwaiter() =>
			new(_tasks, _options);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
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

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public bool IsCompleted => _whenAllAwaiter.IsCompleted;

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public (T1, T2, T3, T4) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result);
			}
		}
	}
	#endregion

	#region (Task<T1>..Task<T5>)
	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public static TupleTaskAwaiter<T1, T2, T3, T4, T5> GetAwaiter<T1, T2, T3, T4, T5>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) tasks) =>
		new(tasks);

	/// <summary>
	/// Configures an awaiter for a tuple of five tasks, specifying whether to marshal the continuation back to the original context.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task.</typeparam>
	/// <typeparam name="T3">The type of the result of the third task.</typeparam>
	/// <typeparam name="T4">The type of the result of the fourth task.</typeparam>
	/// <typeparam name="T5">The type of the result of the fifth task.</typeparam>
	/// <param name="tasks">A tuple containing the five tasks to be awaited.</param>
	/// <param name="continueOnCapturedContext">
	/// A boolean value indicating whether to marshal the continuation back to the original context.
	/// </param>
	/// <returns>
	/// A <see cref="TupleConfiguredTaskAwaitable{T1, T2, T3, T4, T5}"/> instance that can be awaited.
	/// </returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5> ConfigureAwait<T1, T2, T3, T4, T5>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) tasks, bool continueOnCapturedContext) =>
		new(tasks, continueOnCapturedContext
#if NET8_0_OR_GREATER
				? ConfigureAwaitOptions.ContinueOnCapturedContext : ConfigureAwaitOptions.None
#endif
		);

#if NET8_0_OR_GREATER
	/// <summary>
	/// Configures an awaitable tuple of five tasks to await asynchronously with the specified <see cref="ConfigureAwaitOptions"/>.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task.</typeparam>
	/// <typeparam name="T3">The type of the result of the third task.</typeparam>
	/// <typeparam name="T4">The type of the result of the fourth task.</typeparam>
	/// <typeparam name="T5">The type of the result of the fifth task.</typeparam>
	/// <param name="tasks">The tuple containing the five tasks to be awaited.</param>
	/// <param name="options">The <see cref="ConfigureAwaitOptions"/> to use for configuring the awaitable.</param>
	/// <returns>A <see cref="TupleConfiguredTaskAwaitable{T1, T2, T3, T4, T5}"/> that can be awaited.</returns>
	/// <remarks>
	/// This method allows you to configure how the continuation of the await operation is executed.
	/// </remarks>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5> ConfigureAwait<T1, T2, T3, T4, T5>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) tasks, ConfigureAwaitOptions options) =>
		new(tasks, options);
#endif

	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public readonly record struct TupleTaskAwaiter<T1, T2, T3, T4, T5> : ICriticalNotifyCompletion
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) _tasks;
		private readonly TaskAwaiter _whenAllAwaiter;

		internal TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) tasks)
		{
			_tasks = tasks;
			_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5).GetAwaiter();
		}

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public bool IsCompleted => _whenAllAwaiter.IsCompleted;

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public (T1, T2, T3, T4, T5) GetResult()
		{
			_whenAllAwaiter.GetResult();
			return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result);
		}
	}

	/// <summary>This type and its members are intended for use by the compiler.</summary>
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

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public Awaiter GetAwaiter() =>
			new(_tasks, _options);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
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

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public bool IsCompleted => _whenAllAwaiter.IsCompleted;

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public (T1, T2, T3, T4, T5) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result);
			}
		}
	}
	#endregion

	#region (Task<T1>..Task<T6>)
	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public static TupleTaskAwaiter<T1, T2, T3, T4, T5, T6> GetAwaiter<T1, T2, T3, T4, T5, T6>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>) tasks) =>
		new(tasks);

	/// <summary>
	/// Configures an awaiter for a tuple of six tasks, specifying whether to marshal the continuation back to the original context.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task.</typeparam>
	/// <typeparam name="T3">The type of the result of the third task.</typeparam>
	/// <typeparam name="T4">The type of the result of the fourth task.</typeparam>
	/// <typeparam name="T5">The type of the result of the fifth task.</typeparam>
	/// <typeparam name="T6">The type of the result of the sixth task.</typeparam>
	/// <param name="tasks">A tuple containing the six tasks to be awaited.</param>
	/// <param name="continueOnCapturedContext">
	/// A boolean value indicating whether to marshal the continuation back to the original context.
	/// </param>
	/// <returns>
	/// A <see cref="TupleConfiguredTaskAwaitable{T1, T2, T3, T4, T5, T6}"/> instance that can be awaited.
	/// </returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6> ConfigureAwait<T1, T2, T3, T4, T5, T6>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>) tasks, bool continueOnCapturedContext) =>
		new(tasks, continueOnCapturedContext
#if NET8_0_OR_GREATER
				? ConfigureAwaitOptions.ContinueOnCapturedContext : ConfigureAwaitOptions.None
#endif
		);

#if NET8_0_OR_GREATER
	/// <summary>
	/// Configures an awaitable tuple of six tasks to await asynchronously with the specified <see cref="ConfigureAwaitOptions"/>.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task.</typeparam>
	/// <typeparam name="T3">The type of the result of the third task.</typeparam>
	/// <typeparam name="T4">The type of the result of the fourth task.</typeparam>
	/// <typeparam name="T5">The type of the result of the fifth task.</typeparam>
	/// <typeparam name="T6">The type of the result of the sixth task.</typeparam>
	/// <param name="tasks">The tuple containing the six tasks to be awaited.</param>
	/// <param name="options">The <see cref="ConfigureAwaitOptions"/> to use for configuring the awaitable.</param>
	/// <returns>A <see cref="TupleConfiguredTaskAwaitable{T1, T2, T3, T4, T5, T6}"/> that can be awaited.</returns>
	/// <remarks>
	/// This method allows you to configure how the continuation of the await operation is executed.
	/// </remarks>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6> ConfigureAwait<T1, T2, T3, T4, T5, T6>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>) tasks, ConfigureAwaitOptions options) =>
		new(tasks, options);
#endif

	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public readonly record struct TupleTaskAwaiter<T1, T2, T3, T4, T5, T6> : ICriticalNotifyCompletion
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>) _tasks;
		private readonly TaskAwaiter _whenAllAwaiter;

		internal TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>) tasks)
		{
			_tasks = tasks;
			_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6).GetAwaiter();
		}

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public bool IsCompleted => _whenAllAwaiter.IsCompleted;

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public (T1, T2, T3, T4, T5, T6) GetResult()
		{
			_whenAllAwaiter.GetResult();
			return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result);
		}
	}

	/// <summary>This type and its members are intended for use by the compiler.</summary>
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

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public Awaiter GetAwaiter() =>
			new(_tasks, _options);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
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

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public bool IsCompleted => _whenAllAwaiter.IsCompleted;

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public (T1, T2, T3, T4, T5, T6) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result);
			}
		}
	}
	#endregion

	#region (Task<T1>..Task<T7>)
	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public static TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7> GetAwaiter<T1, T2, T3, T4, T5, T6, T7>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>) tasks) =>
		new(tasks);

	/// <summary>
	/// Configures an awaiter for a tuple of seven tasks, specifying whether to marshal the continuation back to the original context.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task.</typeparam>
	/// <typeparam name="T3">The type of the result of the third task.</typeparam>
	/// <typeparam name="T4">The type of the result of the fourth task.</typeparam>
	/// <typeparam name="T5">The type of the result of the fifth task.</typeparam>
	/// <typeparam name="T6">The type of the result of the sixth task.</typeparam>
	/// <typeparam name="T7">The type of the result of the seventh task.</typeparam>
	/// <param name="tasks">A tuple containing the seven tasks to be awaited.</param>
	/// <param name="continueOnCapturedContext">
	/// A boolean value indicating whether to marshal the continuation back to the original context.
	/// </param>
	/// <returns>
	/// A <see cref="TupleConfiguredTaskAwaitable{T1, T2, T3, T4, T5, T6, T7}"/> instance that can be awaited.
	/// </returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>) tasks, bool continueOnCapturedContext) =>
		new(tasks, continueOnCapturedContext
#if NET8_0_OR_GREATER
				? ConfigureAwaitOptions.ContinueOnCapturedContext : ConfigureAwaitOptions.None
#endif
		);

#if NET8_0_OR_GREATER
	/// <summary>
	/// Configures an awaitable tuple of seven tasks to await asynchronously with the specified <see cref="ConfigureAwaitOptions"/>.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task.</typeparam>
	/// <typeparam name="T3">The type of the result of the third task.</typeparam>
	/// <typeparam name="T4">The type of the result of the fourth task.</typeparam>
	/// <typeparam name="T5">The type of the result of the fifth task.</typeparam>
	/// <typeparam name="T6">The type of the result of the sixth task.</typeparam>
	/// <typeparam name="T7">The type of the result of the seventh task.</typeparam>
	/// <param name="tasks">The tuple containing the seven tasks to be awaited.</param>
	/// <param name="options">The <see cref="ConfigureAwaitOptions"/> to use for configuring the awaitable.</param>
	/// <returns>A <see cref="TupleConfiguredTaskAwaitable{T1, T2, T3, T4, T5, T6, T7}"/> that can be awaited.</returns>
	/// <remarks>
	/// This method allows you to configure how the continuation of the await operation is executed.
	/// </remarks>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>) tasks, ConfigureAwaitOptions options) =>
		new(tasks, options);
#endif

	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public readonly record struct TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7> : ICriticalNotifyCompletion
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>) _tasks;
		private readonly TaskAwaiter _whenAllAwaiter;

		internal TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>) tasks)
		{
			_tasks = tasks;
			_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7).GetAwaiter();
		}

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public bool IsCompleted => _whenAllAwaiter.IsCompleted;

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public (T1, T2, T3, T4, T5, T6, T7) GetResult()
		{
			_whenAllAwaiter.GetResult();
			return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result);
		}
	}

	/// <summary>This type and its members are intended for use by the compiler.</summary>
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

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public Awaiter GetAwaiter() =>
			new(_tasks, _options);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
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

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public bool IsCompleted => _whenAllAwaiter.IsCompleted;

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public (T1, T2, T3, T4, T5, T6, T7) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result);
			}
		}
	}
	#endregion

	#region (Task<T1>..Task<T8>)
	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public static TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8> GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>) tasks) =>
		new(tasks);

	/// <summary>
	/// Configures an awaiter for a tuple of eight tasks, specifying whether to marshal the continuation back to the original context.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task.</typeparam>
	/// <typeparam name="T3">The type of the result of the third task.</typeparam>
	/// <typeparam name="T4">The type of the result of the fourth task.</typeparam>
	/// <typeparam name="T5">The type of the result of the fifth task.</typeparam>
	/// <typeparam name="T6">The type of the result of the sixth task.</typeparam>
	/// <typeparam name="T7">The type of the result of the seventh task.</typeparam>
	/// <typeparam name="T8">The type of the result of the eighth task.</typeparam>
	/// <param name="tasks">A tuple containing the eight tasks to be awaited.</param>
	/// <param name="continueOnCapturedContext">
	/// A boolean value indicating whether to marshal the continuation back to the original context.
	/// </param>
	/// <returns>
	/// A <see cref="TupleConfiguredTaskAwaitable{T1, T2, T3, T4, T5, T6, T7, T8}"/> instance that can be awaited.
	/// </returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>) tasks, bool continueOnCapturedContext) =>
		new(tasks, continueOnCapturedContext
#if NET8_0_OR_GREATER
				? ConfigureAwaitOptions.ContinueOnCapturedContext : ConfigureAwaitOptions.None
#endif
		);

#if NET8_0_OR_GREATER
	/// <summary>
	/// Configures an awaitable tuple of eight tasks to await asynchronously with the specified <see cref="ConfigureAwaitOptions"/>.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task.</typeparam>
	/// <typeparam name="T3">The type of the result of the third task.</typeparam>
	/// <typeparam name="T4">The type of the result of the fourth task.</typeparam>
	/// <typeparam name="T5">The type of the result of the fifth task.</typeparam>
	/// <typeparam name="T6">The type of the result of the sixth task.</typeparam>
	/// <typeparam name="T7">The type of the result of the seventh task.</typeparam>
	/// <typeparam name="T8">The type of the result of the eighth task.</typeparam>
	/// <param name="tasks">The tuple containing the eight tasks to be awaited.</param>
	/// <param name="options">The <see cref="ConfigureAwaitOptions"/> to use for configuring the awaitable.</param>
	/// <returns>A <see cref="TupleConfiguredTaskAwaitable{T1, T2, T3, T4, T5, T6, T7, T8}"/> that can be awaited.</returns>
	/// <remarks>
	/// This method allows you to configure how the continuation of the await operation is executed.
	/// </remarks>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>) tasks, ConfigureAwaitOptions options) =>
		new(tasks, options);
#endif

	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public readonly record struct TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8> : ICriticalNotifyCompletion
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>) _tasks;
		private readonly TaskAwaiter _whenAllAwaiter;

		internal TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>) tasks)
		{
			_tasks = tasks;
			_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8).GetAwaiter();
		}

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public bool IsCompleted => _whenAllAwaiter.IsCompleted;

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public (T1, T2, T3, T4, T5, T6, T7, T8) GetResult()
		{
			_whenAllAwaiter.GetResult();
			return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result);
		}
	}

	/// <summary>This type and its members are intended for use by the compiler.</summary>
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

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public Awaiter GetAwaiter() =>
			new(_tasks, _options);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
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

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public bool IsCompleted => _whenAllAwaiter.IsCompleted;

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public (T1, T2, T3, T4, T5, T6, T7, T8) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result);
			}
		}
	}
	#endregion

	#region (Task<T1>..Task<T9>)
	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public static TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9> GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>) tasks) =>
		new(tasks);

	/// <summary>
	/// Configures an awaiter for a tuple of nine tasks, specifying whether to marshal the continuation back to the original context.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task.</typeparam>
	/// <typeparam name="T3">The type of the result of the third task.</typeparam>
	/// <typeparam name="T4">The type of the result of the fourth task.</typeparam>
	/// <typeparam name="T5">The type of the result of the fifth task.</typeparam>
	/// <typeparam name="T6">The type of the result of the sixth task.</typeparam>
	/// <typeparam name="T7">The type of the result of the seventh task.</typeparam>
	/// <typeparam name="T8">The type of the result of the eighth task.</typeparam>
	/// <typeparam name="T9">The type of the result of the ninth task.</typeparam>
	/// <param name="tasks">A tuple containing the nine tasks to be awaited.</param>
	/// <param name="continueOnCapturedContext">
	/// A boolean value indicating whether to marshal the continuation back to the original context.
	/// </param>
	/// <returns>
	/// A <see cref="TupleConfiguredTaskAwaitable{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> instance that can be awaited.
	/// </returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>) tasks, bool continueOnCapturedContext) =>
		new(tasks, continueOnCapturedContext
#if NET8_0_OR_GREATER
				? ConfigureAwaitOptions.ContinueOnCapturedContext : ConfigureAwaitOptions.None
#endif
		);

#if NET8_0_OR_GREATER
	/// <summary>
	/// Configures an awaitable tuple of nine tasks to await asynchronously with the specified <see cref="ConfigureAwaitOptions"/>.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task.</typeparam>
	/// <typeparam name="T3">The type of the result of the third task.</typeparam>
	/// <typeparam name="T4">The type of the result of the fourth task.</typeparam>
	/// <typeparam name="T5">The type of the result of the fifth task.</typeparam>
	/// <typeparam name="T6">The type of the result of the sixth task.</typeparam>
	/// <typeparam name="T7">The type of the result of the seventh task.</typeparam>
	/// <typeparam name="T8">The type of the result of the eighth task.</typeparam>
	/// <typeparam name="T9">The type of the result of the ninth task.</typeparam>
	/// <param name="tasks">The tuple containing the nine tasks to be awaited.</param>
	/// <param name="options">The <see cref="ConfigureAwaitOptions"/> to use for configuring the awaitable.</param>
	/// <returns>A <see cref="TupleConfiguredTaskAwaitable{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> that can be awaited.</returns>
	/// <remarks>
	/// This method allows you to configure how the continuation of the await operation is executed.
	/// </remarks>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>) tasks, ConfigureAwaitOptions options) =>
		new(tasks, options);
#endif

	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public readonly record struct TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9> : ICriticalNotifyCompletion
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>) _tasks;
		private readonly TaskAwaiter _whenAllAwaiter;

		internal TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>) tasks)
		{
			_tasks = tasks;
			_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9).GetAwaiter();
		}

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public bool IsCompleted => _whenAllAwaiter.IsCompleted;

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public (T1, T2, T3, T4, T5, T6, T7, T8, T9) GetResult()
		{
			_whenAllAwaiter.GetResult();
			return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result, _tasks.Item9.Result);
		}
	}

	/// <summary>This type and its members are intended for use by the compiler.</summary>
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

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public Awaiter GetAwaiter() =>
			new(_tasks, _options);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
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

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public bool IsCompleted => _whenAllAwaiter.IsCompleted;

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public (T1, T2, T3, T4, T5, T6, T7, T8, T9) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result, _tasks.Item9.Result);
			}
		}
	}
	#endregion

	#region (Task<T1>..Task<T10>)
	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public static TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>) tasks) =>
		new(tasks);

	/// <summary>
	/// Configures an awaiter for a tuple of ten tasks, specifying whether to marshal the continuation back to the original context.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task.</typeparam>
	/// <typeparam name="T3">The type of the result of the third task.</typeparam>
	/// <typeparam name="T4">The type of the result of the fourth task.</typeparam>
	/// <typeparam name="T5">The type of the result of the fifth task.</typeparam>
	/// <typeparam name="T6">The type of the result of the sixth task.</typeparam>
	/// <typeparam name="T7">The type of the result of the seventh task.</typeparam>
	/// <typeparam name="T8">The type of the result of the eighth task.</typeparam>
	/// <typeparam name="T9">The type of the result of the ninth task.</typeparam>
	/// <typeparam name="T10">The type of the result of the tenth task.</typeparam>
	/// <param name="tasks">A tuple containing the ten tasks to be awaited.</param>
	/// <param name="continueOnCapturedContext">
	/// A boolean value indicating whether to marshal the continuation back to the original context.
	/// </param>
	/// <returns>
	/// A <see cref="TupleConfiguredTaskAwaitable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10}"/> instance that can be awaited.
	/// </returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>) tasks, bool continueOnCapturedContext) =>
		new(tasks, continueOnCapturedContext
#if NET8_0_OR_GREATER
				? ConfigureAwaitOptions.ContinueOnCapturedContext : ConfigureAwaitOptions.None
#endif
		);

#if NET8_0_OR_GREATER
	/// <summary>
	/// Configures an awaitable tuple of ten tasks to await asynchronously with the specified <see cref="ConfigureAwaitOptions"/>.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task.</typeparam>
	/// <typeparam name="T3">The type of the result of the third task.</typeparam>
	/// <typeparam name="T4">The type of the result of the fourth task.</typeparam>
	/// <typeparam name="T5">The type of the result of the fifth task.</typeparam>
	/// <typeparam name="T6">The type of the result of the sixth task.</typeparam>
	/// <typeparam name="T7">The type of the result of the seventh task.</typeparam>
	/// <typeparam name="T8">The type of the result of the eighth task.</typeparam>
	/// <typeparam name="T9">The type of the result of the ninth task.</typeparam>
	/// <typeparam name="T10">The type of the result of the tenth task.</typeparam>
	/// <param name="tasks">The tuple containing the ten tasks to be awaited.</param>
	/// <param name="options">The <see cref="ConfigureAwaitOptions"/> to use for configuring the awaitable.</param>
	/// <returns>A <see cref="TupleConfiguredTaskAwaitable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10}"/> that can be awaited.</returns>
	/// <remarks>
	/// This method allows you to configure how the continuation of the await operation is executed.
	/// </remarks>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>) tasks, ConfigureAwaitOptions options) =>
		new(tasks, options);
#endif

	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public readonly record struct TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : ICriticalNotifyCompletion
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>) _tasks;
		private readonly TaskAwaiter _whenAllAwaiter;

		internal TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>) tasks)
		{
			_tasks = tasks;
			_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10).GetAwaiter();
		}

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public bool IsCompleted => _whenAllAwaiter.IsCompleted;

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10) GetResult()
		{
			_whenAllAwaiter.GetResult();
			return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result, _tasks.Item9.Result, _tasks.Item10.Result);
		}
	}

	/// <summary>This type and its members are intended for use by the compiler.</summary>
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

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public Awaiter GetAwaiter() =>
			new(_tasks, _options);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
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

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public bool IsCompleted => _whenAllAwaiter.IsCompleted;

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result, _tasks.Item9.Result, _tasks.Item10.Result);
			}
		}
	}
	#endregion

	#region (Task<T1>..Task<T11>)
	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public static TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>) tasks) =>
		new(tasks);

	/// <summary>
	/// Configures an awaiter for a tuple of eleven tasks, specifying whether to marshal the continuation back to the original context.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task.</typeparam>
	/// <typeparam name="T3">The type of the result of the third task.</typeparam>
	/// <typeparam name="T4">The type of the result of the fourth task.</typeparam>
	/// <typeparam name="T5">The type of the result of the fifth task.</typeparam>
	/// <typeparam name="T6">The type of the result of the sixth task.</typeparam>
	/// <typeparam name="T7">The type of the result of the seventh task.</typeparam>
	/// <typeparam name="T8">The type of the result of the eighth task.</typeparam>
	/// <typeparam name="T9">The type of the result of the ninth task.</typeparam>
	/// <typeparam name="T10">The type of the result of the tenth task.</typeparam>
	/// <typeparam name="T11">The type of the result of the eleventh task.</typeparam>
	/// <param name="tasks">A tuple containing the eleven tasks to be awaited.</param>
	/// <param name="continueOnCapturedContext">
	/// A boolean value indicating whether to marshal the continuation back to the original context.
	/// </param>
	/// <returns>
	/// A <see cref="TupleConfiguredTaskAwaitable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11}"/> instance that can be awaited.
	/// </returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>) tasks, bool continueOnCapturedContext) =>
		new(tasks, continueOnCapturedContext
#if NET8_0_OR_GREATER
				? ConfigureAwaitOptions.ContinueOnCapturedContext : ConfigureAwaitOptions.None
#endif
		);

#if NET8_0_OR_GREATER
	/// <summary>
	/// Configures an awaitable tuple of eleven tasks to await asynchronously with the specified <see cref="ConfigureAwaitOptions"/>.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task.</typeparam>
	/// <typeparam name="T3">The type of the result of the third task.</typeparam>
	/// <typeparam name="T4">The type of the result of the fourth task.</typeparam>
	/// <typeparam name="T5">The type of the result of the fifth task.</typeparam>
	/// <typeparam name="T6">The type of the result of the sixth task.</typeparam>
	/// <typeparam name="T7">The type of the result of the seventh task.</typeparam>
	/// <typeparam name="T8">The type of the result of the eighth task.</typeparam>
	/// <typeparam name="T9">The type of the result of the ninth task.</typeparam>
	/// <typeparam name="T10">The type of the result of the tenth task.</typeparam>
	/// <typeparam name="T11">The type of the result of the eleventh task.</typeparam>
	/// <param name="tasks">The tuple containing the eleven tasks to be awaited.</param>
	/// <param name="options">The <see cref="ConfigureAwaitOptions"/> to use for configuring the awaitable.</param>
	/// <returns>A <see cref="TupleConfiguredTaskAwaitable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11}"/> that can be awaited.</returns>
	/// <remarks>
	/// This method allows you to configure how the continuation of the await operation is executed.
	/// </remarks>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>) tasks, ConfigureAwaitOptions options) =>
		new(tasks, options);
#endif

	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public readonly record struct TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : ICriticalNotifyCompletion
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>) _tasks;
		private readonly TaskAwaiter _whenAllAwaiter;

		internal TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>) tasks)
		{
			_tasks = tasks;
			_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11).GetAwaiter();
		}

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public bool IsCompleted => _whenAllAwaiter.IsCompleted;

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11) GetResult()
		{
			_whenAllAwaiter.GetResult();
			return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result, _tasks.Item9.Result, _tasks.Item10.Result, _tasks.Item11.Result);
		}
	}

	/// <summary>This type and its members are intended for use by the compiler.</summary>
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

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public Awaiter GetAwaiter() =>
			new(_tasks, _options);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
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

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public bool IsCompleted => _whenAllAwaiter.IsCompleted;

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result, _tasks.Item9.Result, _tasks.Item10.Result, _tasks.Item11.Result);
			}
		}
	}
	#endregion

	#region (Task<T1>..Task<T12>)
	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public static TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>) tasks) =>
		new(tasks);

	/// <summary>
	/// Configures an awaiter for a tuple of twelvh tasks, specifying whether to marshal the continuation back to the original context.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task.</typeparam>
	/// <typeparam name="T3">The type of the result of the third task.</typeparam>
	/// <typeparam name="T4">The type of the result of the fourth task.</typeparam>
	/// <typeparam name="T5">The type of the result of the fifth task.</typeparam>
	/// <typeparam name="T6">The type of the result of the sixth task.</typeparam>
	/// <typeparam name="T7">The type of the result of the seventh task.</typeparam>
	/// <typeparam name="T8">The type of the result of the eighth task.</typeparam>
	/// <typeparam name="T9">The type of the result of the ninth task.</typeparam>
	/// <typeparam name="T10">The type of the result of the tenth task.</typeparam>
	/// <typeparam name="T11">The type of the result of the eleventh task.</typeparam>
	/// <typeparam name="T12">The type of the result of the twelfth task.</typeparam>
	/// <param name="tasks">A tuple containing the twelve tasks to be awaited.</param>
	/// <param name="continueOnCapturedContext">
	/// A boolean value indicating whether to marshal the continuation back to the original context.
	/// </param>
	/// <returns>
	/// A <see cref="TupleConfiguredTaskAwaitable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12}"/> instance that can be awaited.
	/// </returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>) tasks, bool continueOnCapturedContext) =>
		new(tasks, continueOnCapturedContext
#if NET8_0_OR_GREATER
				? ConfigureAwaitOptions.ContinueOnCapturedContext : ConfigureAwaitOptions.None
#endif
		);

#if NET8_0_OR_GREATER
	/// <summary>
	/// Configures an awaitable tuple of twelve tasks to await asynchronously with the specified <see cref="ConfigureAwaitOptions"/>.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task.</typeparam>
	/// <typeparam name="T3">The type of the result of the third task.</typeparam>
	/// <typeparam name="T4">The type of the result of the fourth task.</typeparam>
	/// <typeparam name="T5">The type of the result of the fifth task.</typeparam>
	/// <typeparam name="T6">The type of the result of the sixth task.</typeparam>
	/// <typeparam name="T7">The type of the result of the seventh task.</typeparam>
	/// <typeparam name="T8">The type of the result of the eighth task.</typeparam>
	/// <typeparam name="T9">The type of the result of the ninth task.</typeparam>
	/// <typeparam name="T10">The type of the result of the tenth task.</typeparam>
	/// <typeparam name="T11">The type of the result of the eleventh task.</typeparam>
	/// <typeparam name="T12">The type of the result of the twelfth task.</typeparam>
	/// <param name="tasks">The tuple containing the twelve tasks to be awaited.</param>
	/// <param name="options">The <see cref="ConfigureAwaitOptions"/> to use for configuring the awaitable.</param>
	/// <returns>A <see cref="TupleConfiguredTaskAwaitable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12}"/> that can be awaited.</returns>
	/// <remarks>
	/// This method allows you to configure how the continuation of the await operation is executed.
	/// </remarks>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>) tasks, ConfigureAwaitOptions options) =>
		new(tasks, options);
#endif

	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public readonly record struct TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : ICriticalNotifyCompletion
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>) _tasks;
		private readonly TaskAwaiter _whenAllAwaiter;

		internal TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>) tasks)
		{
			_tasks = tasks;
			_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12).GetAwaiter();
		}

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public bool IsCompleted => _whenAllAwaiter.IsCompleted;

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12) GetResult()
		{
			_whenAllAwaiter.GetResult();
			return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result, _tasks.Item9.Result, _tasks.Item10.Result, _tasks.Item11.Result, _tasks.Item12.Result);
		}
	}

	/// <summary>This type and its members are intended for use by the compiler.</summary>
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

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public Awaiter GetAwaiter() =>
			new(_tasks, _options);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
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

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public bool IsCompleted => _whenAllAwaiter.IsCompleted;

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result, _tasks.Item9.Result, _tasks.Item10.Result, _tasks.Item11.Result, _tasks.Item12.Result);
			}
		}
	}
	#endregion

	#region (Task<T1>..Task<T13>)
	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public static TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>) tasks) =>
		new(tasks);

	/// <summary>
	/// Configures an awaiter for a tuple of thirteen tasks, specifying whether to marshal the continuation back to the original context.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task.</typeparam>
	/// <typeparam name="T3">The type of the result of the third task.</typeparam>
	/// <typeparam name="T4">The type of the result of the fourth task.</typeparam>
	/// <typeparam name="T5">The type of the result of the fifth task.</typeparam>
	/// <typeparam name="T6">The type of the result of the sixth task.</typeparam>
	/// <typeparam name="T7">The type of the result of the seventh task.</typeparam>
	/// <typeparam name="T8">The type of the result of the eighth task.</typeparam>
	/// <typeparam name="T9">The type of the result of the ninth task.</typeparam>
	/// <typeparam name="T10">The type of the result of the tenth task.</typeparam>
	/// <typeparam name="T11">The type of the result of the eleventh task.</typeparam>
	/// <typeparam name="T12">The type of the result of the twelfth task.</typeparam>
	/// <typeparam name="T13">The type of the result of the thirteenth task.</typeparam>
	/// <param name="tasks">A tuple containing the thirteen tasks to be awaited.</param>
	/// <param name="continueOnCapturedContext">
	/// A boolean value indicating whether to marshal the continuation back to the original context.
	/// </param>
	/// <returns>
	/// A <see cref="TupleConfiguredTaskAwaitable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13}"/> instance that can be awaited.
	/// </returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>) tasks, bool continueOnCapturedContext) =>
		new(tasks, continueOnCapturedContext
#if NET8_0_OR_GREATER
				? ConfigureAwaitOptions.ContinueOnCapturedContext : ConfigureAwaitOptions.None
#endif
		);

#if NET8_0_OR_GREATER
	/// <summary>
	/// Configures an awaitable tuple of thirteen tasks to await asynchronously with the specified <see cref="ConfigureAwaitOptions"/>.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task.</typeparam>
	/// <typeparam name="T3">The type of the result of the third task.</typeparam>
	/// <typeparam name="T4">The type of the result of the fourth task.</typeparam>
	/// <typeparam name="T5">The type of the result of the fifth task.</typeparam>
	/// <typeparam name="T6">The type of the result of the sixth task.</typeparam>
	/// <typeparam name="T7">The type of the result of the seventh task.</typeparam>
	/// <typeparam name="T8">The type of the result of the eighth task.</typeparam>
	/// <typeparam name="T9">The type of the result of the ninth task.</typeparam>
	/// <typeparam name="T10">The type of the result of the tenth task.</typeparam>
	/// <typeparam name="T11">The type of the result of the eleventh task.</typeparam>
	/// <typeparam name="T12">The type of the result of the twelfth task.</typeparam>
	/// <typeparam name="T13">The type of the result of the thirteenth task.</typeparam>
	/// <param name="tasks">The tuple containing the thirteen tasks to be awaited.</param>
	/// <param name="options">The <see cref="ConfigureAwaitOptions"/> to use for configuring the awaitable.</param>
	/// <returns>A <see cref="TupleConfiguredTaskAwaitable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13}"/> that can be awaited.</returns>
	/// <remarks>
	/// This method allows you to configure how the continuation of the await operation is executed.
	/// </remarks>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>) tasks, ConfigureAwaitOptions options) =>
		new(tasks, options);
#endif

	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public readonly record struct TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : ICriticalNotifyCompletion
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>) _tasks;
		private readonly TaskAwaiter _whenAllAwaiter;

		internal TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>) tasks)
		{
			_tasks = tasks;
			_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13).GetAwaiter();
		}

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public bool IsCompleted => _whenAllAwaiter.IsCompleted;

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13) GetResult()
		{
			_whenAllAwaiter.GetResult();
			return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result, _tasks.Item9.Result, _tasks.Item10.Result, _tasks.Item11.Result, _tasks.Item12.Result, _tasks.Item13.Result);
		}
	}

	/// <summary>This type and its members are intended for use by the compiler.</summary>
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

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public Awaiter GetAwaiter() =>
			new(_tasks, _options);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
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

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public bool IsCompleted => _whenAllAwaiter.IsCompleted;

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result, _tasks.Item9.Result, _tasks.Item10.Result, _tasks.Item11.Result, _tasks.Item12.Result, _tasks.Item13.Result);
			}
		}
	}
	#endregion

	#region (Task<T1>..Task<T14>)
	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public static TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>) tasks) =>
		new(tasks);

	/// <summary>
	/// Configures an awaiter for a tuple of fourteen tasks, specifying whether to marshal the continuation back to the original context.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task.</typeparam>
	/// <typeparam name="T3">The type of the result of the third task.</typeparam>
	/// <typeparam name="T4">The type of the result of the fourth task.</typeparam>
	/// <typeparam name="T5">The type of the result of the fifth task.</typeparam>
	/// <typeparam name="T6">The type of the result of the sixth task.</typeparam>
	/// <typeparam name="T7">The type of the result of the seventh task.</typeparam>
	/// <typeparam name="T8">The type of the result of the eighth task.</typeparam>
	/// <typeparam name="T9">The type of the result of the ninth task.</typeparam>
	/// <typeparam name="T10">The type of the result of the tenth task.</typeparam>
	/// <typeparam name="T11">The type of the result of the eleventh task.</typeparam>
	/// <typeparam name="T12">The type of the result of the twelfth task.</typeparam>
	/// <typeparam name="T13">The type of the result of the thirteenth task.</typeparam>
	/// <typeparam name="T14">The type of the result of the fourteenth task.</typeparam>
	/// <param name="tasks">A tuple containing the fourteen tasks to be awaited.</param>
	/// <param name="continueOnCapturedContext">
	/// A boolean value indicating whether to marshal the continuation back to the original context.
	/// </param>
	/// <returns>
	/// A <see cref="TupleConfiguredTaskAwaitable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14}"/> instance that can be awaited.
	/// </returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>) tasks, bool continueOnCapturedContext) =>
		new(tasks, continueOnCapturedContext
#if NET8_0_OR_GREATER
				? ConfigureAwaitOptions.ContinueOnCapturedContext : ConfigureAwaitOptions.None
#endif
		);

#if NET8_0_OR_GREATER
	/// <summary>
	/// Configures an awaitable tuple of fourteen tasks to await asynchronously with the specified <see cref="ConfigureAwaitOptions"/>.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task.</typeparam>
	/// <typeparam name="T3">The type of the result of the third task.</typeparam>
	/// <typeparam name="T4">The type of the result of the fourth task.</typeparam>
	/// <typeparam name="T5">The type of the result of the fifth task.</typeparam>
	/// <typeparam name="T6">The type of the result of the sixth task.</typeparam>
	/// <typeparam name="T7">The type of the result of the seventh task.</typeparam>
	/// <typeparam name="T8">The type of the result of the eighth task.</typeparam>
	/// <typeparam name="T9">The type of the result of the ninth task.</typeparam>
	/// <typeparam name="T10">The type of the result of the tenth task.</typeparam>
	/// <typeparam name="T11">The type of the result of the eleventh task.</typeparam>
	/// <typeparam name="T12">The type of the result of the twelfth task.</typeparam>
	/// <typeparam name="T13">The type of the result of the thirteenth task.</typeparam>
	/// <typeparam name="T14">The type of the result of the fourteenth task.</typeparam>
	/// <param name="tasks">The tuple containing the fourteen tasks to be awaited.</param>
	/// <param name="options">The <see cref="ConfigureAwaitOptions"/> to use for configuring the awaitable.</param>
	/// <returns>A <see cref="TupleConfiguredTaskAwaitable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14}"/> that can be awaited.</returns>
	/// <remarks>
	/// This method allows you to configure how the continuation of the await operation is executed.
	/// </remarks>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>) tasks, ConfigureAwaitOptions options) =>
		new(tasks, options);
#endif

	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public readonly record struct TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : ICriticalNotifyCompletion
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>) _tasks;
		private readonly TaskAwaiter _whenAllAwaiter;

		internal TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>) tasks)
		{
			_tasks = tasks;
			_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13, tasks.Item14).GetAwaiter();
		}

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public bool IsCompleted => _whenAllAwaiter.IsCompleted;

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14) GetResult()
		{
			_whenAllAwaiter.GetResult();
			return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result, _tasks.Item9.Result, _tasks.Item10.Result, _tasks.Item11.Result, _tasks.Item12.Result, _tasks.Item13.Result, _tasks.Item14.Result);
		}
	}

	/// <summary>This type and its members are intended for use by the compiler.</summary>
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

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public Awaiter GetAwaiter() =>
			new(_tasks, _options);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
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

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public bool IsCompleted => _whenAllAwaiter.IsCompleted;

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result, _tasks.Item9.Result, _tasks.Item10.Result, _tasks.Item11.Result, _tasks.Item12.Result, _tasks.Item13.Result, _tasks.Item14.Result);
			}
		}
	}
	#endregion

	#region (Task<T1>..Task<T15>)
	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public static TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>, Task<T15>) tasks) =>
		new(tasks);

	/// <summary>
	/// Configures an awaiter for a tuple of fifteen tasks, specifying whether to marshal the continuation back to the original context.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task.</typeparam>
	/// <typeparam name="T3">The type of the result of the third task.</typeparam>
	/// <typeparam name="T4">The type of the result of the fourth task.</typeparam>
	/// <typeparam name="T5">The type of the result of the fifth task.</typeparam>
	/// <typeparam name="T6">The type of the result of the sixth task.</typeparam>
	/// <typeparam name="T7">The type of the result of the seventh task.</typeparam>
	/// <typeparam name="T8">The type of the result of the eighth task.</typeparam>
	/// <typeparam name="T9">The type of the result of the ninth task.</typeparam>
	/// <typeparam name="T10">The type of the result of the tenth task.</typeparam>
	/// <typeparam name="T11">The type of the result of the eleventh task.</typeparam>
	/// <typeparam name="T12">The type of the result of the twelfth task.</typeparam>
	/// <typeparam name="T13">The type of the result of the thirteenth task.</typeparam>
	/// <typeparam name="T14">The type of the result of the fourteenth task.</typeparam>
	/// <typeparam name="T15">The type of the result of the fifteenth task.</typeparam>
	/// <param name="tasks">A tuple containing the fifteen tasks to be awaited.</param>
	/// <param name="continueOnCapturedContext">
	/// A boolean value indicating whether to marshal the continuation back to the original context.
	/// </param>
	/// <returns>
	/// A <see cref="TupleConfiguredTaskAwaitable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15}"/> instance that can be awaited.
	/// </returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>, Task<T15>) tasks, bool continueOnCapturedContext) =>
		new(tasks, continueOnCapturedContext
#if NET8_0_OR_GREATER
				? ConfigureAwaitOptions.ContinueOnCapturedContext : ConfigureAwaitOptions.None
#endif
		);

#if NET8_0_OR_GREATER
	/// <summary>
	/// Configures an awaitable tuple of fifteen tasks to await asynchronously with the specified <see cref="ConfigureAwaitOptions"/>.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task.</typeparam>
	/// <typeparam name="T3">The type of the result of the third task.</typeparam>
	/// <typeparam name="T4">The type of the result of the fourth task.</typeparam>
	/// <typeparam name="T5">The type of the result of the fifth task.</typeparam>
	/// <typeparam name="T6">The type of the result of the sixth task.</typeparam>
	/// <typeparam name="T7">The type of the result of the seventh task.</typeparam>
	/// <typeparam name="T8">The type of the result of the eighth task.</typeparam>
	/// <typeparam name="T9">The type of the result of the ninth task.</typeparam>
	/// <typeparam name="T10">The type of the result of the tenth task.</typeparam>
	/// <typeparam name="T11">The type of the result of the eleventh task.</typeparam>
	/// <typeparam name="T12">The type of the result of the twelfth task.</typeparam>
	/// <typeparam name="T13">The type of the result of the thirteenth task.</typeparam>
	/// <typeparam name="T14">The type of the result of the fourteenth task.</typeparam>
	/// <typeparam name="T15">The type of the result of the fifteenth task.</typeparam>
	/// <param name="tasks">The tuple containing the fifteen tasks to be awaited.</param>
	/// <param name="options">The <see cref="ConfigureAwaitOptions"/> to use for configuring the awaitable.</param>
	/// <returns>A <see cref="TupleConfiguredTaskAwaitable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15}"/> that can be awaited.</returns>
	/// <remarks>
	/// This method allows you to configure how the continuation of the await operation is executed.
	/// </remarks>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>, Task<T15>) tasks, ConfigureAwaitOptions options) =>
		new(tasks, options);
#endif

	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public readonly record struct TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : ICriticalNotifyCompletion
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>, Task<T15>) _tasks;
		private readonly TaskAwaiter _whenAllAwaiter;

		internal TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>, Task<T15>) tasks)
		{
			_tasks = tasks;
			_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13, tasks.Item14, tasks.Item15).GetAwaiter();
		}

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public bool IsCompleted => _whenAllAwaiter.IsCompleted;

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15) GetResult()
		{
			_whenAllAwaiter.GetResult();
			return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result, _tasks.Item9.Result, _tasks.Item10.Result, _tasks.Item11.Result, _tasks.Item12.Result, _tasks.Item13.Result, _tasks.Item14.Result, _tasks.Item15.Result);
		}
	}

	/// <summary>This type and its members are intended for use by the compiler.</summary>
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

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public Awaiter GetAwaiter() =>
			new(_tasks, _options);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
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

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public bool IsCompleted => _whenAllAwaiter.IsCompleted;

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result, _tasks.Item9.Result, _tasks.Item10.Result, _tasks.Item11.Result, _tasks.Item12.Result, _tasks.Item13.Result, _tasks.Item14.Result, _tasks.Item15.Result);
			}
		}
	}
	#endregion

	#region (Task<T1>..Task<T16>)
	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public static TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>, Task<T15>, Task<T16>) tasks) =>
		new(tasks);

	/// <summary>
	/// Configures an awaiter for a tuple of sixteen tasks, specifying whether to marshal the continuation back to the original context.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task.</typeparam>
	/// <typeparam name="T3">The type of the result of the third task.</typeparam>
	/// <typeparam name="T4">The type of the result of the fourth task.</typeparam>
	/// <typeparam name="T5">The type of the result of the fifth task.</typeparam>
	/// <typeparam name="T6">The type of the result of the sixth task.</typeparam>
	/// <typeparam name="T7">The type of the result of the seventh task.</typeparam>
	/// <typeparam name="T8">The type of the result of the eighth task.</typeparam>
	/// <typeparam name="T9">The type of the result of the ninth task.</typeparam>
	/// <typeparam name="T10">The type of the result of the tenth task.</typeparam>
	/// <typeparam name="T11">The type of the result of the eleventh task.</typeparam>
	/// <typeparam name="T12">The type of the result of the twelfth task.</typeparam>
	/// <typeparam name="T13">The type of the result of the thirteenth task.</typeparam>
	/// <typeparam name="T14">The type of the result of the fourteenth task.</typeparam>
	/// <typeparam name="T15">The type of the result of the fifteenth task.</typeparam>
	/// <typeparam name="T16">The type of the result of the sixteenth task.</typeparam>
	/// <param name="tasks">A tuple containing the sixteen tasks to be awaited.</param>
	/// <param name="continueOnCapturedContext">
	/// A boolean value indicating whether to marshal the continuation back to the original context.
	/// </param>
	/// <returns>
	/// A <see cref="TupleConfiguredTaskAwaitable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16}"/> instance that can be awaited.
	/// </returns>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>, Task<T15>, Task<T16>) tasks, bool continueOnCapturedContext) =>
		new(tasks, continueOnCapturedContext
#if NET8_0_OR_GREATER
				? ConfigureAwaitOptions.ContinueOnCapturedContext : ConfigureAwaitOptions.None
#endif
		);

#if NET8_0_OR_GREATER
	/// <summary>
	/// Configures an awaitable tuple of sixteen tasks to await asynchronously with the specified <see cref="ConfigureAwaitOptions"/>.
	/// </summary>
	/// <typeparam name="T1">The type of the result of the first task.</typeparam>
	/// <typeparam name="T2">The type of the result of the second task.</typeparam>
	/// <typeparam name="T3">The type of the result of the third task.</typeparam>
	/// <typeparam name="T4">The type of the result of the fourth task.</typeparam>
	/// <typeparam name="T5">The type of the result of the fifth task.</typeparam>
	/// <typeparam name="T6">The type of the result of the sixth task.</typeparam>
	/// <typeparam name="T7">The type of the result of the seventh task.</typeparam>
	/// <typeparam name="T8">The type of the result of the eighth task.</typeparam>
	/// <typeparam name="T9">The type of the result of the ninth task.</typeparam>
	/// <typeparam name="T10">The type of the result of the tenth task.</typeparam>
	/// <typeparam name="T11">The type of the result of the eleventh task.</typeparam>
	/// <typeparam name="T12">The type of the result of the twelfth task.</typeparam>
	/// <typeparam name="T13">The type of the result of the thirteenth task.</typeparam>
	/// <typeparam name="T14">The type of the result of the fourteenth task.</typeparam>
	/// <typeparam name="T15">The type of the result of the fifteenth task.</typeparam>
	/// <typeparam name="T16">The type of the result of the sixteenth task.</typeparam>
	/// <param name="tasks">The tuple containing the sixteen tasks to be awaited.</param>
	/// <param name="options">The <see cref="ConfigureAwaitOptions"/> to use for configuring the awaitable.</param>
	/// <returns>A <see cref="TupleConfiguredTaskAwaitable{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16}"/> that can be awaited.</returns>
	/// <remarks>
	/// This method allows you to configure how the continuation of the await operation is executed.
	/// </remarks>
	public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>, Task<T15>, Task<T16>) tasks, ConfigureAwaitOptions options) =>
		new(tasks, options);
#endif

	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public readonly record struct TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> : ICriticalNotifyCompletion
	{
		private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>, Task<T15>, Task<T16>) _tasks;
		private readonly TaskAwaiter _whenAllAwaiter;

		internal TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>, Task<T11>, Task<T12>, Task<T13>, Task<T14>, Task<T15>, Task<T16>) tasks)
		{
			_tasks = tasks;
			_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13, tasks.Item14, tasks.Item15, tasks.Item16).GetAwaiter();
		}

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public bool IsCompleted => _whenAllAwaiter.IsCompleted;

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16) GetResult()
		{
			_whenAllAwaiter.GetResult();
			return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result, _tasks.Item9.Result, _tasks.Item10.Result, _tasks.Item11.Result, _tasks.Item12.Result, _tasks.Item13.Result, _tasks.Item14.Result, _tasks.Item15.Result, _tasks.Item16.Result);
		}
	}

	/// <summary>This type and its members are intended for use by the compiler.</summary>
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

		/// <summary>This type and its members are intended for use by the compiler.</summary>
		public Awaiter GetAwaiter() =>
			new(_tasks, _options);

		/// <summary>This type and its members are intended for use by the compiler.</summary>
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

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public bool IsCompleted => _whenAllAwaiter.IsCompleted;

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>This type and its members are intended for use by the compiler.</summary>
			public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result, _tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result, _tasks.Item9.Result, _tasks.Item10.Result, _tasks.Item11.Result, _tasks.Item12.Result, _tasks.Item13.Result, _tasks.Item14.Result, _tasks.Item15.Result, _tasks.Item16.Result);
			}
		}
	}
	#endregion

	#region Task
	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public static TaskAwaiter GetAwaiter(this ValueTuple<Task> tasks) =>
		tasks.Item1.GetAwaiter();

	/// <summary>
	/// Configures an awaiter for a single task within a <see cref="ValueTuple{T1}"/> to continue on a specified context.
	/// </summary>
	/// <param name="tasks">A <see cref="ValueTuple{T1}"/> containing a single <see cref="Task"/>.</param>
	/// <param name="continueOnCapturedContext">
	/// A Boolean value indicating whether to marshal the continuation back to the original context captured.
	/// </param>
	/// <returns>
	/// A <see cref="ConfiguredTaskAwaitable"/> that can be used to await the task with the specified configuration.
	/// </returns>
	/// <remarks>
	/// This method is useful for controlling the behavior of task continuations, such as whether they should
	/// execute on the original synchronization context or not.
	/// </remarks>
	public static ConfiguredTaskAwaitable ConfigureAwait(this ValueTuple<Task> tasks, bool continueOnCapturedContext) =>
		tasks.Item1.ConfigureAwait(continueOnCapturedContext);

	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public static TaskAwaiter GetAwaiter(this (Task, Task) tasks) =>
		Task.WhenAll(tasks.Item1, tasks.Item2).GetAwaiter();

	/// <summary>
	/// Configures an awaiter used to await a tuple of two tasks.
	/// </summary>
	/// <param name="tasks">A tuple containing two tasks to be awaited.</param>
	/// <param name="continueOnCapturedContext">
	/// A boolean value indicating whether to marshal the continuation back to the original context captured.
	/// </param>
	/// <returns>
	/// A <see cref="ConfiguredTaskAwaitable"/> instance that can be used to await the completion of the tasks.
	/// </returns>
	/// <remarks>
	/// This method allows configuring the behavior of awaiting a tuple of two tasks, such as whether the continuation
	/// should be executed on the captured synchronization context.
	/// </remarks>
	/// <exception cref="ArgumentNullException">
	/// Thrown if any of the tasks in the tuple are <c>null</c>.
	/// </exception>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task) tasks, bool continueOnCapturedContext) =>
		Task.WhenAll(tasks.Item1, tasks.Item2).ConfigureAwait(continueOnCapturedContext);

	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public static TaskAwaiter GetAwaiter(this (Task, Task, Task) tasks) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3).GetAwaiter();

	/// <summary>
	/// Configures an awaiter for a tuple of three <see cref="Task"/> instances, specifying whether to continue on the captured context.
	/// </summary>
	/// <param name="tasks">A tuple containing three <see cref="Task"/> instances to be awaited.</param>
	/// <param name="continueOnCapturedContext">
	/// A boolean value indicating whether to attempt to marshal the continuation back to the original context captured.
	/// </param>
	/// <returns>
	/// A <see cref="ConfiguredTaskAwaitable"/> instance that can be used to await the completion of the tasks.
	/// </returns>
	/// <remarks>
	/// This method is useful for awaiting multiple tasks in a tuple while controlling the context in which the continuation executes.
	/// </remarks>
	/// <exception cref="ArgumentNullException">
	/// Thrown if any of the tasks in the tuple is <c>null</c>.
	/// </exception>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task) tasks, bool continueOnCapturedContext) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3).ConfigureAwait(continueOnCapturedContext);

	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task) tasks) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4).GetAwaiter();

	/// <summary>
	/// Configures an awaiter for a tuple of four <see cref="Task"/> instances, specifying whether to continue on the captured context.
	/// </summary>
	/// <param name="tasks">A tuple containing four <see cref="Task"/> instances to be awaited.</param>
	/// <param name="continueOnCapturedContext">
	/// A boolean value indicating whether to attempt to marshal the continuation back to the original context captured.
	/// </param>
	/// <returns>
	/// A <see cref="ConfiguredTaskAwaitable"/> instance that can be used to await the completion of the tasks.
	/// </returns>
	/// <remarks>
	/// This method is useful for awaiting multiple tasks in a tuple while controlling the context in which the continuation executes.
	/// </remarks>
	/// <exception cref="ArgumentNullException">
	/// Thrown if any of the tasks in the tuple is <c>null</c>.
	/// </exception>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task) tasks, bool continueOnCapturedContext) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4).ConfigureAwait(continueOnCapturedContext);

	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task) tasks) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5).GetAwaiter();

	/// <summary>
	/// Configures an awaiter for a tuple of five <see cref="Task"/> instances, specifying whether to continue on the captured context.
	/// </summary>
	/// <param name="tasks">A tuple containing five <see cref="Task"/> instances to be awaited.</param>
	/// <param name="continueOnCapturedContext">
	/// A boolean value indicating whether to attempt to marshal the continuation back to the original context captured.
	/// </param>
	/// <returns>
	/// A <see cref="ConfiguredTaskAwaitable"/> instance that can be used to await the completion of the tasks.
	/// </returns>
	/// <remarks>
	/// This method is useful for awaiting multiple tasks in a tuple while controlling the context in which the continuation executes.
	/// </remarks>
	/// <exception cref="ArgumentNullException">
	/// Thrown if any of the tasks in the tuple is <c>null</c>.
	/// </exception>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task) tasks, bool continueOnCapturedContext) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5).ConfigureAwait(continueOnCapturedContext);

	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task) tasks) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6).GetAwaiter();

	/// <summary>
	/// Configures an awaiter for a tuple of six <see cref="Task"/> instances, specifying whether to continue on the captured context.
	/// </summary>
	/// <param name="tasks">A tuple containing six <see cref="Task"/> instances to be awaited.</param>
	/// <param name="continueOnCapturedContext">
	/// A boolean value indicating whether to attempt to marshal the continuation back to the original context captured.
	/// </param>
	/// <returns>
	/// A <see cref="ConfiguredTaskAwaitable"/> instance that can be used to await the completion of the tasks.
	/// </returns>
	/// <remarks>
	/// This method is useful for awaiting multiple tasks in a tuple while controlling the context in which the continuation executes.
	/// </remarks>
	/// <exception cref="ArgumentNullException">
	/// Thrown if any of the tasks in the tuple is <c>null</c>.
	/// </exception>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task) tasks, bool continueOnCapturedContext) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6).ConfigureAwait(continueOnCapturedContext);

	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task, Task) tasks) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7).GetAwaiter();

	/// <summary>
	/// Configures an awaiter for a tuple of seven <see cref="Task"/> instances, specifying whether to continue on the captured context.
	/// </summary>
	/// <param name="tasks">A tuple containing seven <see cref="Task"/> instances to be awaited.</param>
	/// <param name="continueOnCapturedContext">
	/// A boolean value indicating whether to attempt to marshal the continuation back to the original context captured.
	/// </param>
	/// <returns>
	/// A <see cref="ConfiguredTaskAwaitable"/> instance that can be used to await the completion of the tasks.
	/// </returns>
	/// <remarks>
	/// This method is useful for awaiting multiple tasks in a tuple while controlling the context in which the continuation executes.
	/// </remarks>
	/// <exception cref="ArgumentNullException">
	/// Thrown if any of the tasks in the tuple is <c>null</c>.
	/// </exception>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task) tasks, bool continueOnCapturedContext) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7).ConfigureAwait(continueOnCapturedContext);

	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task, Task, Task) tasks) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8).GetAwaiter();

	/// <summary>
	/// Configures an awaiter for a tuple of eight <see cref="Task"/> instances, specifying whether to continue on the captured context.
	/// </summary>
	/// <param name="tasks">A tuple containing eight <see cref="Task"/> instances to be awaited.</param>
	/// <param name="continueOnCapturedContext">
	/// A boolean value indicating whether to attempt to marshal the continuation back to the original context captured.
	/// </param>
	/// <returns>
	/// A <see cref="ConfiguredTaskAwaitable"/> instance that can be used to await the completion of the tasks.
	/// </returns>
	/// <remarks>
	/// This method is useful for awaiting multiple tasks in a tuple while controlling the context in which the continuation executes.
	/// </remarks>
	/// <exception cref="ArgumentNullException">
	/// Thrown if any of the tasks in the tuple is <c>null</c>.
	/// </exception>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task) tasks, bool continueOnCapturedContext) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8).ConfigureAwait(continueOnCapturedContext);

	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9).GetAwaiter();

	/// <summary>
	/// Configures an awaiter for a tuple of nine <see cref="Task"/> instances, specifying whether to continue on the captured context.
	/// </summary>
	/// <param name="tasks">A tuple containing nine <see cref="Task"/> instances to be awaited.</param>
	/// <param name="continueOnCapturedContext">
	/// A boolean value indicating whether to attempt to marshal the continuation back to the original context captured.
	/// </param>
	/// <returns>
	/// A <see cref="ConfiguredTaskAwaitable"/> instance that can be used to await the completion of the tasks.
	/// </returns>
	/// <remarks>
	/// This method is useful for awaiting multiple tasks in a tuple while controlling the context in which the continuation executes.
	/// </remarks>
	/// <exception cref="ArgumentNullException">
	/// Thrown if any of the tasks in the tuple is <c>null</c>.
	/// </exception>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, bool continueOnCapturedContext) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9).ConfigureAwait(continueOnCapturedContext);

	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10).GetAwaiter();

	/// <summary>
	/// Configures an awaiter for a tuple of ten <see cref="Task"/> instances, specifying whether to continue on the captured context.
	/// </summary>
	/// <param name="tasks">A tuple containing ten <see cref="Task"/> instances to be awaited.</param>
	/// <param name="continueOnCapturedContext">
	/// A boolean value indicating whether to attempt to marshal the continuation back to the original context captured.
	/// </param>
	/// <returns>
	/// A <see cref="ConfiguredTaskAwaitable"/> instance that can be used to await the completion of the tasks.
	/// </returns>
	/// <remarks>
	/// This method is useful for awaiting multiple tasks in a tuple while controlling the context in which the continuation executes.
	/// </remarks>
	/// <exception cref="ArgumentNullException">
	/// Thrown if any of the tasks in the tuple is <c>null</c>.
	/// </exception>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, bool continueOnCapturedContext) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10).ConfigureAwait(continueOnCapturedContext);

	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11).GetAwaiter();

	/// <summary>
	/// Configures an awaiter for a tuple of eleven <see cref="Task"/> instances, specifying whether to continue on the captured context.
	/// </summary>
	/// <param name="tasks">A tuple containing eleven <see cref="Task"/> instances to be awaited.</param>
	/// <param name="continueOnCapturedContext">
	/// A boolean value indicating whether to attempt to marshal the continuation back to the original context captured.
	/// </param>
	/// <returns>
	/// A <see cref="ConfiguredTaskAwaitable"/> instance that can be used to await the completion of the tasks.
	/// </returns>
	/// <remarks>
	/// This method is useful for awaiting multiple tasks in a tuple while controlling the context in which the continuation executes.
	/// </remarks>
	/// <exception cref="ArgumentNullException">
	/// Thrown if any of the tasks in the tuple is <c>null</c>.
	/// </exception>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, bool continueOnCapturedContext) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11).ConfigureAwait(continueOnCapturedContext);

	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12).GetAwaiter();

	/// <summary>
	/// Configures an awaiter for a tuple of twelve <see cref="Task"/> instances, specifying whether to continue on the captured context.
	/// </summary>
	/// <param name="tasks">A tuple containing twelve <see cref="Task"/> instances to be awaited.</param>
	/// <param name="continueOnCapturedContext">
	/// A boolean value indicating whether to attempt to marshal the continuation back to the original context captured.
	/// </param>
	/// <returns>
	/// A <see cref="ConfiguredTaskAwaitable"/> instance that can be used to await the completion of the tasks.
	/// </returns>
	/// <remarks>
	/// This method is useful for awaiting multiple tasks in a tuple while controlling the context in which the continuation executes.
	/// </remarks>
	/// <exception cref="ArgumentNullException">
	/// Thrown if any of the tasks in the tuple is <c>null</c>.
	/// </exception>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, bool continueOnCapturedContext) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12).ConfigureAwait(continueOnCapturedContext);

	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13).GetAwaiter();

	/// <summary>
	/// Configures an awaiter for a tuple of thirteen <see cref="Task"/> instances, specifying whether to continue on the captured context.
	/// </summary>
	/// <param name="tasks">A tuple containing thirteen <see cref="Task"/> instances to be awaited.</param>
	/// <param name="continueOnCapturedContext">
	/// A boolean value indicating whether to attempt to marshal the continuation back to the original context captured.
	/// </param>
	/// <returns>
	/// A <see cref="ConfiguredTaskAwaitable"/> instance that can be used to await the completion of the tasks.
	/// </returns>
	/// <remarks>
	/// This method is useful for awaiting multiple tasks in a tuple while controlling the context in which the continuation executes.
	/// </remarks>
	/// <exception cref="ArgumentNullException">
	/// Thrown if any of the tasks in the tuple is <c>null</c>.
	/// </exception>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, bool continueOnCapturedContext) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13).ConfigureAwait(continueOnCapturedContext);

	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13, tasks.Item14).GetAwaiter();

	/// <summary>
	/// Configures an awaiter for a tuple of fourteen <see cref="Task"/> instances, specifying whether to continue on the captured context.
	/// </summary>
	/// <param name="tasks">A tuple containing fourteen <see cref="Task"/> instances to be awaited.</param>
	/// <param name="continueOnCapturedContext">
	/// A boolean value indicating whether to attempt to marshal the continuation back to the original context captured.
	/// </param>
	/// <returns>
	/// A <see cref="ConfiguredTaskAwaitable"/> instance that can be used to await the completion of the tasks.
	/// </returns>
	/// <remarks>
	/// This method is useful for awaiting multiple tasks in a tuple while controlling the context in which the continuation executes.
	/// </remarks>
	/// <exception cref="ArgumentNullException">
	/// Thrown if any of the tasks in the tuple is <c>null</c>.
	/// </exception>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, bool continueOnCapturedContext) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13, tasks.Item14).ConfigureAwait(continueOnCapturedContext);

	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13, tasks.Item14, tasks.Item15).GetAwaiter();

	/// <summary>
	/// Configures an awaiter for a tuple of fifteen <see cref="Task"/> instances, specifying whether to continue on the captured context.
	/// </summary>
	/// <param name="tasks">A tuple containing fifteen <see cref="Task"/> instances to be awaited.</param>
	/// <param name="continueOnCapturedContext">
	/// A boolean value indicating whether to attempt to marshal the continuation back to the original context captured.
	/// </param>
	/// <returns>
	/// A <see cref="ConfiguredTaskAwaitable"/> instance that can be used to await the completion of the tasks.
	/// </returns>
	/// <remarks>
	/// This method is useful for awaiting multiple tasks in a tuple while controlling the context in which the continuation executes.
	/// </remarks>
	/// <exception cref="ArgumentNullException">
	/// Thrown if any of the tasks in the tuple is <c>null</c>.
	/// </exception>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, bool continueOnCapturedContext) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13, tasks.Item14, tasks.Item15).ConfigureAwait(continueOnCapturedContext);

	/// <summary>This type and its members are intended for use by the compiler.</summary>
	public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13, tasks.Item14, tasks.Item15, tasks.Item16).GetAwaiter();

	/// <summary>
	/// Configures an awaiter for a tuple of sixteen <see cref="Task"/> instances, specifying whether to continue on the captured context.
	/// </summary>
	/// <param name="tasks">A tuple containing sixteen <see cref="Task"/> instances to be awaited.</param>
	/// <param name="continueOnCapturedContext">
	/// A boolean value indicating whether to attempt to marshal the continuation back to the original context captured.
	/// </param>
	/// <returns>
	/// A <see cref="ConfiguredTaskAwaitable"/> instance that can be used to await the completion of the tasks.
	/// </returns>
	/// <remarks>
	/// This method is useful for awaiting multiple tasks in a tuple while controlling the context in which the continuation executes.
	/// </remarks>
	/// <exception cref="ArgumentNullException">
	/// Thrown if any of the tasks in the tuple is <c>null</c>.
	/// </exception>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, bool continueOnCapturedContext) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13, tasks.Item14, tasks.Item15, tasks.Item16).ConfigureAwait(continueOnCapturedContext);

#if NET8_0_OR_GREATER
	/// <summary>
	/// Configures an awaiter used to await the specified task tuple.
	/// </summary>
	/// <param name="tasks">A tuple containing a single <see cref="Task"/> to configure the awaiter for.</param>
	/// <param name="options">The <see cref="ConfigureAwaitOptions"/> to use for configuring the awaiter.</param>
	/// <returns>A <see cref="ConfiguredTaskAwaitable"/> that can be awaited.</returns>
	/// <remarks>
	/// This method allows you to configure how the continuation of the awaited task is executed, 
	/// such as whether to capture the current synchronization context.
	/// </remarks>
	public static ConfiguredTaskAwaitable ConfigureAwait(this ValueTuple<Task> tasks, ConfigureAwaitOptions options) =>
		tasks.Item1.ConfigureAwait(options);

	/// <summary>
	/// Configures an awaiter for a tuple of two <see cref="Task"/> instances, specifying 
	/// the behavior for continuation after the tasks complete.
	/// </summary>
	/// <param name="tasks">
	/// A tuple containing two <see cref="Task"/> instances to be awaited.
	/// </param>
	/// <param name="options">
	/// A <see cref="ConfigureAwaitOptions"/> value that specifies how the continuation 
	/// should be executed.
	/// </param>
	/// <returns>
	/// A <see cref="ConfiguredTaskAwaitable"/> instance that can be used to await the completion 
	/// of all tasks in the tuple with the specified configuration.
	/// </returns>
	/// <remarks>
	/// This method allows you to configure how the continuation after awaiting the tasks is executed, 
	/// such as whether to capture the current synchronization context.
	/// </remarks>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task) tasks, ConfigureAwaitOptions options) =>
		Task.WhenAll(tasks.Item1, tasks.Item2).ConfigureAwait(options);

	/// <summary>
	/// Configures an awaiter for a tuple of three <see cref="Task"/> instances, specifying 
	/// the behavior for continuation after the tasks complete.
	/// </summary>
	/// <param name="tasks">
	/// A tuple containing three <see cref="Task"/> instances to be awaited.
	/// </param>
	/// <param name="options">
	/// A <see cref="ConfigureAwaitOptions"/> value that specifies how the continuation 
	/// should be executed.
	/// </param>
	/// <returns>
	/// A <see cref="ConfiguredTaskAwaitable"/> instance that can be used to await the completion 
	/// of all tasks in the tuple with the specified configuration.
	/// </returns>
	/// <remarks>
	/// This method allows you to configure how the continuation after awaiting the tasks is executed, 
	/// such as whether to capture the current synchronization context.
	/// </remarks>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task) tasks, ConfigureAwaitOptions options) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3).ConfigureAwait(options);

	/// <summary>
	/// Configures an awaiter for a tuple of four <see cref="Task"/> instances, specifying 
	/// the behavior for continuation after the tasks complete.
	/// </summary>
	/// <param name="tasks">
	/// A tuple containing four <see cref="Task"/> instances to be awaited.
	/// </param>
	/// <param name="options">
	/// A <see cref="ConfigureAwaitOptions"/> value that specifies how the continuation 
	/// should be executed.
	/// </param>
	/// <returns>
	/// A <see cref="ConfiguredTaskAwaitable"/> instance that can be used to await the completion 
	/// of all tasks in the tuple with the specified configuration.
	/// </returns>
	/// <remarks>
	/// This method allows you to configure how the continuation after awaiting the tasks is executed, 
	/// such as whether to capture the current synchronization context.
	/// </remarks>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task) tasks, ConfigureAwaitOptions options) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4).ConfigureAwait(options);

	/// <summary>
	/// Configures an awaiter for a tuple of five <see cref="Task"/> instances, specifying 
	/// the behavior for continuation after the tasks complete.
	/// </summary>
	/// <param name="tasks">
	/// A tuple containing five <see cref="Task"/> instances to be awaited.
	/// </param>
	/// <param name="options">
	/// A <see cref="ConfigureAwaitOptions"/> value that specifies how the continuation 
	/// should be executed.
	/// </param>
	/// <returns>
	/// A <see cref="ConfiguredTaskAwaitable"/> instance that can be used to await the completion 
	/// of all tasks in the tuple with the specified configuration.
	/// </returns>
	/// <remarks>
	/// This method allows you to configure how the continuation after awaiting the tasks is executed, 
	/// such as whether to capture the current synchronization context.
	/// </remarks>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task) tasks, ConfigureAwaitOptions options) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5).ConfigureAwait(options);

	/// <summary>
	/// Configures an awaiter for a tuple of six <see cref="Task"/> instances, specifying 
	/// the behavior for continuation after the tasks complete.
	/// </summary>
	/// <param name="tasks">
	/// A tuple containing six <see cref="Task"/> instances to be awaited.
	/// </param>
	/// <param name="options">
	/// A <see cref="ConfigureAwaitOptions"/> value that specifies how the continuation 
	/// should be executed.
	/// </param>
	/// <returns>
	/// A <see cref="ConfiguredTaskAwaitable"/> instance that can be used to await the completion 
	/// of all tasks in the tuple with the specified configuration.
	/// </returns>
	/// <remarks>
	/// This method allows you to configure how the continuation after awaiting the tasks is executed, 
	/// such as whether to capture the current synchronization context.
	/// </remarks>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task) tasks, ConfigureAwaitOptions options) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6).ConfigureAwait(options);

	/// <summary>
	/// Configures an awaiter for a tuple of seven <see cref="Task"/> instances, specifying 
	/// the behavior for continuation after the tasks complete.
	/// </summary>
	/// <param name="tasks">
	/// A tuple containing seven <see cref="Task"/> instances to be awaited.
	/// </param>
	/// <param name="options">
	/// A <see cref="ConfigureAwaitOptions"/> value that specifies how the continuation 
	/// should be executed.
	/// </param>
	/// <returns>
	/// A <see cref="ConfiguredTaskAwaitable"/> instance that can be used to await the completion 
	/// of all tasks in the tuple with the specified configuration.
	/// </returns>
	/// <remarks>
	/// This method allows you to configure how the continuation after awaiting the tasks is executed, 
	/// such as whether to capture the current synchronization context.
	/// </remarks>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task) tasks, ConfigureAwaitOptions options) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7).ConfigureAwait(options);

	/// <summary>
	/// Configures an awaiter for a tuple of eight <see cref="Task"/> instances, specifying 
	/// the behavior for continuation after the tasks complete.
	/// </summary>
	/// <param name="tasks">
	/// A tuple containing eight <see cref="Task"/> instances to be awaited.
	/// </param>
	/// <param name="options">
	/// A <see cref="ConfigureAwaitOptions"/> value that specifies how the continuation 
	/// should be executed.
	/// </param>
	/// <returns>
	/// A <see cref="ConfiguredTaskAwaitable"/> instance that can be used to await the completion 
	/// of all tasks in the tuple with the specified configuration.
	/// </returns>
	/// <remarks>
	/// This method allows you to configure how the continuation after awaiting the tasks is executed, 
	/// such as whether to capture the current synchronization context.
	/// </remarks>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task) tasks, ConfigureAwaitOptions options) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8).ConfigureAwait(options);

	/// <summary>
	/// Configures an awaiter for a tuple of nine <see cref="Task"/> instances, specifying 
	/// the behavior for continuation after the tasks complete.
	/// </summary>
	/// <param name="tasks">
	/// A tuple containing nine <see cref="Task"/> instances to be awaited.
	/// </param>
	/// <param name="options">
	/// A <see cref="ConfigureAwaitOptions"/> value that specifies how the continuation 
	/// should be executed.
	/// </param>
	/// <returns>
	/// A <see cref="ConfiguredTaskAwaitable"/> instance that can be used to await the completion 
	/// of all tasks in the tuple with the specified configuration.
	/// </returns>
	/// <remarks>
	/// This method allows you to configure how the continuation after awaiting the tasks is executed, 
	/// such as whether to capture the current synchronization context.
	/// </remarks>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, ConfigureAwaitOptions options) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9).ConfigureAwait(options);

	/// <summary>
	/// Configures an awaiter for a tuple of ten <see cref="Task"/> instances, specifying 
	/// the behavior for continuation after the tasks complete.
	/// </summary>
	/// <param name="tasks">
	/// A tuple containing ten <see cref="Task"/> instances to be awaited.
	/// </param>
	/// <param name="options">
	/// A <see cref="ConfigureAwaitOptions"/> value that specifies how the continuation 
	/// should be executed.
	/// </param>
	/// <returns>
	/// A <see cref="ConfiguredTaskAwaitable"/> instance that can be used to await the completion 
	/// of all tasks in the tuple with the specified configuration.
	/// </returns>
	/// <remarks>
	/// This method allows you to configure how the continuation after awaiting the tasks is executed, 
	/// such as whether to capture the current synchronization context.
	/// </remarks>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, ConfigureAwaitOptions options) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10).ConfigureAwait(options);

	/// <summary>
	/// Configures an awaiter for a tuple of eleven <see cref="Task"/> instances, specifying 
	/// the behavior for continuation after the tasks complete.
	/// </summary>
	/// <param name="tasks">
	/// A tuple containing eleven <see cref="Task"/> instances to be awaited.
	/// </param>
	/// <param name="options">
	/// A <see cref="ConfigureAwaitOptions"/> value that specifies how the continuation 
	/// should be executed.
	/// </param>
	/// <returns>
	/// A <see cref="ConfiguredTaskAwaitable"/> instance that can be used to await the completion 
	/// of all tasks in the tuple with the specified configuration.
	/// </returns>
	/// <remarks>
	/// This method allows you to configure how the continuation after awaiting the tasks is executed, 
	/// such as whether to capture the current synchronization context.
	/// </remarks>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, ConfigureAwaitOptions options) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11).ConfigureAwait(options);

	/// <summary>
	/// Configures an awaiter for a tuple of twelve <see cref="Task"/> instances, specifying 
	/// the behavior for continuation after the tasks complete.
	/// </summary>
	/// <param name="tasks">
	/// A tuple containing twelve <see cref="Task"/> instances to be awaited.
	/// </param>
	/// <param name="options">
	/// A <see cref="ConfigureAwaitOptions"/> value that specifies how the continuation 
	/// should be executed.
	/// </param>
	/// <returns>
	/// A <see cref="ConfiguredTaskAwaitable"/> instance that can be used to await the completion 
	/// of all tasks in the tuple with the specified configuration.
	/// </returns>
	/// <remarks>
	/// This method allows you to configure how the continuation after awaiting the tasks is executed, 
	/// such as whether to capture the current synchronization context.
	/// </remarks>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, ConfigureAwaitOptions options) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12).ConfigureAwait(options);

	/// <summary>
	/// Configures an awaiter for a tuple of thirteen <see cref="Task"/> instances, specifying 
	/// the behavior for continuation after the tasks complete.
	/// </summary>
	/// <param name="tasks">
	/// A tuple containing thirteen <see cref="Task"/> instances to be awaited.
	/// </param>
	/// <param name="options">
	/// A <see cref="ConfigureAwaitOptions"/> value that specifies how the continuation 
	/// should be executed.
	/// </param>
	/// <returns>
	/// A <see cref="ConfiguredTaskAwaitable"/> instance that can be used to await the completion 
	/// of all tasks in the tuple with the specified configuration.
	/// </returns>
	/// <remarks>
	/// This method allows you to configure how the continuation after awaiting the tasks is executed, 
	/// such as whether to capture the current synchronization context.
	/// </remarks>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, ConfigureAwaitOptions options) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13).ConfigureAwait(options);

	/// <summary>
	/// Configures an awaiter for a tuple of fourteen <see cref="Task"/> instances, specifying 
	/// the behavior for continuation after the tasks complete.
	/// </summary>
	/// <param name="tasks">
	/// A tuple containing fourteen <see cref="Task"/> instances to be awaited.
	/// </param>
	/// <param name="options">
	/// A <see cref="ConfigureAwaitOptions"/> value that specifies how the continuation 
	/// should be executed.
	/// </param>
	/// <returns>
	/// A <see cref="ConfiguredTaskAwaitable"/> instance that can be used to await the completion 
	/// of all tasks in the tuple with the specified configuration.
	/// </returns>
	/// <remarks>
	/// This method allows you to configure how the continuation after awaiting the tasks is executed, 
	/// such as whether to capture the current synchronization context.
	/// </remarks>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, ConfigureAwaitOptions options) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13, tasks.Item14).ConfigureAwait(options);

	/// <summary>
	/// Configures an awaiter for a tuple of fifteen <see cref="Task"/> instances, specifying 
	/// the behavior for continuation after the tasks complete.
	/// </summary>
	/// <param name="tasks">
	/// A tuple containing fifteen <see cref="Task"/> instances to be awaited.
	/// </param>
	/// <param name="options">
	/// A <see cref="ConfigureAwaitOptions"/> value that specifies how the continuation 
	/// should be executed.
	/// </param>
	/// <returns>
	/// A <see cref="ConfiguredTaskAwaitable"/> instance that can be used to await the completion 
	/// of all tasks in the tuple with the specified configuration.
	/// </returns>
	/// <remarks>
	/// This method allows you to configure how the continuation after awaiting the tasks is executed, 
	/// such as whether to capture the current synchronization context.
	/// </remarks>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, ConfigureAwaitOptions options) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13, tasks.Item14, tasks.Item15).ConfigureAwait(options);

	/// <summary>
	/// Configures an awaiter for a tuple of sixteen <see cref="Task"/> instances, specifying 
	/// the behavior for continuation after the tasks complete.
	/// </summary>
	/// <param name="tasks">
	/// A tuple containing sixteen <see cref="Task"/> instances to be awaited.
	/// </param>
	/// <param name="options">
	/// A <see cref="ConfigureAwaitOptions"/> value that specifies how the continuation 
	/// should be executed.
	/// </param>
	/// <returns>
	/// A <see cref="ConfiguredTaskAwaitable"/> instance that can be used to await the completion 
	/// of all tasks in the tuple with the specified configuration.
	/// </returns>
	/// <remarks>
	/// This method allows you to configure how the continuation after awaiting the tasks is executed, 
	/// such as whether to capture the current synchronization context.
	/// </remarks>
	public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, ConfigureAwaitOptions options) =>
		Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10, tasks.Item11, tasks.Item12, tasks.Item13, tasks.Item14, tasks.Item15, tasks.Item16).ConfigureAwait(options);
#endif
	#endregion
}
