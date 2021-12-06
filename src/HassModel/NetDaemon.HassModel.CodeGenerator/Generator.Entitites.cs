﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using JoySoftware.HomeAssistant.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NetDaemon.HassModel.CodeGenerator.Helpers;
using NetDaemon.HassModel.CodeGenerator.Extensions;
using NetDaemon.HassModel.Common;
using NetDaemon.HassModel.Entities;
using static NetDaemon.HassModel.CodeGenerator.Helpers.NamingHelper;
using static NetDaemon.HassModel.CodeGenerator.Helpers.SyntaxFactoryHelper;

namespace NetDaemon.HassModel.CodeGenerator
{
    public partial class Generator
    {
        private static IEnumerable<TypeDeclarationSyntax> GenerateEntityTypes(IReadOnlyCollection<HassState> entities)
        {
            var entitySets = entities.GroupBy(e => (EntityIdHelper.GetDomain(e.EntityId), IsNumeric(e)))
                .Select(g => new EntitySet(g.Key.Item1, g.Key.Item2, g))
                .OrderBy(s => s.Domain)
                .ToList();
            
            var entityIds = entities.Select(x => x.EntityId).ToList();

            var entityDomains = GetDomainsFromEntities(entityIds).OrderBy(s => s).ToList();

            yield return GenerateRootEntitiesInterface(entityDomains);

            yield return GenerateRootEntitiesClass(entitySets);

            foreach (var entityClass in entitySets.GroupBy(s => s.EntitiesForDomainClassName).Select(g => GenerateEntiesForDomainClass(g.Key, g)))
            {
                yield return entityClass;
            }

            foreach (var entitytype in entitySets.Select(GenerateEntityType))
            {
                yield return entitytype;
            }

            foreach (var attributeRecord in entitySets.Select(GenerateAtributeRecord))
            {
                yield return attributeRecord;
            }
        }
        private static TypeDeclarationSyntax GenerateRootEntitiesInterface(IEnumerable<string> domains)
        {
            var autoProperties = domains.Select(domain =>
            {
                var typeName = GetEntitiesForDomainClassName(domain);
                var propertyName = domain.ToPascalCase();

                return (MemberDeclarationSyntax)Property(typeName, propertyName, init: false);
            }).ToArray();

            return Interface("IEntities").AddMembers(autoProperties).ToPublic();
        }

        // The Entites class that provides properties to all Domains
        private static TypeDeclarationSyntax GenerateRootEntitiesClass(IEnumerable<EntitySet> entitySet)
        {
            var haContextNames = GetNames<IHaContext>();

            var properties = entitySet.DistinctBy(s=>s.Domain).Select(set =>
            {
                var entitiesTypeName = GetEntitiesForDomainClassName(set.Domain);
                var entitiesPropertyName = set.Domain.ToPascalCase();

                return (MemberDeclarationSyntax)ParseProperty($"{entitiesTypeName} {entitiesPropertyName} => new(_{haContextNames.VariableName});")
                    .ToPublic();
            }).ToArray();

            return ClassWithInjected<IHaContext>("Entities").WithBase((string)"IEntities").AddMembers(properties).ToPublic();
        }

        /// <summary>
        /// Generates a record with all the attributes found in a set of entities providing unique names for each.
        /// </summary>
        private static RecordDeclarationSyntax GenerateAtributeRecord(EntitySet entitySet)
        {
            // Get all attributes of all entities in this set
            var jsonProperties = entitySet.EntityStates.SelectMany(s => s.AttributesJson?.EnumerateObject() ?? Enumerable.Empty<JsonProperty>());

            // Group the attributes by JsonPropertyName and find the best ClrType that fits all
            var attributesByJsonName = jsonProperties
                .GroupBy(p => p.Name)
                .Select(group => (CSharpName: group.Key.ToNormalizedPascalCase(),
                                  JsonName: group.Key, 
                                  Type: GetBestClrType(group)));

            // We might have different json names that after CamelCasing result in the same CSharpName 
            var uniqueProperties = attributesByJsonName
                .GroupBy(t => t.CSharpName)
                .SelectMany(DeduplictateCSharpName)
                .OrderBy(p => p.CSharpName);

            var propertyDeclarations = uniqueProperties.Select(a => Property($"{a.ClrType.GetFriendlyName()}?", a.CSharpName)
                                                                    .ToPublic()
                                                                    .WithAttribute<JsonPropertyNameAttribute>(a.JsonName));

            return Record(entitySet.AttributesClassName, propertyDeclarations).ToPublic();
        }

