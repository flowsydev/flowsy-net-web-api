using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Flowsy.Web.Api.Documentation;

/// <summary>
/// Dependency injection extensions.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Swagger to the application builder. 
    /// </summary>
    /// <param name="builder">The application builder</param>
    /// <param name="exampleAssemblies">The list of assemblies containing request examples.</param>
    /// <returns>The application builder.</returns>
    public static WebApplicationBuilder AddDocumentation(
        this WebApplicationBuilder builder,
        params Assembly[] exampleAssemblies
        )
        => builder.AddDocumentation(null, true, exampleAssemblies);

    /// <summary>
    /// Adds Swagger to the application builder. 
    /// </summary>
    /// <param name="builder">The application builder</param>
    /// <param name="setupAction">An action to configure a SwaggerGenOptions instance.</param>
    /// <param name="includeApiKeyOperationFilter">Indicates whether or not to add an operation filter of type ApiKeyOperationFilter.</param>
    /// <param name="exampleAssemblies">The list of assemblies containing request examples.</param>
    /// <returns>The application builder.</returns>
    public static WebApplicationBuilder AddDocumentation(
        this WebApplicationBuilder builder,
        Action<SwaggerGenOptions>? setupAction = null,
        bool includeApiKeyOperationFilter = true,
        params Assembly[] exampleAssemblies
        )
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SchemaFilter<IgnoredDataMemberSchemaFilter>();
            
            options.OperationFilter<IgnoredDataMemberOperationFilter>();
            options.OperationFilter<AcceptLanguageOperationFilter>();
            options.OperationFilter<ApiVersionOperationFilter>();

            if (includeApiKeyOperationFilter)
                options.OperationFilter<ApiKeyOperationFilter>();

            options.ExampleFilters();
            
            setupAction?.Invoke(options);
        });
        builder.Services.AddSwaggerExamplesFromAssemblies(exampleAssemblies);

        return builder;
    }

    /// <summary>
    /// Configures Swagger in the application's request pipeline.
    /// </summary>
    /// <param name="application">The application.</param>
    /// <returns>The application.</returns>
    public static WebApplication UseDocumentation(this WebApplication application)
        => application.UseDocumentation(null);

    /// <summary>
    /// Configures Swagger in the application's request pipeline.
    /// </summary>
    /// <param name="application">The application.</param>
    /// <param name="setupAction">An action to configure a SwaggerUIOptions instance.</param>
    /// <returns>The application.</returns>
    public static WebApplication UseDocumentation(this WebApplication application, Action<SwaggerUIOptions>? setupAction)
    {
        application.UseSwagger();
        application.UseSwaggerUI(options => setupAction?.Invoke(options));
        return application;
    }
}