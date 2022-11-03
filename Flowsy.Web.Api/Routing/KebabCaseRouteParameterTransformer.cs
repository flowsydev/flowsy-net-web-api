using Flowsy.Core;
using Microsoft.AspNetCore.Routing;

namespace Flowsy.Web.Api.Routing;

/// <summary>
/// Applies lower kebab case convention to route tokens.
/// </summary>
public class KebabCaseRouteParameterTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        return value?.ToString()?.ApplyNamingConvention(NamingConvention.LowerKebabCase);
    }
}