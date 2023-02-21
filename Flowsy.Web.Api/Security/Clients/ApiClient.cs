using System.Security.Claims;

namespace Flowsy.Web.Api.Security.Clients;

/// <summary>
/// Represents an external application or service that consumes the API. 
/// </summary>
public class ApiClient
{
    public static ApiClient Anonymous { get; } = new ();
    public static ApiClient Current { get; set; } = Anonymous;
    
    private ApiClient() : this(string.Empty, string.Empty, Array.Empty<Claim>())
    {
    }

    public ApiClient(string clientId, string apiKey, IEnumerable<Claim> claims)
    {
        ClientId = clientId;
        ApiKey = apiKey;
        Claims = claims;
    }

    /// <summary>
    /// The unique identifier for the external application or service.
    /// </summary>
    public string ClientId { get; }
    
    /// <summary>
    /// The API Key for the external application or service.
    /// </summary>
    public string ApiKey { get; }
    
    /// <summary>
    /// Custom claims for the external application or service.
    /// </summary>
    public IEnumerable<Claim> Claims { get; }
}