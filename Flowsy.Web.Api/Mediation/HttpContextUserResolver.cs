using System.Security.Claims;
using Flowsy.Mediation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Flowsy.Web.Api.Mediation;

public class HttpContextUserResolver : IRequestUserResolver
{
    private readonly ClaimsPrincipal? _userMockup;
    
    public HttpContextUserResolver(IHttpContextAccessor httpContextAccessor, ClaimsPrincipal? userMockup)
    {
        HttpContextAccessor = httpContextAccessor;
        _userMockup = userMockup;
    }
    
    protected IHttpContextAccessor HttpContextAccessor { get; }
    
    public virtual Task<ClaimsPrincipal?> GetUserAsync<TRequest, TResult>(
        TRequest request,
        CancellationToken cancellationToken
        )
        where TRequest : Request<TResult>, IRequest<TResult>
        => Task.Run(
            () =>
            {
                var user = HttpContextAccessor.HttpContext?.User;
                return user is not null && user.Claims.Any() ? user : _userMockup;
            },
            cancellationToken
            );
}