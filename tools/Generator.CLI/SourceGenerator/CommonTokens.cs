using System.Linq;

namespace Generator.CLI.SourceGenerator
{
	public static class CommonTokens
	{
		/// <summary>
		/// Creates syntax which declares a type parameter list similar to <c>T1, T2, T3</c>.
		/// </summary>
		public static string GenericParameterList(int maxArity)
		{
			return Pattern(maxArity, "T{0}");
		}

		/// <summary>
		/// Creates syntax which declares a type parameter list similar to <c>Task&lt;T1&gt;, Task&lt;T2&gt;, Task&lt;T3&gt;</c>.
		/// </summary>
		public static string ListOfTaskOfEachTypeParameter(int maxArity)
		{
			return Pattern(maxArity, "Task<T{0}>");
		}

		/// <summary>
		/// Creates code according to the given pattern and separator. Produces <c>T1, T2, T3</c> for pattern <c>T{0}</c> with separator <c>, </c>
		/// </summary>
		public static string Pattern(int maxArity, string pattern, string separator = ", ")
		{
			return string.Join(separator, Enumerable.Range(1, maxArity).Select(d => string.Format(pattern, d)));
		}
	}
}
