using System.Text;
using static Generator.CLI.SourceGenerator.CommonTokens;

namespace Generator.CLI.SourceGenerator
{
	public static class TaskTupleAwaiterGenerator
	{
		public static string GenerateContent(int arity)
		{
			/**
			 * based on https://github.com/buvinghausen/TaskTupleAwaiter/blob/master/src/TaskTupleAwaiter/TaskTupleExtensions.cs
			 * https://github.com/buvinghausen/TaskTupleAwaiter/commit/ead4851edaf7553fc938a22377e48ff9f5c70cdc
			 */
			var sb = new StringBuilder();
			AddFileHeader(sb);
			AddArityOne(sb);
			if (arity > 1)
			{
				for (var i = 2; i < arity; i++)
				{
					AddGeneratedArity(sb, i);
				}

				sb.AppendLine($"#region Task");
				for (var i = 2; i < arity; i++)
				{
					AddTasks(sb, i);
				}
				sb.AppendLine($"#endregion Task");
			}

			// namespace close brace
			sb.Append("}");
			sb.Append("}");
			return sb.ToString();
		}

		private static void AddGeneratedArity(StringBuilder sb, int i)
		{
			sb.AppendLine($"#region (Task<T1>..Task<T{i}>)");
			AddGetAwaiter(sb, i);
			AddTupleTaskAwaiter(sb, i);
			AddTupleConfiguredTaskAwaitable(sb, i);
			sb.AppendLine($"#endregion (Task<T1>..Task<T{i}>)");
		}

		private static void AddTasks(StringBuilder sb, int i)
		{
			sb.Append($@"
		public static TaskAwaiter GetAwaiter(this ({Pattern(i, "Task")}) tasks) =>
			Task.WhenAll({Pattern(i, "tasks.Item{0}")}).GetAwaiter();

		public static ConfiguredTaskAwaitable ConfigureAwait(this ({Pattern(i, "Task")}) tasks,
			bool continueOnCapturedContext) => Task
			.WhenAll({Pattern(i, "tasks.Item{0}")})
.ConfigureAwait(continueOnCapturedContext);
");
		}

		private static void AddTupleConfiguredTaskAwaitable(StringBuilder sb, int i)
		{
			sb.Append($@"
		public static TupleConfiguredTaskAwaitable<{GenericParameterList(i)}>
			ConfigureAwait<{GenericParameterList(i)}>(this ({TaskifiedList(i)}) tasks,
				bool continueOnCapturedContext) =>
			new TupleConfiguredTaskAwaitable<{GenericParameterList(i)}>(tasks,
				continueOnCapturedContext);
");

			sb.Append($@"

public struct TupleConfiguredTaskAwaitable<{GenericParameterList(i)}>
{{
	private readonly ({TaskifiedList(i)}) _tasks;
	private readonly bool _continueOnCapturedContext;

	public TupleConfiguredTaskAwaitable(({TaskifiedList(i)}) tasks,
		bool continueOnCapturedContext)
	{{
		_tasks = tasks;
		_continueOnCapturedContext = continueOnCapturedContext;
	}}

	public Awaiter GetAwaiter() =>
		new Awaiter(_tasks, _continueOnCapturedContext);

	public struct Awaiter : ICriticalNotifyCompletion
	{{
		private readonly ({TaskifiedList(i)}) _tasks;
		private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _whenAllAwaiter;

		public Awaiter(({TaskifiedList(i)}) tasks,
			bool continueOnCapturedContext)
		{{
			_tasks = tasks;
			_whenAllAwaiter = Task.WhenAll({Pattern(i, "_tasks.Item{0}")})
				.ConfigureAwait(continueOnCapturedContext).GetAwaiter();
		}}

		public bool IsCompleted => _whenAllAwaiter.IsCompleted;

		public void OnCompleted(Action continuation) =>
			_whenAllAwaiter.OnCompleted(continuation);

		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation) =>
			_whenAllAwaiter.UnsafeOnCompleted(continuation);

		public ({GenericParameterList(i)}) GetResult()
		{{
			_whenAllAwaiter.GetResult();
			return ({Pattern(i, "_tasks.Item{0}.Result")});
		}}
	}}
}}
");
		}

		private static void AddTupleTaskAwaiter(StringBuilder sb, int i)
		{
			sb.Append($@"

public struct TupleTaskAwaiter<{GenericParameterList(i)}> : ICriticalNotifyCompletion
{{
	private readonly ({TaskifiedList(i)}) _tasks;
	private readonly TaskAwaiter _whenAllAwaiter;

	public TupleTaskAwaiter(({TaskifiedList(i)}) tasks)
	{{
		_tasks = tasks;
		_whenAllAwaiter = Task.WhenAll({Pattern(i, "tasks.Item{0}")})
			.GetAwaiter();
	}}

	public bool IsCompleted => _whenAllAwaiter.IsCompleted;

	public void OnCompleted(Action continuation) =>
		_whenAllAwaiter.OnCompleted(continuation);

	[SecurityCritical]
	public void UnsafeOnCompleted(Action continuation) =>
		_whenAllAwaiter.UnsafeOnCompleted(continuation);

	public ({GenericParameterList(i)}) GetResult()
	{{
		_whenAllAwaiter.GetResult();
		return ({Pattern(i, "_tasks.Item{0}.Result")});
	}}
}}
");
		}


		private static void AddGetAwaiter(StringBuilder sb, int i)
		{
			sb.Append($@"
		public static TupleTaskAwaiter<{GenericParameterList(i)}> GetAwaiter<{GenericParameterList(i)}>(
					this ({TaskifiedList(i)}) tasks) =>
					new TupleTaskAwaiter<{GenericParameterList(i)}>(tasks);
");
		}

		private static void AddFileHeader(StringBuilder sb)
		{
			sb.Append(@"
using System;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading.Tasks;
// ReSharper disable ImpureMethodCallOnReadonlyValueField

namespace TaskTupleAwaiter
{
	// Hopefully this will make its way into the compiler someday...
	// Ported from jnm2: https://gist.github.com/jnm2/3660db29457d391a34151f764bfe6ef7
	public static class TaskTupleExtensions
{
");
		}


		private static void AddArityOne(StringBuilder sb)
		{
			sb.Append(@"
#region (Task<T1>)
	public static TaskAwaiter<T1>
		GetAwaiter<T1>(this ValueTuple<Task<T1>> tasks) => tasks.Item1.GetAwaiter();

	public static ConfiguredTaskAwaitable<T1> ConfigureAwait<T1>(
		this ValueTuple<Task<T1>> tasks, bool continueOnCapturedContext) =>
		tasks.Item1.ConfigureAwait(continueOnCapturedContext);
#endregion
			");
		}
	}
}
