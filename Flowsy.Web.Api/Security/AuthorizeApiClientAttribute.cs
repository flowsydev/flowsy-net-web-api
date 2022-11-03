using System.Security.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Flowsy.Web.Api.Security;

/// <summary>
/// Protects a controller or a specific controller method so it can be invoked only by clients with a valid API Key.
/// The API Key is expected to be present in a request header named X-{ClientId}-ApiKey.
/// This filter requires an implementation of IApiClientManager to be registered in the application services.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeApiClientAttribute : ActionFilterAttribute
{
    private readonly IEnumerable<string> _clients;
    private readonly ILogger _logger;

    /// <summary>
    /// Creates a new instance of AuthorizeApiClientAttribute.
    /// </summary>
    /// <param name="clients">
    /// The list of client identifiers allowed to invoke the controller methods.
    /// If none is specified, any client with a valid API Key can invoke the controller methods. 
    /// </param>
    public AuthorizeApiClientAttribute(params string[] clients)
    {
        _clients = clients;
        _logger = Log.ForContext<AuthorizeApiClientAttribute>();
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        try
        {
            var httpContext = context.HttpContext;
            httpContext.GetApiKey(out var clientId, out var apiKey);
            
            if (clientId is null || apiKey is null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
        
            var clientManager = httpContext.RequestServices.GetRequiredService<IApiClientManager>();
            
            if (_clients.Any() && _clients.All(id => string.Compare(id, clientId, StringComparison.OrdinalIgnoreCase) != 0))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            try
            {
                ApiClient.Current = await clientManager.ValidateAsync(clientId, apiKey, CancellationToken.None);
                await next();
            }
            catch (AuthenticationException)
            {
                _logger.Error("Invalid API Key for client {ClientId}", clientId);
                context.Result = new UnauthorizedResult();
            }
        }
        catch (Exception exception)
        {
            _logger.Error(exception, "Could not retrieve API Key from HTTP request");
            context.Result = new UnauthorizedResult();
        }
    }
}