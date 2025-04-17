using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace MessagePackFingerprintGenerator;

public static class FingerprintHelper
{
    public static string ComputeFingerprint(INamedTypeSymbol type, HashSet<string> referencedWithGenerator)
    {
        var sb = new StringBuilder();
        
        AppendTypeFingerprint(type, referencedWithGenerator, sb, new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default));
        
        return sb.ToString();
    }

    private static void AppendTypeFingerprint(
        INamedTypeSymbol type,
        HashSet<string> referencedWithGenerator,
        StringBuilder sb,
        HashSet<INamedTypeSymbol> visited)
    {
        if (!visited.Add(type))
            return;

        sb.AppendLine($"TYPE:{type.ToDisplayString()}");

        if (!IsMessagePackObject(type))
            return;

        var unionAttrs = type.GetAttributes()
            .Where(a => a.AttributeClass?.Name == "UnionAttribute");

        foreach (var unionAttr in unionAttrs)
        {
            if (unionAttr.ConstructorArguments.Length == 2)
            {
                var tagArg = unionAttr.ConstructorArguments[0];
                var typeArg = unionAttr.ConstructorArguments[1];

                if (tagArg.Value is int tag && typeArg.Value is INamedTypeSymbol unionType)
                {
                    sb.AppendLine($"UNION:{tag}:{unionType.ToDisplayString()}");

                    if (IsMessagePackObject(unionType))
                    {
                        GenerateTypeCall(referencedWithGenerator, sb, visited, unionType);
                    }
                }
            }
        }
        
        var keyAsPropertyName = UsesKeyAsPropertyName(type);

        var properties = type.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => 
                p is { DeclaredAccessibility: Accessibility.Public, IsImplicitlyDeclared: false, IsStatic: false } &&
                !HasIgnoreMember(p))
            .Select(p => new
            {
                Property = p,
                Key = GetMessagePackKey(p, keyAsPropertyName)
            })
            .OrderBy(p => p.Key, StringComparer.Ordinal);

        foreach (var prop in properties)
        {
            var key = prop.Key;
            var typeDisplay = prop.Property.Type.ToDisplayString();
            sb.AppendLine($"PROP:{key}:{typeDisplay}");

            if (prop.Property.Type is INamedTypeSymbol nested && IsMessagePackObject(nested))
            {
                GenerateTypeCall(referencedWithGenerator, sb, visited, nested);
            }
        }
    }

    private static void GenerateTypeCall(
        HashSet<string> referencedWithGenerator, 
        StringBuilder sb, 
        HashSet<INamedTypeSymbol> visited,
        INamedTypeSymbol namedTypeSymbol)
    {
        var typeName = namedTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            .Replace("global::", "")
            .Replace(".", "_");
                
        var assemblyName = namedTypeSymbol.ContainingAssembly.Identity.Name;

        if (referencedWithGenerator.Contains(assemblyName))
        {
            sb.AppendLine($"TYPE:{{{assemblyName}.GeneratedFingerprints.MessagePackFingerprints.{typeName}}}");
        }
        else
        {
            AppendTypeFingerprint(namedTypeSymbol, referencedWithGenerator, sb, visited);  
        }
    }

    private static bool HasIgnoreMember(IPropertySymbol property)
    {
        return property.GetAttributes()
            .Any(a => a.AttributeClass?.Name == "IgnoreMemberAttribute");
    }

    private static bool UsesKeyAsPropertyName(INamedTypeSymbol type)
    {
        var attr = type.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == "MessagePackObjectAttribute");

        if (attr != null && attr.ConstructorArguments.Length == 1)
        {
            var arg = attr.ConstructorArguments[0];
            if (arg.Type?.SpecialType == SpecialType.System_Boolean)
                return (bool)arg.Value!;
        }

        return false;
    }

    private static string GetMessagePackKey(IPropertySymbol property, bool keyAsPropertyName)
    {
        var keyAttr = property.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == "KeyAttribute");

        if (keyAttr != null && keyAttr.ConstructorArguments.Length > 0)
        {
            var arg = keyAttr.ConstructorArguments[0];
            return arg.Kind switch
            {
                //add more kinds
                TypedConstantKind.Primitive when arg.Type?.SpecialType == SpecialType.System_Int32 => arg.Value!.ToString()!,
                TypedConstantKind.Primitive when arg.Type?.SpecialType == SpecialType.System_String => arg.Value!.ToString()!,
                _ => "<unknown>"
            };
        }

        if (keyAsPropertyName)
            return property.Name;

        return "<unknown>";
    }

    private static bool IsMessagePackObject(INamedTypeSymbol type) =>
        type.GetAttributes().Any(attr => attr.AttributeClass?.Name == "MessagePackObjectAttribute");
}