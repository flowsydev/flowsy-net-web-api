using Flowsy.Web.Api.Parameters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace Flowsy.Web.Api.Versioning;

/// <summary>
/// Dependency injection extensions for API versioning
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds API versioning with a combination of HeaderApiVersionReader, QueryStringApiVersionReader and MediaTypeApiVersionReader.
    /// </summary>
    /// <param name="services">The application service collection.</param>
    /// <param name="version">The API version.</param>
    /// <returns>The application service collection.</returns>
    public static IServiceCollection AddApiVersioning(this IServiceCollection services, string version)
    {
        services.AddApiVersioning(options =>
        {
            var versionComponents = version.Split(".");

            options.DefaultApiVersion = new ApiVersion(
                int.Parse(versionComponents[0]), 
                versionComponents.Length > 1 ? int.Parse(versionComponents[1]) : 0
            );
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new HeaderApiVersionReader(ExtendedHeaderNames.Version),
                new QueryStringApiVersionReader(QueryStringParameterNames.ApiVersion),
                new MediaTypeApiVersionReader(MediaTypeParameterNames.Version)
            );
        });

        return services;
    }
}