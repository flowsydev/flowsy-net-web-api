using System.Security.Authentication;
using Flowsy.Localization;

namespace Flowsy.Web.Api.Security.Clients;

public abstract class PersistentApiClientManager : IApiClientManager
{
    protected PersistentApiClientManager() : this(TimeSpan.Zero)
    {
    }

    protected PersistentApiClientManager(TimeSpan cacheLifetime)
    {
        Cache = new ApiClientCache(cacheLifetime);
    }

    protected PersistentApiClientManager(IEnumerable<ApiClient> clients, TimeSpan cacheLifetime) : this(cacheLifetime)
    {
        Cache.Save(clients.ToArray());
    }

    public virtual Task<ApiClient?> GetClientAsync(string clientId, CancellationToken cancellationToken)
        => Task.Run(() => Cache.GetOne(clientId), cancellationToken);

    public virtual Task<IEnumerable<ApiClient>> GetClientsAsync(CancellationToken cancellationToken)
        => Task.Run(() => Cache.GetAll(), cancellationToken);
    
    public virtual async Task<ApiClient> ValidateAsync(string clientId, string apiKey, CancellationToken cancellationToken)
    {
        var client = await GetClientAsync(clientId, cancellationToken);
        
        if (client is null || client.ApiKey != apiKey)
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