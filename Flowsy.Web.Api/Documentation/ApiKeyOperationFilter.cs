using Flowsy.Web.Api.Parameters;
using Flowsy.Web.Api.Security;
using Flowsy.Web.Api.Security.Clients;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Flowsy.Web.Api.Documentation;

/// <summary>
/// Adds a X-{ClientId}-ApiKey header to Swagger requests for every known API client.
/// </summary>
public class ApiKeyOperationFilter : IOperationFilter
{
    private readonly IApiClientManager? _apiClientManager;
    
    public ApiKeyOperationFilter(IServiceProvider serviceProvider)
    {
        _apiClientManager = serviceProvider.GetService<IApiClientManager>();
    }
    
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (_apiClientManager is null)
            return;

        var task = _apiClientManager.GetClientsAsync(CancellationToken.None);
        task.Wait();
        
        foreach (var client in task.Result)
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = ExtendedHeaderNames.ApiKey.Replace("{ClientId}", client.ClientId),
                In = ParameterLocation.Header,
                Required = false
            });
        }
    }
}