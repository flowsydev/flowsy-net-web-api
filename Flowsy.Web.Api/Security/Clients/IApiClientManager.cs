namespace Flowsy.Web.Api.Security.Clients;

public interface IApiClientManager
{
    IEnumerable<string> ClientIds { get; }
    
    ApiClient Validate(string clientId, string apiKey);
    Task<ApiClient> ValidateAsync(string clientId, string apiKey, CancellationToken cancellationToken);
    
    bool IsValid(string clientId, string apiKey);
    Task<bool> IsValidAsync(string clientId, string apiKey, CancellationToken cancellationToken);
}