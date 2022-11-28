using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NetDaemon.HassModel.CodeGenerator.CodeGeneration;

internal static class HelpersGenerator
{
    public static IEnumerable<MemberDeclarationSyntax> Generate(IEnumerable<EntityDomainMetadata> domains, IEnumerable<HassServiceDomain> orderedServiceDomains)
    {
        var injectableTypes = GetInjectableTypes(domains, orderedServiceDomains);
        return new[] { GenerateServiceCollectionExtension(injectableTypes) };
    }

    private static IEnumerable<string> GetInjectableTypes(IEnumerable<EntityDomainMetadata> domains, IEnumerable<HassServiceDomain> orderedServiceDomains) =>
        domains.Select(d => d.EntitiesForDomainClassName)
            .Prepend(EntitiesClassName)
            .Append(ServicesClassName)
            .Union(orderedServiceDomains.Select(d => GetServicesTypeName(d.Domain)));

    /// <summary>
    /// Generates the ServiceCollectionExtensions class
    /// </summary>
    /// <param name="typeNames"></param>
    /// <example>
    /// public static class ServiceCollectionExtensions
    /// {
    ///     public static IServiceCollection AddGeneratedCode(this IServiceCollection serviceCollection)
    ///     {
    ///         serviceCollection.AddTransient<Entities>();
    ///         serviceCollection.AddTransient<AutomationEntities>();
    ///         serviceCollection.AddTransient<BinarySensorEntities>();
    ///         serviceCollection.AddTransient<Services>();
    ///         serviceCollection.AddTransient<AlarmControlPanelServices>();
    ///         return serviceCollection;
    ///    }
    /// }
    /// </example>
    private static MemberDeclarationSyntax GenerateServiceCollectionExtension(IEnumerable<string> typeNames)
    {
        return
            ClassDeclaration("ServiceCollectionExtensions")
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                .WithMembers(SingletonList<MemberDeclarationSyntax>(
                    MethodDeclaration(IdentifierName("IServiceCollection"), Identifier("AddHomeAssistantGenerated"))
                        .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                        .WithParameterList(ParameterList(SingletonSeparatedList(Parameter(Identifier("serviceCollection"))
                            .WithModifiers(TokenList(Token(SyntaxKind.ThisKeyword))).WithType(IdentifierName("IServiceCollection")))))
                        .WithBody(Block(
                            typeNames.Select<string, StatementSyntax>(name =>
                                    ExpressionStatement(InvocationExpression(
                                        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("serviceCollection"),
                                            GenericName(Identifier("AddTransient"))
                                                .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName(name))))))))
                                .Append(
                                    ReturnStatement(IdentifierName("serviceCollection"))
                                )))
                        .WithSummaryComment("Registers all injectable generated types in the serviceCollection")
                ));
    }
}