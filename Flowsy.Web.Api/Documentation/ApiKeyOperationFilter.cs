using Flowsy.Web.Api.Parameters;
using Flowsy.Web.Api.Security;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Flowsy.Web.Api.Documentation;

/// <summary>
/// Adds a X-{ClientId}-ApiKey header to Swagger requests for every known API client.
/// </summary>
public class ApiKeyOperationFilter : IOperationFilter
{
    private readonly IApiClientManager _apiClientManager;
    
    public ApiKeyOperationFilter(IApiClientManager apiClientManager)
    {
        _apiClientManager = apiClientManager;
    }
    
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        foreach (var clientId in _apiClientManager.ClientIds)
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = ExtendedHeaderNames.ApiKey.Replace("{ClientId}", clientId),
                In = ParameterLocation.Header,
                Required = false
            });   
        }
    }
}