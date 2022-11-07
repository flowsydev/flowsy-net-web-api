# Flowsy Web API

Foundation components for Web APIs.

## Features
This package gathers and extends the tools needed to create solid Web APIs by covering the following aspects:
* API Versioning
* API Key Security
* Routing Naming Convenion
* Data Validation
* Mediator Pattern for Controllers
* Form Content Management
* Data Streaming
* Logging
* Problem Details
* Swagger Documentation with Schema & Operation Filters

## Dependencies
Flowsy Web API relies on other Flowsy packages as well as other excellent libraries well known by the community.

* [Flowsy.Core](https://www.nuget.org/packages/Flowsy.Core)
* [Flowsy.Localization](https://www.nuget.org/packages/Flowsy.Localization)
* [Flowsy.Mediation](https://www.nuget.org/packages/Flowsy.Mediation)
* [FluentValidation.AspNetCore](https://www.nuget.org/packages/FluentValidation.AspNetCore)
* [Hellang.Middleware.ProblemDetails](https://www.nuget.org/packages/Hellang.Middleware.ProblemDetails)
* [Serilog.AspNetCore](https://www.nuget.org/packages/Serilog.AspNetCore)
* [Swashbuckle.AspNetCore](https://www.nuget.org/packages/Swashbuckle.AspNetCore)
* [Swashbuckle.AspNetCore.Filters](https://www.nuget.org/packages/Swashbuckle.AspNetCore.Filters)
* [Swashbuckle.AspNetCore.Swagger](https://www.nuget.org/packages/Swashbuckle.AspNetCore.Swagger)
* [Swashbuckle.AspNetCore.SwaggerGen](https://www.nuget.org/packages/Swashbuckle.AspNetCore.SwaggerGen)
* [Swashbuckle.AspNetCore.SwaggerUI](https://www.nuget.org/packages/Swashbuckle.AspNetCore.SwaggerUI)

## Startup
Add the following code to the Program.cs file and customize as needed:

```csharp
using System.Globalization;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using Flowsy.Mediation;
using Flowsy.Web.Api.Documentation;
using Flowsy.Web.Api.Exceptions;
using Flowsy.Web.Api.Routing;
using Flowsy.Web.Api.Security;
using Flowsy.Web.Api.Versioning;
// using Flowsy.Web.Localization; // Add a reference to Flowsy.Web.Localization to add localization support
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

//////////////////////////////////////////////////////////////////////
// Build the application
//////////////////////////////////////////////////////////////////////

var builder = WebApplication.CreateBuilder(args);
var myApiAssembly = Assembly.GetExecutingAssembly();

// Load default culture from configuration
CultureInfo.CurrentUICulture = new CultureInfo("en-US");

// Add logging
builder.Host.UseSerilog((_, loggerConfiguration) =>
    {
        loggerConfiguration
            .ReadFrom
            .Configuration(builder.Configuration, "Some:Configuration:Section")
            .Destructure.ByTransforming<ClaimsPrincipal>(p => p.Identity?.Name ?? p.ToString() ?? "Unknown user")
            .Destructure.ByTransforming<ApiClient>(c => c.ClientId)
            .Destructure.ByTransforming<CultureInfo>(c => c.Name);
            // Add other transformations as needed
    
        // Customize loggerConfiguration
    });

// Add API Versioning
builder.Services.AddApiVersioning("1.0");
    
// Add a reference to Flowsy.Web.Localization to add localization support
// builder.AddLocalization(options => {
//     // Load supported culture names from configuration
//     options.SupportedCultureNames = new [] { "en-US", "es-MX" };
// });

// Add CORS policy
builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            // Load settings from configuration
            policy.WithOrigins("https://www.example1.com", "https://www.example2.com");
            policy.WithMethods("OPTIONS", "GET", "POST", "PUT", "PATCH", "DELETE");
            policy.WithHeaders("Accept-Content", "Content-Type");
        });
    });

// Add controllers and customize as needed
builder.Services
    .AddControllers(options =>
    {
        options.Conventions.Add(new RouteTokenTransformerConvention(new KebabCaseRouteParameterTransformer()));
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// Configure form options
builder.Services.Configure<FormOptions>(options =>
    {
        // Customize options
    });

// Add API client management
builder.Services.AddSingleton<IApiClientManager>(serviceProvider => {
    var clients = new List<ApiClient>();
    // Load clients from some data store or provide a custom IApiClientManager implementation
    return new InMemoryApiClientManager(clients);
});

// Add FluentValidation and customize as needed
ValidatorOptions.Global.LanguageManager.Enabled = languageManagerEnabled;
ValidatorOptions.Global.PropertyNameResolver = (_, member, _) => member?.Name.ApplyNamingConvention(propertyNamingConvention);
builder.Services.AddValidatorsFromAssembly(myApiAssembly);

// Add mediation
builder.Services.AddMediation(
    true, // Register InitializationBehavior to set the current user and culture for every request
    true, // Register LoggingBehavior to log information for every request and its result
    myApiAssembly // Register queries and commands from this assembly
    // Register queries and commands from others assemblies
    );

// Add other services

// Add Problem Details and customize as needed
builder.Services.AddProblemDetails(options =>
    {
        var isProduction = builder.Environment.IsProduction();

        options.IncludeExceptionDetails = (context, exception) => !isProduction;

        options.ShouldLogUnhandledException = (context, exception, problemDetails) => true;
        
        // Map different exception classes to specific HTTP status codes
        options.Map<SomeException>(exception => exception.Map(StatusCodes.Status500InternalServerError, !isProduction));
    });

// If not in production environment, add Swagger documentation with request examples from the executing assembly
if (!builder.Environment.IsProduction())
    builder.AddDocumentation(myApiAssembly);


//////////////////////////////////////////////////////////////////////
// Configure the HTTP request pipeline and run the application
//////////////////////////////////////////////////////////////////////

var app = builder.Build();

app.UseProblemDetails();

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app
    .UseApiVersioning()
    .UseSerilogRequestLogging(options =>
    {
        // Customize options if needed
    });
    
app.UseCors();

// Add a reference to Flowsy.Web.Localization to add localization support
// app.UseLocalization();

app.UseAuthentication();
app.UseRouting();
app.MapControllers();

// Add custom middleware
// app
    // .UseMiddleware<SomeMiddleware>()
    // .Use((context, next) =>
    // {
    //     // Perform some task
    //     return next(context);
    // });

app.UseAuthorization();   

app.Run();
```

## Controllers
This package provides the MediationController class to offer built-in validation and mediation functionallity based on
[FluentValidation.AspNetCore](https://www.nuget.org/packages/FluentValidation.AspNetCore) and [Flowsy.Mediation](https://www.nuget.org/packages/Flowsy.Mediation).

Our goal is to put the application logic out of controllers, even in separate assemblies.

```csharp
using System.Threading;
using Flowsy.Web.Api.Mediation;
using Flowsy.Web.Api.Security;
using Microsoft.AspNetCore.Mvc;
using My.Application.Commands;

[ApiController]
[Route("/api/[controller]")]
[AuthorizeApiClient("ClientId1", "ClientId2")] // Only for specific API clients with a valid API Key 
// [AuthorizeApiClient] // For any API client with a valid API Key 
public class CustomerController : MediationController
{
    // With manual validation and using the IMediator instance directly
    // The mediation result is and instance of the expected CreateCustomerCommandResult class 
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateCustomerCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return ValidationProblem(validationResult);
            
        var commandResult = await Mediator.Send(command, cancellationToken);
        return Ok(commandResult);
    }
    
    // With automatic validation if an instance of IValidator<UpdateCustomerCommand> is registered
    // The mediation result is and instance of the expected UpdateCustomerCommandResult class 
    [HttpPut("{customerId:int}")]
    public async Task<IActionResult> UpdateAsync(int customerId, [FromBody] UpdateCustomerCommand command, CancellationToken cancellationToken)
    {
        command.CustomerId = customerId; // Ensure the command is using the right customer ID
        var commandResult = await MediateAsync<UpdateCustomerCommand, UpdateCustomerCommandResult>(command, cancellationToken);
        return Ok(commandResult);
    }
    
    // With automatic validation if an instance of IValidator<DeleteCustomerCommand>
    // The mediation result is an instance of IActionResult 
    [HttpDelete("{customerId:int}")]
    public Task<IActionResult> DeleteAsync(int customerId, [FromBody] DeleteCustomerCommand command, CancellationToken cancellationToken)
    {
        command.CustomerId = customerId; // Ensure the command is using the right customer ID
        return MediateActionResultAsync<DeleteCustomerCommand, DeleteCustomerCommandResult>(command, cancellationToken);
    }
}
```

## Data Streaming
### 1. Configure File Buffering Options and Register a Streaming Provider
```csharp
// Program.cs
// using ...
using Flowsy.Web.Api.Streaming;
// using ...

var builder = WebApplication.CreateBuilder(args);
// Add services
builder.Services.Configure<FileBufferingOptions>(options =>
    {
        // Configure options:
        // MemoryThreshold
        // BufferLimit
        // TempFileDirectory
        // TempFileDirectoryAccessor
        // BytePool
    });
builder.Services.AddSingleton<IStreamingProvider, StreamingProvider>()

var app = builder.Build();
// Use services
app.Run();
```
### 2. Read or Write Streams Using a Streaming Provider
```csharp
// FileUploader.cs
// using ...
using Flowsy.Web.Api.Streaming;
// using ...

public class FileUploader
{
    private readonly IStreamingProvider _streamingProvider;
    
    public FileUploader(IStreamingProvider streamingProvider)
    {
        _streamingProvider = streamingProvider;
    }
    
    public void UploadLargeFile(Stream inputStream)
    {
        using var bufferingStream = _streamingProvider.CreateFileBufferingReadStream(inputStream);
        // Read content using bufferingStream 
        // Make decisions based on the content
        bufferingStream.Seek(0, SeekOrigin.Begin); // Rewind
        // Read content again and store it somewhere
    }
}
```

## Forms
The following example shows how to read data from a multipart request.
If an instance of IStreamingProvider is registered, the MultipartHandler service will use it to buffer content while reading request body sections.
```csharp
using System.Threading;
using Flowsy.Web.Api.Forms;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/api/[controller]")]
public class ExampleController : ControllerBase // Or MediationController
{
    private readonly IMultipartHandler _multipartHandler;
    
    public ExampleController(IMultipartHandler multipartHandler)
    {
        _multipartHandler = multipartHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Post(CancellationToken cancellationToken)
    {
        // The MultipartContent instance will be disposed after return
        await using var multpartContent = await _multipartHandler.GetContentAsync(Request, cancellationToken);
        
        // Loop through request fields
        foreach (var (key, values) in multpartContent.Data)
        {
            // Process each field
            foreach (var value in values)
            {
                // Process each field value
            }
        }

        // Loop through request files
        foreach (var (key, multipartFile) in multpartContent.Files)
        {
            // Process each multipart file
        }
        
        // Deserialize fields expected to be in JSON format
        var myObject = multpartContent.DeserializeJsonField<MyClass>("fieldName");
        
        return Ok(/* Some result */);
    }
}
```

## Security Extensions
```csharp
using System.Threading;
using Flowsy.Web.Api.Forms;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/api/[controller]")]
public class ExampleController : ControllerBase // Or MediationController
{
    [HttpPost]
    public IActionResult Post()
    {
        // Obtain the client identifier and API Key from a header named X-{ClientId}-ApiKey
        HttpContext.GetApiKey(out var clientId, out var apiKey);
        // Do something with clientId and apiKey
        
        // Obtain value from a header named Athorization without the 'Bearer ' prefix
        var authorization = HttpContext.GetHeaderValue("Authorization", "Bearer ");
        // Do something with authorization
        
        return Ok(/* Some result */);
    }
}
```