        private static IEnumerable<(string CSharpName, string JsonName, Type ClrType)> DeduplictateCSharpName(IEnumerable<(string CSharpName, string JsonName, Type ClrType)> items)
        {
            var list = items.ToList();
            if (list.Count == 1) return new[] { list.First() };

            return list.OrderBy(i => i.JsonName).Select((p, i) => ($"{p.CSharpName}_{i}", jsonName: p.JsonName, type: p.ClrType));
        }

        private static Type GetBestClrType(IEnumerable<JsonProperty> valueKinds)
        {
            var distinctCrlTypes = valueKinds
                .Select(p => p.Value.ValueKind)
                .Distinct()
                .Where(k => k!= JsonValueKind.Null) // null fits in any type so we can ignore it for now
                .Select(MapJsonType)
                .ToHashSet();

            // If all have the same clr type use that, if not it will be 'object'
            return distinctCrlTypes.Count == 1
                ? distinctCrlTypes.First()
                : typeof(object);
        }

        private static Type MapJsonType(JsonValueKind kind) =>
            kind switch
            {
                JsonValueKind.False => typeof(bool),
                JsonValueKind.Undefined => typeof(object),
                JsonValueKind.Object => typeof(object),
                JsonValueKind.Array => typeof(object),
                JsonValueKind.String => typeof(string),
                JsonValueKind.Number => typeof(double),
                JsonValueKind.True => typeof(bool),
                JsonValueKind.Null => typeof(object),
                _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
            };

        /// <summary>
        /// Generates the class with all the properties for the Entities of one domain
        /// </summary>
        private static TypeDeclarationSyntax GenerateEntiesForDomainClass(string className, IEnumerable<EntitySet> entitySets)
        {
            var entityClass = ClassWithInjected<IHaContext>(className).ToPublic();

            var entityProperty = entitySets.SelectMany(s=> s.EntityStates.Select(e => GenerateEntityProperty(e, s.EntityClassName))).ToArray();

            return entityClass.AddMembers(entityProperty);
        }

        private static MemberDeclarationSyntax GenerateEntityProperty(HassState entity, string className)
        {
            var entityName = EntityIdHelper.GetEntity(entity.EntityId);

            var propertyCode = $@"{className} {entityName.ToNormalizedPascalCase((string)"E_")} => new(_{GetNames<IHaContext>().VariableName}, ""{entity.EntityId}"");";

            return ParseProperty(propertyCode).ToPublic();
        }

        private static bool IsNumeric(HassState entity)
        {
            if (EntityIdHelper.GetDomain(entity.EntityId) == "input_number") return true; 
            return entity.Attributes?.ContainsKey("unit_of_measurement") ?? false;
        }

        /// <summary>
        /// Generates a record derived from Entity like ClimateEntity or SensorEntity for a specific set of entities
        /// </summary>
        private static TypeDeclarationSyntax GenerateEntityType(EntitySet entitySet)
        {
            string attributesGeneric = entitySet.AttributesClassName;

            var baseType = entitySet.IsNumeric ? typeof(NumericEntity) : typeof(Entity);
            var entityStateType = entitySet.IsNumeric ? typeof(NumericEntityState) : typeof(EntityState);

            var baseClass = $"{baseType.FullName}<{entitySet.EntityClassName}, {entityStateType.FullName}<{attributesGeneric}>, {attributesGeneric}>";

            var (className, variableName) = GetNames<IHaContext>();
            var classDeclaration = $@"record {entitySet.EntityClassName} : {baseClass}
                                    {{
                                            public {entitySet.EntityClassName}({className} {variableName}, string entityId) : base({variableName}, entityId)
                                            {{}}

                                            public {entitySet.EntityClassName}({typeof(Entity).FullName} entity) : base(entity)
                                            {{}}
                                    }}";

            return ParseRecord(classDeclaration).ToPublic();
        }

        /// <summary>
        ///     Returns a list of domains from all entities
        /// </summary>
        /// <param name="entities">A list of entities</param>
        private static IEnumerable<string> GetDomainsFromEntities(IEnumerable<string> entities) => entities.Select(EntityIdHelper.GetDomain).Distinct();
    }
}
