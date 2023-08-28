namespace Flowsy.Web.Api.Security.Clients;

public interface IApiClientManager
{
    Task<ApiClient?> GetClientAsync(string clientId, CancellationToken cancellationToken);
    Task<IEnumerable<ApiClient>> GetClientsAsync(CancellationToken cancellationToken);
    
    Task<ApiClient> ValidateAsync(string clientId, string apiKey, CancellationToken cancellationToken);
    
    Task<bool> IsValidAsync(string clientId, string apiKey, CancellationToken cancellationToken);
}