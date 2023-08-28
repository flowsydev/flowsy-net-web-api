using System.Security.Authentication;
using Flowsy.Localization;

namespace Flowsy.Web.Api.Security.Clients;

public class InMemoryApiClientManager : IApiClientManager
{
    private readonly IDictionary<string, ApiClient> _clients;

    public InMemoryApiClientManager(IEnumerable<ApiClient> clients)
    {
        _clients = clients.ToDictionary(apiKey => apiKey.ClientId);
    }

    public Task<ApiClient?> GetClientAsync(string clientId, CancellationToken cancellationToken)
        => Task.Run(
            () =>
            {
                if (_clients.All(e => string.Compare(e.Key, clientId, StringComparison.OrdinalIgnoreCase) != 0))
                    return null;
                
                var entry = _clients.FirstOrDefault(e =>
                    string.Compare(e.Key, clientId, StringComparison.OrdinalIgnoreCase) == 0);

                return entry.Value;
            },
            cancellationToken
            );

    public Task<IEnumerable<ApiClient>> GetClientsAsync(CancellationToken cancellationToken)
        => Task.Run<IEnumerable<ApiClient>>(() => _clients.Values, cancellationToken);
    
    public Task<ApiClient> ValidateAsync(string clientId, string apiKey, CancellationToken cancellationToken)
        => Task.Run(() =>
            {
                var client =
                (
                    from entry in _clients
                    where string.Compare(entry.Key, clientId, StringComparison.OrdinalIgnoreCase) == 0 &&
                          entry.Value.ApiKey == apiKey
                    select entry.Value
                ).FirstOrDefault();

                if (client is null)
                    throw new AuthenticationException("InvalidClientIdOrApiKey".Localize());

                return client;
            },
            cancellationToken
            );

    public Task<bool> IsValidAsync(string clientId, string apiKey, CancellationToken cancellationToken)
        => Task.Run(
            () => 
                _clients.Any(
                    entry =>
                        string.Compare(entry.Key, clientId, StringComparison.OrdinalIgnoreCase) == 0 &&
                        entry.Value.ApiKey == apiKey
                ),
            cancellationToken
            );
}