using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Flowsy.Web.Api.Documentation;

/// <summary>
/// Removes from Swagger requests the properties marked with the [IgnoreDataMember] attribute.
/// </summary>
public class IgnoredDataMemberOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var ignoredProperties = context.MethodInfo.GetParameters()
            .SelectMany(p => 
                p.ParameterType
                    .GetProperties()
                    .Where(prop => prop.GetCustomAttribute<IgnoreDataMemberAttribute>() != null)
                )
            .ToList();

        if (!ignoredProperties.Any()) return;

        foreach (var property in ignoredProperties)
        {
            operation.Parameters = operation.Parameters
                .Where(p => p.Name != property.Name && !p.Name.StartsWith($"{property.Name}."))
                .ToList();
        }
    }
}