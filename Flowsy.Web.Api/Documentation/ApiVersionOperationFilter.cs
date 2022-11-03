using Flowsy.Web.Api.Parameters;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Flowsy.Web.Api.Documentation;

/// <summary>
/// Adds the X-Version header and the apiVersion query parameter to Swagger requests.
/// </summary>
public class ApiVersionOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters.Add(new OpenApiParameter
        {
            In = ParameterLocation.Header,
            Name = ExtendedHeaderNames.Version,
            Required = false
        });
        
        operation.Parameters.Add(new OpenApiParameter
        {
            In = ParameterLocation.Query,
            Name = QueryStringParameterNames.ApiVersion,
            Required = false
        });
    }
}