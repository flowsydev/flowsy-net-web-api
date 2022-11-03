using System.Reflection;
using System.Runtime.Serialization;
using Flowsy.Core;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Flowsy.Web.Api.Documentation;

/// <summary>
/// Removes from Swagger schemas the properties marked with the [IgnoreDataMember] attribute.
/// </summary>
public class IgnoredDataMemberSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Properties == null)
            return;

        foreach (var property in context.Type.GetProperties())
        {
            var schemaPropertyName = property.Name.ApplyNamingConvention(NamingConvention.CamelCase);
            if (
                property.GetCustomAttribute<IgnoreDataMemberAttribute>() != null &&
                schema.Properties.ContainsKey(schemaPropertyName)
                )
            {
                schema.Properties.Remove(schemaPropertyName);
            }
        }
    }
}