using System.Linq;

namespace Generator.CLI.SourceGenerator
{
	public static class CommonTokens
	{
		/// <summary>
		/// Returns T1, T2, T3 ...
		/// </summary>
		/// <param name="i"></param>
		/// <returns>Returns T1, T2, T3 ...</returns>
		public static string GenericParameterList(int i)
		{
			return string.Join(',', Enumerable.Range(1, i).Select(d => $"T{d}"));
		}

		/// <summary>
		/// Returns Task&lt;T1&gt;, Task&lt;T2&gt;, Task&lt;T3&gt; ...
		/// </summary>
		/// <param name="i"></param>
		/// <returns>Returns Task&lt;T1&gt;, Task&lt;T2&gt;, Task&lt;T3&gt; ...</returns>
		public static string TaskifiedList(int i)
		{
			return string.Join(',', Enumerable.Range(1, i).Select(d => $"Task<T{d}>"));
		}

		/// <summary>
		/// Returns Task&lt;T1&gt;, Task&lt;T2&gt;, Task&lt;T3&gt; ...
		/// </summary>
		/// <param name="i"></param>
		/// <returns>Returns Task&lt;T1&gt;, Task&lt;T2&gt;, Task&lt;T3&gt; ...</returns>
		public static string Pattern(int i, string pattern, string seperator = ", ")
		{
			return string.Join(seperator, Enumerable.Range(1, i).Select(d => string.Format(pattern, d)));
		}
	}
}
