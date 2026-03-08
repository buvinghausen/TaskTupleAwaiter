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
		var rootCache = new Dictionary<SyntaxTree, SyntaxNode>();
		foreach (var diagnostic in context.ReportedDiagnostics)
		{
			var tree = diagnostic.Location.SourceTree;
			if (tree is null)
				continue;

			if (!rootCache.TryGetValue(tree, out var root))
			{
				root = tree.GetRoot(context.CancellationToken);
				rootCache[tree] = root;
			}

			var node = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
			if (IsInsideAwaitedTuple(node))
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
		// Skip over any extra parentheses around the tuple expression
		var current = node.Parent;
		while (current is ParenthesizedExpressionSyntax)
		{
			current = current.Parent;
		}

		// Direct: await (vt1, vt2)
		if (current is AwaitExpressionSyntax)
			return true;

		// Via ConfigureAwait: await (vt1, vt2).ConfigureAwait(...)
		return current is MemberAccessExpressionSyntax
		{
			Name.Identifier.Text: "ConfigureAwait", Parent: InvocationExpressionSyntax
			{
				Parent: AwaitExpressionSyntax
			}
		};
	}
}
