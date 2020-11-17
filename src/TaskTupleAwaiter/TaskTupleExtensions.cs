using System;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading.Tasks;

namespace TaskTupleAwaiter
{
	// Hopefully this will make its way into the compiler someday...
	// Ported from jnm2: https://gist.github.com/jnm2/3660db29457d391a34151f764bfe6ef7
	/// <summary>
	/// </summary>
	public static class TaskTupleExtensions
	{
		#region (Task<T1>)

		/// <summary>
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <param name="tasks"></param>
		/// <returns></returns>
		public static TaskAwaiter<T1> GetAwaiter<T1>(this ValueTuple<Task<T1>> tasks) => tasks.Item1.GetAwaiter();

		/// <summary>
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <param name="tasks"></param>
		/// <param name="continueOnCapturedContext"></param>
		/// <returns></returns>
		public static ConfiguredTaskAwaitable<T1> ConfigureAwait<T1>(this ValueTuple<Task<T1>> tasks,
			bool continueOnCapturedContext) => tasks.Item1.ConfigureAwait(continueOnCapturedContext);

		#endregion

		#region (Task<T1>..Task<T2>)

		/// <summary>
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <param name="tasks"></param>
		/// <returns></returns>
		public static TupleTaskAwaiter<T1, T2> GetAwaiter<T1, T2>(this (Task<T1>, Task<T2>) tasks) => new(tasks);

		/// <summary>
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		public struct TupleTaskAwaiter<T1, T2> : ICriticalNotifyCompletion
		{
			private readonly (Task<T1>, Task<T2>) _tasks;
			private TaskAwaiter _whenAllAwaiter;

			/// <summary>
			/// </summary>
			/// <param name="tasks"></param>
			public TupleTaskAwaiter((Task<T1>, Task<T2>) tasks)
			{
				_tasks = tasks;
				_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2).GetAwaiter();
			}

