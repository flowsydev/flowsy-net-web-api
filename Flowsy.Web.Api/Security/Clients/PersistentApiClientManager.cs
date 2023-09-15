using System.Security.Authentication;
using System.Text;
using Flowsy.Localization;

namespace Flowsy.Web.Api.Security.Clients;

public abstract class PersistentApiClientManager : IApiClientManager
{
    private readonly bool _encodedApiKey;
    
    protected PersistentApiClientManager() : this(TimeSpan.Zero, true)
    {
    }

    protected PersistentApiClientManager(TimeSpan cacheLifetime, bool encodedApiKey)
    {
        Cache = new ApiClientCache(cacheLifetime);
        _encodedApiKey = encodedApiKey;
    }

    protected PersistentApiClientManager(IEnumerable<ApiClient> clients, TimeSpan cacheLifetime, bool encodedApiKey) : this(cacheLifetime, encodedApiKey)
    {
        Cache.Save(clients.ToArray());
    }

    protected virtual string DecodeApiKey(string encodedApiKey)
    {
        var bytes = Convert.FromBase64String(encodedApiKey);
        return Encoding.UTF8.GetString(bytes);
    }

    public virtual Task<ApiClient?> GetClientAsync(string clientId, CancellationToken cancellationToken)
        => Task.Run(() => Cache.GetOne(clientId), cancellationToken);

    public virtual Task<IEnumerable<ApiClient>> GetClientsAsync(CancellationToken cancellationToken)
        => Task.Run(() => Cache.GetAll(), cancellationToken);
    
    public virtual async Task<ApiClient> ValidateAsync(string clientId, string apiKey, CancellationToken cancellationToken)
    {
        var client = await GetClientAsync(clientId, cancellationToken);
        var decodedApiKey = _encodedApiKey ? DecodeApiKey(apiKey) : apiKey;
        
        if (client is null || client.ApiKey != decodedApiKey)
            throw new AuthenticationException("InvalidClientIdOrApiKey".Localize());

        return client;
    }

    public virtual async Task<bool> IsValidAsync(string clientId, string apiKey, CancellationToken cancellationToken)
    {
        try
        {
            await ValidateAsync(clientId, apiKey, cancellationToken);
            return true;
        }
        catch (AuthenticationException)
        {
            return false;
        }
    }
    
    protected ApiClientCache Cache { get; }
}