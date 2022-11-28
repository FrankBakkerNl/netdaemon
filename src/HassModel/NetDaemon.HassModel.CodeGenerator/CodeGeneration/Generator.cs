using Microsoft.CodeAnalysis.CSharp;
using NetDaemon.HassModel.CodeGenerator.CodeGeneration;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NetDaemon.HassModel.CodeGenerator;

internal static class Generator
{
    public static IEnumerable<MemberDeclarationSyntax> GenerateTypes(
        IReadOnlyCollection<EntityDomainMetadata> entityDomains,
        IReadOnlyCollection<HassServiceDomain> serviceDomains)
    {
        var orderedServiceDomains = serviceDomains.Where(d => d.Services?.Any() ?? false).OrderBy(x => x.Domain).ToArray();

        var helpers = HelpersGenerator.Generate(entityDomains, orderedServiceDomains);
        var entityClasses = EntitiesGenerator.Generate(entityDomains);
        var serviceClasses = ServicesGenerator.Generate(orderedServiceDomains);
        var extensionMethodClasses = ExtensionMethodsGenerator.Generate(orderedServiceDomains, entityDomains);

        return new[] {helpers, entityClasses, serviceClasses, extensionMethodClasses }.SelectMany(x => x).ToList();
    }

    public static CompilationUnitSyntax BuildCompilationUnit(string namespaceName, params MemberDeclarationSyntax[] generatedTypes)
    {
        return CompilationUnit()
            .AddUsings(UsingNamespaces.Select(u => UsingDirective(ParseName(u))).ToArray())
            .WithLeadingTrivia(TriviaHelper.GetFileHeader()
                .Append(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true))))
            .AddMembers(FileScopedNamespaceDeclaration(ParseName(namespaceName)))
            .AddMembers(generatedTypes)
            .NormalizeWhitespace();
    }
}