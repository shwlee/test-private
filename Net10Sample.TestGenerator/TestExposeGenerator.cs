using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace Net10Sample.TestGenerator;

[Generator]
public sealed class TestExposeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var methods = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) =>
                    node is MethodDeclarationSyntax method &&
                    method.AttributeLists.Count > 0,

                transform: static (ctx, _) =>
                {
                    var method = (MethodDeclarationSyntax)ctx.Node;

                    return ctx.SemanticModel.GetDeclaredSymbol(method)
                        as IMethodSymbol;
                })
            .Where(static method => method is not null)
            .Select(static (method, _) => method!);

        context.RegisterSourceOutput(
            methods.Collect(),
            static (spc, methods) =>
            {
                Generate(spc, methods);
            });
    }

    private static void Generate(
        SourceProductionContext context,
        ImmutableArray<IMethodSymbol> methods)
    {
        var targetMethods = methods
            .Where(HasTestExposeAttribute)
            .Where(IsPrivate)
            .GroupBy(m => m.ContainingType, SymbolEqualityComparer.Default);

        foreach (var group in targetMethods)
        {
            GenerateForType(
                context,
                (INamedTypeSymbol)group.Key!,
                group);
        }
    }

    private static bool HasTestExposeAttribute(IMethodSymbol method)
    {
        return method.GetAttributes()
            .Any(attribute =>
                attribute.AttributeClass?.ToDisplayString().Contains("TestExposeAttribute") ?? false);
    }

    private static bool IsPrivate(IMethodSymbol method)
    {
        return method.DeclaredAccessibility ==
               Accessibility.Private;
    }

    private static void GenerateForType(
        SourceProductionContext context,
        INamedTypeSymbol type,
        IEnumerable<IMethodSymbol> methods)
    {
        var source = new StringBuilder();

        source.AppendLine("#if TEST_EXPOSE");
        source.AppendLine();

        var namespaceName = type.ContainingNamespace.IsGlobalNamespace
            ? null
            : type.ContainingNamespace.ToDisplayString();

        if (namespaceName is not null)
        {
            source.Append("namespace ")
                  .Append(namespaceName)
                  .AppendLine(";");
            source.AppendLine();
        }

        source.Append("public partial class ")
              .Append(type.Name)
              .AppendLine();
        source.AppendLine("{");

        foreach (var method in methods)
        {
            GenerateWrapper(source, method);
        }

        source.AppendLine("}");
        source.AppendLine();
        source.AppendLine("#endif");

        var hintName =
            $"{type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                .Replace("global::", "")
                .Replace('.', '_')}.TestExpose.g.cs";

        context.AddSource(
            hintName,
            SourceText.From(source.ToString(), Encoding.UTF8));
    }

    private static void GenerateWrapper(
        StringBuilder source,
        IMethodSymbol method)
    {
        var returnType = method.ReturnType.ToDisplayString();

        var parameters = string.Join(
            ", ",
            method.Parameters.Select(FormatParameter));

        var arguments = string.Join(
            ", ",
            method.Parameters.Select(p => p.Name));

        source.Append("    public ")
              .Append(returnType)
              .Append(" Test_")
              .Append(method.Name)
              .Append("(")
              .Append(parameters)
              .AppendLine(")")
              .AppendLine("    {");

        if (method.ReturnsVoid)
        {
            source.Append("        ")
                  .Append(method.Name)
                  .Append("(")
                  .Append(arguments)
                  .AppendLine(");");
        }
        else
        {
            source.Append("        return ")
                  .Append(method.Name)
                  .Append("(")
                  .Append(arguments)
                  .AppendLine(");");
        }

        source.AppendLine("    }");
        source.AppendLine();
    }

    private static string FormatParameter(
        IParameterSymbol parameter)
    {
        var modifier = parameter.RefKind switch
        {
            RefKind.Ref => "ref ",
            RefKind.Out => "out ",
            RefKind.In => "in ",
            _ => ""
        };

        return $"{modifier}{parameter.Type.ToDisplayString()} {parameter.Name}";
    }
}
