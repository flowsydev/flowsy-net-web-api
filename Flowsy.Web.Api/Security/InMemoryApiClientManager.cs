using System.Security.Authentication;
using Flowsy.Localization;

namespace Flowsy.Web.Api.Security;

public class InMemoryApiClientManager
{
    private readonly IDictionary<string, ApiClient> _clients;

    public InMemoryApiClientManager(IEnumerable<ApiClient> clients)
    {
        _clients = clients.ToDictionary(apiKey => apiKey.ClientId);
    }

    public IEnumerable<string> ClientIds => _clients.Keys;

    public ApiClient Validate(string clientId, string apiKey)
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
    }

    public Task<ApiClient> ValidateAsync(string clientId, string apiKey, CancellationToken cancellationToken)
        => Task.Run(() => Validate(clientId, apiKey), cancellationToken);

    public bool IsValid(string clientId)
        => _clients.Any(entry => string.Compare(entry.Key, clientId, StringComparison.OrdinalIgnoreCase) == 0);

    public Task<bool> IsValidAsync(string clientId, string apiKey, CancellationToken cancellationToken)
        => Task.Run(() => IsValid(clientId, apiKey), cancellationToken);

    public bool IsValid(string clientId, string apiKey)
        => _clients.Any(
            entry =>
                string.Compare(entry.Key, clientId, StringComparison.OrdinalIgnoreCase) == 0 &&
                entry.Value.ApiKey == apiKey
        );
}