			/// <summary>
			/// </summary>
			public bool IsCompleted => _whenAllAwaiter.IsCompleted;

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <returns></returns>
			public (T1, T2) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result);
			}
		}

		/// <summary>
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <param name="tasks"></param>
		/// <param name="continueOnCapturedContext"></param>
		/// <returns></returns>
		public static TupleConfiguredTaskAwaitable<T1, T2> ConfigureAwait<T1, T2>(this (Task<T1>, Task<T2>) tasks,
			bool continueOnCapturedContext) => new(tasks, continueOnCapturedContext);

		/// <summary>
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		public readonly struct TupleConfiguredTaskAwaitable<T1, T2>
		{
			private readonly (Task<T1>, Task<T2>) _tasks;
			private readonly bool _continueOnCapturedContext;

			/// <summary>
			/// </summary>
			/// <param name="tasks"></param>
			/// <param name="continueOnCapturedContext"></param>
			public TupleConfiguredTaskAwaitable((Task<T1>, Task<T2>) tasks, bool continueOnCapturedContext)
			{
				_tasks = tasks;
				_continueOnCapturedContext = continueOnCapturedContext;
			}

			/// <summary>
			/// </summary>
			/// <returns></returns>
			public Awaiter GetAwaiter() => new(_tasks, _continueOnCapturedContext);

			/// <summary>
			/// </summary>
			public struct Awaiter : ICriticalNotifyCompletion
			{
				private readonly (Task<T1>, Task<T2>) _tasks;

				private ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _whenAllAwaiter;

				/// <summary>
				/// </summary>
				/// <param name="tasks"></param>
				/// <param name="continueOnCapturedContext"></param>
				public Awaiter((Task<T1>, Task<T2>) tasks, bool continueOnCapturedContext)
				{
					_tasks = tasks;
					_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2).ConfigureAwait(continueOnCapturedContext)
						.GetAwaiter();
				}

				/// <summary>
				/// </summary>
				public bool IsCompleted => _whenAllAwaiter.IsCompleted;

				/// <summary>
				/// </summary>
				/// <param name="continuation"></param>
				public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

				/// <summary>
				/// </summary>
				/// <param name="continuation"></param>
				[SecurityCritical]
				public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

				/// <summary>
				/// </summary>
				/// <returns></returns>
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
		public static TupleTaskAwaiter<T1, T2, T3> GetAwaiter<T1, T2, T3>(this (Task<T1>, Task<T2>, Task<T3>) tasks) => new(tasks);

		/// <summary>
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="T3"></typeparam>
		public struct TupleTaskAwaiter<T1, T2, T3> : ICriticalNotifyCompletion
		{
			private readonly (Task<T1>, Task<T2>, Task<T3>) _tasks;
			private TaskAwaiter _whenAllAwaiter;

			/// <summary>
			/// </summary>
			/// <param name="tasks"></param>
			public TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>) tasks)
			{
				_tasks = tasks;
				_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3).GetAwaiter();
			}

			/// <summary>
			/// </summary>
			public bool IsCompleted => _whenAllAwaiter.IsCompleted;

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

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
		public static TupleConfiguredTaskAwaitable<T1, T2, T3> ConfigureAwait<T1, T2, T3>(
			this (Task<T1>, Task<T2>, Task<T3>) tasks, bool continueOnCapturedContext) =>
			new(tasks, continueOnCapturedContext);

		/// <summary>
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="T3"></typeparam>
		public readonly struct TupleConfiguredTaskAwaitable<T1, T2, T3>
		{
			private readonly (Task<T1>, Task<T2>, Task<T3>) _tasks;
			private readonly bool _continueOnCapturedContext;

			/// <summary>
			/// </summary>
			/// <param name="tasks"></param>
			/// <param name="continueOnCapturedContext"></param>
			public TupleConfiguredTaskAwaitable((Task<T1>, Task<T2>, Task<T3>) tasks, bool continueOnCapturedContext)
			{
				_tasks = tasks;
				_continueOnCapturedContext = continueOnCapturedContext;
			}

			/// <summary>
			/// </summary>
			/// <returns></returns>
			public Awaiter GetAwaiter() => new(_tasks, _continueOnCapturedContext);

			/// <summary>
			/// </summary>
			public struct Awaiter : ICriticalNotifyCompletion
			{
				private readonly (Task<T1>, Task<T2>, Task<T3>) _tasks;

				private ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _whenAllAwaiter;

				/// <summary>
				/// </summary>
				/// <param name="tasks"></param>
				/// <param name="continueOnCapturedContext"></param>
				public Awaiter((Task<T1>, Task<T2>, Task<T3>) tasks, bool continueOnCapturedContext)
				{
					_tasks = tasks;
					_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3)
						.ConfigureAwait(continueOnCapturedContext).GetAwaiter();
				}

				/// <summary>
				/// </summary>
				public bool IsCompleted => _whenAllAwaiter.IsCompleted;

				/// <summary>
				/// </summary>
				/// <param name="continuation"></param>
				public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

				/// <summary>
				/// </summary>
				/// <param name="continuation"></param>
				[SecurityCritical]
				public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

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
		public static TupleTaskAwaiter<T1, T2, T3, T4> GetAwaiter<T1, T2, T3, T4>(
			this (Task<T1>, Task<T2>, Task<T3>, Task<T4>) tasks) => new(tasks);

		/// <summary>
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="T3"></typeparam>
		/// <typeparam name="T4"></typeparam>
		public struct TupleTaskAwaiter<T1, T2, T3, T4> : ICriticalNotifyCompletion
		{
			private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>) _tasks;
			private TaskAwaiter _whenAllAwaiter;

			/// <summary>
			/// </summary>
			/// <param name="tasks"></param>
			public TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>) tasks)
			{
				_tasks = tasks;
				_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4).GetAwaiter();
			}

			/// <summary>
			/// </summary>
			public bool IsCompleted => _whenAllAwaiter.IsCompleted;

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

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
		public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4> ConfigureAwait<T1, T2, T3, T4>(
			this (Task<T1>, Task<T2>, Task<T3>, Task<T4>) tasks, bool continueOnCapturedContext) =>
			new(tasks, continueOnCapturedContext);

		/// <summary>
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="T3"></typeparam>
		/// <typeparam name="T4"></typeparam>
		public readonly struct TupleConfiguredTaskAwaitable<T1, T2, T3, T4>
		{
			private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>) _tasks;
			private readonly bool _continueOnCapturedContext;

			/// <summary>
			/// </summary>
			/// <param name="tasks"></param>
			/// <param name="continueOnCapturedContext"></param>
			public TupleConfiguredTaskAwaitable((Task<T1>, Task<T2>, Task<T3>, Task<T4>) tasks,
				bool continueOnCapturedContext)
			{
				_tasks = tasks;
				_continueOnCapturedContext = continueOnCapturedContext;
			}

			/// <summary>
			/// </summary>
			/// <returns></returns>
			public Awaiter GetAwaiter() => new(_tasks, _continueOnCapturedContext);

			/// <summary>
			/// </summary>
			public struct Awaiter : ICriticalNotifyCompletion
			{
				private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>) _tasks;

				private ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _whenAllAwaiter;

				/// <summary>
				/// </summary>
				/// <param name="tasks"></param>
				/// <param name="continueOnCapturedContext"></param>
				public Awaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>) tasks, bool continueOnCapturedContext)
				{
					_tasks = tasks;
					_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4)
						.ConfigureAwait(continueOnCapturedContext).GetAwaiter();
				}

				/// <summary>
				/// </summary>
				public bool IsCompleted => _whenAllAwaiter.IsCompleted;

				/// <summary>
				/// </summary>
				/// <param name="continuation"></param>
				public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

				/// <summary>
				/// </summary>
				/// <param name="continuation"></param>
				[SecurityCritical]
				public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

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
		public static TupleTaskAwaiter<T1, T2, T3, T4, T5> GetAwaiter<T1, T2, T3, T4, T5>(
			this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) tasks) => new(tasks);

		/// <summary>
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="T3"></typeparam>
		/// <typeparam name="T4"></typeparam>
		/// <typeparam name="T5"></typeparam>
		public struct TupleTaskAwaiter<T1, T2, T3, T4, T5> : ICriticalNotifyCompletion
		{
			private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) _tasks;

			private TaskAwaiter _whenAllAwaiter;

			/// <summary>
			/// </summary>
			/// <param name="tasks"></param>
			public TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) tasks)
			{
				_tasks = tasks;
				_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5)
					.GetAwaiter();
			}

			/// <summary>
			/// </summary>
			public bool IsCompleted => _whenAllAwaiter.IsCompleted;

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <returns></returns>
			public (T1, T2, T3, T4, T5) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result,
					_tasks.Item5.Result);
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
		public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5> ConfigureAwait<T1, T2, T3, T4, T5>(
			this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) tasks, bool continueOnCapturedContext) =>
			new(tasks, continueOnCapturedContext);

		/// <summary>
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="T3"></typeparam>
		/// <typeparam name="T4"></typeparam>
		/// <typeparam name="T5"></typeparam>
		public readonly struct TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5>
		{
			private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) _tasks;

			private readonly bool _continueOnCapturedContext;

			/// <summary>
			/// </summary>
			/// <param name="tasks"></param>
			/// <param name="continueOnCapturedContext"></param>
			public TupleConfiguredTaskAwaitable((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) tasks,
				bool continueOnCapturedContext)
			{
				_tasks = tasks;
				_continueOnCapturedContext = continueOnCapturedContext;
			}

			/// <summary>
			/// </summary>
			/// <returns></returns>
			public Awaiter GetAwaiter() => new(_tasks, _continueOnCapturedContext);

			/// <summary>
			/// </summary>
			public struct Awaiter : ICriticalNotifyCompletion
			{
				private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) _tasks;

				private ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _whenAllAwaiter;

				/// <summary>
				/// </summary>
				/// <param name="tasks"></param>
				/// <param name="continueOnCapturedContext"></param>
				public Awaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) tasks, bool continueOnCapturedContext)
				{
					_tasks = tasks;
					_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5)
						.ConfigureAwait(continueOnCapturedContext).GetAwaiter();
				}

				/// <summary>
				/// </summary>
				public bool IsCompleted => _whenAllAwaiter.IsCompleted;

				/// <summary>
				/// </summary>
				/// <param name="continuation"></param>
				public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

				/// <summary>
				/// </summary>
				/// <param name="continuation"></param>
				[SecurityCritical]
				public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

				/// <summary>
				/// </summary>
				/// <returns></returns>
				public (T1, T2, T3, T4, T5) GetResult()
				{
					_whenAllAwaiter.GetResult();
					return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result,
						_tasks.Item5.Result);
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
		public static TupleTaskAwaiter<T1, T2, T3, T4, T5, T6> GetAwaiter<T1, T2, T3, T4, T5, T6>(
			this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>) tasks) => new(tasks);

		/// <summary>
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="T3"></typeparam>
		/// <typeparam name="T4"></typeparam>
		/// <typeparam name="T5"></typeparam>
		/// <typeparam name="T6"></typeparam>
		public struct TupleTaskAwaiter<T1, T2, T3, T4, T5, T6> : ICriticalNotifyCompletion
		{
			private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>) _tasks;

			private TaskAwaiter _whenAllAwaiter;

			/// <summary>
			/// </summary>
			/// <param name="tasks"></param>
			public TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>) tasks)
			{
				_tasks = tasks;
				_whenAllAwaiter =
					Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6)
						.GetAwaiter();
			}

			/// <summary>
			/// </summary>
			public bool IsCompleted => _whenAllAwaiter.IsCompleted;

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <returns></returns>
			public (T1, T2, T3, T4, T5, T6) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result,
					_tasks.Item5.Result, _tasks.Item6.Result);
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
		public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6> ConfigureAwait<T1, T2, T3, T4, T5, T6>(
			this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>) tasks, bool continueOnCapturedContext) =>
			new(tasks, continueOnCapturedContext);

		/// <summary>
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="T3"></typeparam>
		/// <typeparam name="T4"></typeparam>
		/// <typeparam name="T5"></typeparam>
		/// <typeparam name="T6"></typeparam>
		public readonly struct TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6>
		{
			private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>) _tasks;

			private readonly bool _continueOnCapturedContext;

			/// <summary>
			/// </summary>
			/// <param name="tasks"></param>
			/// <param name="continueOnCapturedContext"></param>
			public TupleConfiguredTaskAwaitable((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>) tasks,
				bool continueOnCapturedContext)
			{
				_tasks = tasks;
				_continueOnCapturedContext = continueOnCapturedContext;
			}

			/// <summary>
			/// </summary>
			/// <returns></returns>
			public Awaiter GetAwaiter() => new(_tasks, _continueOnCapturedContext);

			/// <summary>
			/// </summary>
			public struct Awaiter : ICriticalNotifyCompletion
			{
				private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>) _tasks;

				private ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _whenAllAwaiter;

				/// <summary>
				/// </summary>
				/// <param name="tasks"></param>
				/// <param name="continueOnCapturedContext"></param>
				public Awaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>) tasks,
					bool continueOnCapturedContext)
				{
					_tasks = tasks;
					_whenAllAwaiter =
						Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6)
							.ConfigureAwait(continueOnCapturedContext).GetAwaiter();
				}

				/// <summary>
				/// </summary>
				public bool IsCompleted => _whenAllAwaiter.IsCompleted;

				/// <summary>
				/// </summary>
				/// <param name="continuation"></param>
				public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

				/// <summary>
				/// </summary>
				/// <param name="continuation"></param>
				[SecurityCritical]
				public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

				/// <summary>
				/// </summary>
				/// <returns></returns>
				public (T1, T2, T3, T4, T5, T6) GetResult()
				{
					_whenAllAwaiter.GetResult();
					return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result,
						_tasks.Item5.Result, _tasks.Item6.Result);
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
		public static TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7> GetAwaiter<T1, T2, T3, T4, T5, T6, T7>(
			this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>) tasks) => new(tasks);

		/// <summary>
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="T3"></typeparam>
		/// <typeparam name="T4"></typeparam>
		/// <typeparam name="T5"></typeparam>
		/// <typeparam name="T6"></typeparam>
		/// <typeparam name="T7"></typeparam>
		public struct TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7> : ICriticalNotifyCompletion
		{
			private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>) _tasks;

			private TaskAwaiter _whenAllAwaiter;

			/// <summary>
			/// </summary>
			/// <param name="tasks"></param>
			public TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>) tasks)
			{
				_tasks = tasks;
				_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5,
					tasks.Item6, tasks.Item7).GetAwaiter();
			}

			/// <summary>
			/// </summary>
			public bool IsCompleted => _whenAllAwaiter.IsCompleted;

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <returns></returns>
			public (T1, T2, T3, T4, T5, T6, T7) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result,
					_tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result);
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
		public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7>
			ConfigureAwait<T1, T2, T3, T4, T5, T6, T7>(
				this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>) tasks,
				bool continueOnCapturedContext) => new(tasks, continueOnCapturedContext);

		/// <summary>
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="T3"></typeparam>
		/// <typeparam name="T4"></typeparam>
		/// <typeparam name="T5"></typeparam>
		/// <typeparam name="T6"></typeparam>
		/// <typeparam name="T7"></typeparam>
		public readonly struct TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7>
		{
			private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>) _tasks;

			private readonly bool _continueOnCapturedContext;

			/// <summary>
			/// </summary>
			/// <param name="tasks"></param>
			/// <param name="continueOnCapturedContext"></param>
			public TupleConfiguredTaskAwaitable(
				(Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>) tasks,
				bool continueOnCapturedContext)
			{
				_tasks = tasks;
				_continueOnCapturedContext = continueOnCapturedContext;
			}

			/// <summary>
			/// </summary>
			/// <returns></returns>
			public Awaiter GetAwaiter() => new(_tasks, _continueOnCapturedContext);

			/// <summary>
			/// </summary>
			public struct Awaiter : ICriticalNotifyCompletion
			{
				private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>) _tasks;

				private ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _whenAllAwaiter;

				/// <summary>
				/// </summary>
				/// <param name="tasks"></param>
				/// <param name="continueOnCapturedContext"></param>
				public Awaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>) tasks,
					bool continueOnCapturedContext)
				{
					_tasks = tasks;
					_whenAllAwaiter =
						Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6,
							tasks.Item7).ConfigureAwait(continueOnCapturedContext).GetAwaiter();
				}

				/// <summary>
				/// </summary>
				public bool IsCompleted => _whenAllAwaiter.IsCompleted;

				/// <summary>
				/// </summary>
				/// <param name="continuation"></param>
				public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

				/// <summary>
				/// </summary>
				/// <param name="continuation"></param>
				[SecurityCritical]
				public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

				/// <summary>
				/// </summary>
				/// <returns></returns>
				public (T1, T2, T3, T4, T5, T6, T7) GetResult()
				{
					_whenAllAwaiter.GetResult();
					return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result,
						_tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result);
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
		public static TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8> GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8>(
			this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>) tasks) => new(tasks);

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
		public struct TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8> : ICriticalNotifyCompletion
		{
			private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>) _tasks;

			private TaskAwaiter _whenAllAwaiter;

			/// <summary>
			/// </summary>
			/// <param name="tasks"></param>
			public TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>) tasks)
			{
				_tasks = tasks;
				_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8).GetAwaiter();
			}

			/// <summary>
			/// </summary>
			public bool IsCompleted => _whenAllAwaiter.IsCompleted;

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

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
		public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8>
			ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8>(
				this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>) tasks,
				bool continueOnCapturedContext) => new(tasks, continueOnCapturedContext);

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
		public readonly struct TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8>
		{
			private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>) _tasks;

			private readonly bool _continueOnCapturedContext;

			/// <summary>
			/// </summary>
			/// <param name="tasks"></param>
			/// <param name="continueOnCapturedContext"></param>
			public TupleConfiguredTaskAwaitable(
				(Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>) tasks,
				bool continueOnCapturedContext)
			{
				_tasks = tasks;
				_continueOnCapturedContext = continueOnCapturedContext;
			}

			/// <summary>
			/// </summary>
			/// <returns></returns>
			public Awaiter GetAwaiter() => new(_tasks, _continueOnCapturedContext);

			/// <summary>
			/// </summary>
			public struct Awaiter : ICriticalNotifyCompletion
			{
				private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>)
					_tasks;

				private ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _whenAllAwaiter;

				/// <summary>
				/// </summary>
				/// <param name="tasks"></param>
				/// <param name="continueOnCapturedContext"></param>
				public Awaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>) tasks,
					bool continueOnCapturedContext)
				{
					_tasks = tasks;
					_whenAllAwaiter =
						Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6,
							tasks.Item7, tasks.Item8).ConfigureAwait(continueOnCapturedContext).GetAwaiter();
				}

				/// <summary>
				/// </summary>
				public bool IsCompleted => _whenAllAwaiter.IsCompleted;

				/// <summary>
				/// </summary>
				/// <param name="continuation"></param>
				public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

				/// <summary>
				/// </summary>
				/// <param name="continuation"></param>
				[SecurityCritical]
				public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

				/// <summary>
				/// </summary>
				/// <returns></returns>
				public (T1, T2, T3, T4, T5, T6, T7, T8) GetResult()
				{
					_whenAllAwaiter.GetResult();
					return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result,
						_tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result);
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
		public static TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9>
			GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
				this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>)
					tasks) => new(tasks);

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
		public struct TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9> : ICriticalNotifyCompletion
		{
			private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>)
				_tasks;

			private TaskAwaiter _whenAllAwaiter;

			/// <summary>
			/// </summary>
			/// <param name="tasks"></param>
			public TupleTaskAwaiter(
				(Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>) tasks)
			{
				_tasks = tasks;
				_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5,
					tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9).GetAwaiter();
			}

			/// <summary>
			/// </summary>
			public bool IsCompleted => _whenAllAwaiter.IsCompleted;

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <returns></returns>
			public (T1, T2, T3, T4, T5, T6, T7, T8, T9) GetResult()
			{
				_whenAllAwaiter.GetResult();
				return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result,
					_tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result,
					_tasks.Item9.Result);
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
		public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9>
			ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
				this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>) tasks,
				bool continueOnCapturedContext) => new(tasks, continueOnCapturedContext);

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
		public readonly struct TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9>
		{
			private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>)
				_tasks;

			private readonly bool _continueOnCapturedContext;

			/// <summary>
			/// </summary>
			/// <param name="tasks"></param>
			/// <param name="continueOnCapturedContext"></param>
			public TupleConfiguredTaskAwaitable(
				(Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>) tasks,
				bool continueOnCapturedContext)
			{
				_tasks = tasks;
				_continueOnCapturedContext = continueOnCapturedContext;
			}

			/// <summary>
			/// </summary>
			/// <returns></returns>
			public Awaiter GetAwaiter() => new(_tasks, _continueOnCapturedContext);

			/// <summary>
			/// </summary>
			public struct Awaiter : ICriticalNotifyCompletion
			{
				private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>,
					Task<T9>) _tasks;

				private ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _whenAllAwaiter;

				/// <summary>
				/// </summary>
				/// <param name="tasks"></param>
				/// <param name="continueOnCapturedContext"></param>
				public Awaiter(
					(Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>) tasks,
					bool continueOnCapturedContext)
				{
					_tasks = tasks;
					_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5,
							tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9)
						.ConfigureAwait(continueOnCapturedContext).GetAwaiter();
				}

				/// <summary>
				/// </summary>
				public bool IsCompleted => _whenAllAwaiter.IsCompleted;

				/// <summary>
				/// </summary>
				/// <param name="continuation"></param>
				public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

				/// <summary>
				/// </summary>
				/// <param name="continuation"></param>
				[SecurityCritical]
				public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

				/// <summary>
				/// </summary>
				/// <returns></returns>
				public (T1, T2, T3, T4, T5, T6, T7, T8, T9) GetResult()
				{
					_whenAllAwaiter.GetResult();
					return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result,
						_tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result,
						_tasks.Item9.Result);
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
		public static TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
			GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
				this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>,
					Task<T10>) tasks) => new(tasks);

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
		public struct TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : ICriticalNotifyCompletion
		{
			private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>) _tasks;

			private TaskAwaiter _whenAllAwaiter;

			/// <summary>
			/// </summary>
			/// <param name="tasks"></param>
			public TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>) tasks)
			{
				_tasks = tasks;
				_whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10).GetAwaiter();
			}

			/// <summary>
			/// </summary>
			public bool IsCompleted => _whenAllAwaiter.IsCompleted;

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

			/// <summary>
			/// </summary>
			/// <param name="continuation"></param>
			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

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
		public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
			ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
				this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>,
					Task<T10>) tasks, bool continueOnCapturedContext) => new(tasks, continueOnCapturedContext);

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
		public readonly struct TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
		{
			private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>,
				Task<T10>) _tasks;

			private readonly bool _continueOnCapturedContext;

			/// <summary>
			/// </summary>
			/// <param name="tasks"></param>
			/// <param name="continueOnCapturedContext"></param>
			public TupleConfiguredTaskAwaitable(
				(Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>)
					tasks, bool continueOnCapturedContext)
			{
				_tasks = tasks;
				_continueOnCapturedContext = continueOnCapturedContext;
			}

			/// <summary>
			/// </summary>
			/// <returns></returns>
			public Awaiter GetAwaiter() => new(_tasks, _continueOnCapturedContext);

			/// <summary>
			/// </summary>
			public struct Awaiter : ICriticalNotifyCompletion
			{
				private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>,
					Task<T9>, Task<T10>) _tasks;

				private ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _whenAllAwaiter;

				/// <summary>
				/// </summary>
				/// <param name="tasks"></param>
				/// <param name="continueOnCapturedContext"></param>
				public Awaiter(
					(Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>
						) tasks, bool continueOnCapturedContext)
				{
					_tasks = tasks;
					_whenAllAwaiter =
						Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6,
								tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10)
							.ConfigureAwait(continueOnCapturedContext).GetAwaiter();
				}

				/// <summary>
				/// </summary>
				public bool IsCompleted => _whenAllAwaiter.IsCompleted;

				/// <summary>
				/// </summary>
				/// <param name="continuation"></param>
				public void OnCompleted(Action continuation) => _whenAllAwaiter.OnCompleted(continuation);

				/// <summary>
				/// </summary>
				/// <param name="continuation"></param>
				[SecurityCritical]
				public void UnsafeOnCompleted(Action continuation) => _whenAllAwaiter.UnsafeOnCompleted(continuation);

				/// <summary>
				/// </summary>
				/// <returns></returns>
				public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10) GetResult()
				{
					_whenAllAwaiter.GetResult();
					return (_tasks.Item1.Result, _tasks.Item2.Result, _tasks.Item3.Result, _tasks.Item4.Result,
						_tasks.Item5.Result, _tasks.Item6.Result, _tasks.Item7.Result, _tasks.Item8.Result,
						_tasks.Item9.Result, _tasks.Item10.Result);
				}
			}
		}

		#endregion

		#region Task

		/// <summary>
		/// </summary>
		/// <param name="tasks"></param>
		/// <returns></returns>
		public static TaskAwaiter GetAwaiter(this ValueTuple<Task> tasks) => tasks.Item1.GetAwaiter();

		/// <summary>
		/// </summary>
		/// <param name="tasks"></param>
		/// <param name="continueOnCapturedContext"></param>
		/// <returns></returns>
		public static ConfiguredTaskAwaitable ConfigureAwait(this ValueTuple<Task> tasks,
			bool continueOnCapturedContext) => tasks.Item1.ConfigureAwait(continueOnCapturedContext);

		/// <summary>
		/// </summary>
		/// <param name="tasks"></param>
		/// <returns></returns>
		public static TaskAwaiter GetAwaiter(this (Task, Task) tasks) =>
			Task.WhenAll(tasks.Item1, tasks.Item2).GetAwaiter();

		/// <summary>
		/// </summary>
		/// <param name="tasks"></param>
		/// <param name="continueOnCapturedContext"></param>
		/// <returns></returns>
		public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task) tasks, bool continueOnCapturedContext) =>
			Task.WhenAll(tasks.Item1, tasks.Item2).ConfigureAwait(continueOnCapturedContext);

		/// <summary>
		/// </summary>
		/// <param name="tasks"></param>
		/// <returns></returns>
		public static TaskAwaiter GetAwaiter(this (Task, Task, Task) tasks) =>
			Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3).GetAwaiter();

		/// <summary>
		/// </summary>
		/// <param name="tasks"></param>
		/// <param name="continueOnCapturedContext"></param>
		/// <returns></returns>
		public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task) tasks,
			bool continueOnCapturedContext) => Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3)
			.ConfigureAwait(continueOnCapturedContext);

		/// <summary>
		/// </summary>
		/// <param name="tasks"></param>
		/// <returns></returns>
		public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task) tasks) =>
			Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4).GetAwaiter();

		/// <summary>
		/// </summary>
		/// <param name="tasks"></param>
		/// <param name="continueOnCapturedContext"></param>
		/// <returns></returns>
		public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task) tasks,
			bool continueOnCapturedContext) => Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4)
			.ConfigureAwait(continueOnCapturedContext);

		/// <summary>
		/// </summary>
		/// <param name="tasks"></param>
		/// <returns></returns>
		public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task) tasks) =>
			Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5).GetAwaiter();

		/// <summary>
		/// </summary>
		/// <param name="tasks"></param>
		/// <param name="continueOnCapturedContext"></param>
		/// <returns></returns>
		public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task) tasks,
			bool continueOnCapturedContext) => Task
			.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5)
			.ConfigureAwait(continueOnCapturedContext);

		/// <summary>
		/// </summary>
		/// <param name="tasks"></param>
		/// <returns></returns>
		public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task) tasks) => Task
			.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6).GetAwaiter();

		/// <summary>
		/// </summary>
		/// <param name="tasks"></param>
		/// <param name="continueOnCapturedContext"></param>
		/// <returns></returns>
		public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task) tasks,
			bool continueOnCapturedContext) => Task
			.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6)
			.ConfigureAwait(continueOnCapturedContext);

		/// <summary>
		/// </summary>
		/// <param name="tasks"></param>
		/// <returns></returns>
		public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task, Task) tasks) => Task
			.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7)
			.GetAwaiter();

		/// <summary>
		/// </summary>
		/// <param name="tasks"></param>
		/// <param name="continueOnCapturedContext"></param>
		/// <returns></returns>
		public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task) tasks,
			bool continueOnCapturedContext) => Task
			.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7)
			.ConfigureAwait(continueOnCapturedContext);

		/// <summary>
		/// </summary>
		/// <param name="tasks"></param>
		/// <returns></returns>
		public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task, Task, Task) tasks) => Task
			.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7,
				tasks.Item8).GetAwaiter();

		/// <summary>
		/// </summary>
		/// <param name="tasks"></param>
		/// <param name="continueOnCapturedContext"></param>
		/// <returns></returns>
		public static ConfiguredTaskAwaitable
			ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task) tasks,
				bool continueOnCapturedContext) => Task
			.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7,
				tasks.Item8).ConfigureAwait(continueOnCapturedContext);

		/// <summary>
		/// </summary>
		/// <param name="tasks"></param>
		/// <returns></returns>
		public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks) => Task
			.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7,
				tasks.Item8, tasks.Item9).GetAwaiter();

		/// <summary>
		/// </summary>
		/// <param name="tasks"></param>
		/// <param name="continueOnCapturedContext"></param>
		/// <returns></returns>
		public static ConfiguredTaskAwaitable ConfigureAwait(
			this (Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, bool continueOnCapturedContext) => Task
			.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7,
				tasks.Item8, tasks.Item9).ConfigureAwait(continueOnCapturedContext);

		/// <summary>
		/// </summary>
		/// <param name="tasks"></param>
		/// <returns></returns>
		public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks) =>
			Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7,
				tasks.Item8, tasks.Item9, tasks.Item10).GetAwaiter();

		/// <summary>
		/// </summary>
		/// <param name="tasks"></param>
		/// <param name="continueOnCapturedContext"></param>
		/// <returns></returns>
		public static ConfiguredTaskAwaitable ConfigureAwait(
			this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, bool continueOnCapturedContext) =>
			Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7,
				tasks.Item8, tasks.Item9, tasks.Item10).ConfigureAwait(continueOnCapturedContext);
		#endregion
	}
}
