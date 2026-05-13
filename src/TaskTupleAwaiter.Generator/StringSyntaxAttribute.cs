// Polyfill for System.Diagnostics.CodeAnalysis.StringSyntaxAttribute (added in .NET 7).
// The generator project targets netstandard2.0, so the BCL definition isn't available.
// Roslyn's IDE classifiers recognise the attribute by namespace + type name (not assembly
// identity), so an internal declaration here drives the embedded-language hint in VS / Rider.
#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace System.Diagnostics.CodeAnalysis;
#pragma warning restore IDE0130 // Namespace does not match folder structure

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
sealed class StringSyntaxAttribute(string syntax) : Attribute
{
	public string Syntax { get; } = syntax;
}
