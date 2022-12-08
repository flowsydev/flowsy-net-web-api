using Flowsy.Web.Api.Parameters;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Flowsy.Web.Api.Security;

/// <summary>
/// Security extensions methods.
/// </summary>
public static class SecurityExtensions
{
    /// <summary>
    /// Obtains the client identifier and its API Key from the request header named X-{ClientId}-ApiKey.
    /// The request must contain only one API Key header.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="apiKey">The API Key.</param>
    public static void GetApiKey(this HttpContext httpContext, out string? clientId, out string? apiKey)
    {
        try
        {
            KeyValuePair<string, StringValues>? header =
                httpContext.Request.Headers.SingleOrDefault(
                    h => ExtendedHeaderNames.ApiKeyRegex.IsMatch(h.Key)
                    );
            var headerName = header?.Key;
            var headerValues = header?.Value;

            if (headerName is null || headerValues is null || !headerValues.Value.Any())
            {
                clientId = null;
                apiKey = null;
                return;
            }

            clientId = ExtendedHeaderNames.ApiKeyRegex.Match(headerName).Groups[1].Value;
            apiKey = headerValues.Value.FirstOrDefault();
        }
        catch
        {
            clientId = null;
            apiKey = null;
        }
    }

    /// <summary>
    /// Obtains a single value for a HTTP header.
    /// The request must contain only one header with the specified name.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <param name="header">The header name.</param>
    /// <param name="valuePrefix">A prefix to be removed from the header value.</param>
    /// <returns>The header value.</returns>
    public static string? GetHeaderValue(this HttpContext httpContext, string header, string? valuePrefix = null)
    {
        try
        {
            KeyValuePair<string, StringValues>? headerKeyValuePair = 
                httpContext
                    .Request
                    .Headers
                    .Single(h => string.Compare(h.Key, header, StringComparison.OrdinalIgnoreCase) == 0);
        
            var headerValue = headerKeyValuePair?.Value.FirstOrDefault();
            if (headerKeyValuePair is null || string.IsNullOrEmpty(headerValue) || (valuePrefix is not null && !headerValue.StartsWith($"{valuePrefix} ")))
                return null;
        
            return valuePrefix is null
                ? headerValue 
                : headerValue.Split($"{valuePrefix} ")[1];
        }
        catch
        {
            return null;
        }
    }
}