using System.Security.Claims;
using Flowsy.Mediation;
using Microsoft.AspNetCore.Http;

namespace Flowsy.Web.Api.Mediation;

public class HttpContextUserResolver : IRequestUserResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextUserResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<ClaimsPrincipal?> GetUserAsync()
        => Task.FromResult(_httpContextAccessor.HttpContext?.User);
}