using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MessagePackFingerprintGenerator;

public static class GeneratorPipelines
{
    public static IncrementalValueProvider<HashSet<string>> ConfigProvider(
        IncrementalGeneratorInitializationContext context)
    {
        var configProvider = context.AnalyzerConfigOptionsProvider
            .Select((provider, _) =>
            {
                var options = provider.GlobalOptions;
                if (options.TryGetValue("build_property.MessagePackFingerprintReferencedWithGenerator", out var raw))
                {
                    return [..raw.Split(',')];
                }

                return new HashSet<string>();
            });

        return configProvider;
    }

    public static IncrementalValueProvider<ImmutableArray<INamedTypeSymbol>> GetMessagePackTypes(
        IncrementalGeneratorInitializationContext context)
    {
        // Helper to collect named type symbols from syntax nodes
        IncrementalValueProvider<ImmutableArray<INamedTypeSymbol>> CollectSymbols(
            Func<SyntaxNode, System.Threading.CancellationToken, bool> predicate,
            Func<GeneratorSyntaxContext, System.Threading.CancellationToken, INamedTypeSymbol?> transform)
        {
            return context.SyntaxProvider
                .CreateSyntaxProvider(predicate, transform)
                .Where(sym => sym is not null)
                .Select((sym, _) => sym!)
                .WithComparer(SymbolEqualityComparer.Default)
                .Collect();
        }

        // 1) All explicit TypeSyntax occurrences
        var typeSyntax = CollectSymbols(
            static (node, _) => node is TypeSyntax,
            static (ctx, _) => ctx.SemanticModel.GetTypeInfo((TypeSyntax)ctx.Node).Type as INamedTypeSymbol);

        // 2) All object/array creation expressions
        var creations = CollectSymbols(
            static (node, _) =>
                node is ObjectCreationExpressionSyntax
                or ImplicitObjectCreationExpressionSyntax
                or ArrayCreationExpressionSyntax
                or ImplicitArrayCreationExpressionSyntax,
            static (ctx, _) => ctx.SemanticModel.GetTypeInfo((ExpressionSyntax)ctx.Node).Type as INamedTypeSymbol);

        // 3) Own MessagePack types declared in this project
        var ownTypes = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => s is TypeDeclarationSyntax { AttributeLists.Count: > 0 },
                transform: static (ctx, _) => GetSemanticTarget(ctx))
            .Where(t => t is not null)
            .Select((t, _) => t!)
            .WithComparer(SymbolEqualityComparer.Default)
            .Collect();

        // 4) Combine all sources, filter and remove duplicates in one pass
        var allTypes = typeSyntax
            .Combine(creations)
            .Combine(ownTypes)
            .Select((tuple, _) =>
            {
                var (first, second) = tuple.Left;
                var own = tuple.Right;
                return first
                    .Concat(second)
                    .Concat(own)
                    .Where(sym => sym.GetAttributes().Any(a => a.AttributeClass?.Name == "MessagePackObjectAttribute"))
                    .Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default)
                    .ToImmutableArray();
            })
            .WithTrackingName("AllMessagePackTypes");

        return allTypes;
    }

    private static INamedTypeSymbol? GetSemanticTarget(GeneratorSyntaxContext context)
    {
        var classDecl = (TypeDeclarationSyntax)context.Node;
        var symbol = context.SemanticModel.GetDeclaredSymbol(classDecl);
        if (symbol is null) return null;

        foreach (var attr in symbol.GetAttributes())
        {
            if (attr.AttributeClass?.ToDisplayString() == "MessagePack.MessagePackObjectAttribute")
            {
                return symbol as INamedTypeSymbol;
            }
        }

        return null;
    }
}