using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Filters;

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
    public static WebApplicationBuilder AddDocumentation(this WebApplicationBuilder builder, params Assembly[] exampleAssemblies)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SchemaFilter<IgnoredDataMemberSchemaFilter>();
            
            options.OperationFilter<IgnoredDataMemberOperationFilter>();
            options.OperationFilter<AcceptLanguageOperationFilter>();
            options.OperationFilter<ApiVersionOperationFilter>();

            options.OperationFilter<ApiKeyOperationFilter>();

            options.ExampleFilters();
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
    {
        application.UseSwagger();
        application.UseSwaggerUI();
        return application;
    }
}