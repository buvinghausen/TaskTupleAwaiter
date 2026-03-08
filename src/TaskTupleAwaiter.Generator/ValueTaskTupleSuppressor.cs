using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace TaskTupleAwaiter.Generator;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ValueTaskTupleSuppressor : DiagnosticSuppressor
{
	internal static readonly SuppressionDescriptor Rule = new(
		id: "TTA0001",
		suppressedDiagnosticId: "CA2012",
		justification:
		"ValueTask<T> used as an element of a tuple awaited via TaskTupleAwaiter is safely consumed via .AsTask().");

	public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions { get; } =
		[Rule];

	public override void ReportSuppressions(SuppressionAnalysisContext context)
	{
		foreach (var diagnostic in context.ReportedDiagnostics
			         .Select(diagnostic => new { diagnostic, tree = diagnostic.Location.SourceTree })
			         .Where(@t => @t.tree is not null)
			         .Select(@t => new { @t, root = @t.tree.GetRoot(context.CancellationToken) })
			         .Select(@t => new
			         {
				         @t,
				         node = @t.root.FindNode(@t.@t.diagnostic.Location.SourceSpan, getInnermostNodeForTie: true)
			         })
			         .Where(@t => IsInsideAwaitedTuple(@t.node))
			         .Select(@t => @t.@t.@t.diagnostic))
		{
			context.ReportSuppression(Suppression.Create(Rule, diagnostic));
		}
	}

	private static bool IsInsideAwaitedTuple(SyntaxNode node)
	{
		var current = node;
		while (current is not null)
		{
			if (current is TupleExpressionSyntax tupleExpr)
				return IsAwaited(tupleExpr);

			// Don't walk past statement boundaries
			if (current is StatementSyntax)
				break;

			current = current.Parent;
		}

		return false;
	}

	private static bool IsAwaited(SyntaxNode node)
	{
		var parent = node.Parent;
		// Direct: await (vt1, vt2)
		return parent is AwaitExpressionSyntax ||
			   // Via ConfigureAwait: await (vt1, vt2).ConfigureAwait(...)
			   parent is MemberAccessExpressionSyntax
			   {
				   Name.Identifier.Text: "ConfigureAwait", Parent: InvocationExpressionSyntax
				   {
					   Parent: AwaitExpressionSyntax
				   }
			   };
	}
}
