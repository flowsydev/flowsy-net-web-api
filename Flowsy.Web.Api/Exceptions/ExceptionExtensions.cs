using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Flowsy.Web.Api.Exceptions;

/// <summary>
/// Extension methods for exceptions.
/// </summary>
public static class ExceptionExtensions
{
    /// <summary>
    /// Obtains a ProblemDetails instance from the given exception.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <param name="statusCode">The status code.</param>
    /// <param name="type">A URI reference [RFC3986] that identifies the problem type</param>
    /// <param name="detail">The exception detail.</param>
    /// <param name="extensions">Additional properties containing exception details.</param>
    /// <returns>An instance of ProblemDetails</returns>
    public static ProblemDetails Map(
        this Exception exception,
        int statusCode,
        string? type = null,
        string? detail = null,
        IDictionary<string, object?>? extensions = null
        )
    {
        var problemDetails = new ProblemDetails
        {
            Type = type ?? $@"https://httpstatuses.com/{statusCode}",
            Status = statusCode,
            Title = exception.Message
        };

        if (detail is not null)
            problemDetails.Detail = detail;

        if (extensions is null)
            return problemDetails;
        
        foreach (var (key, value) in extensions)
            problemDetails.Extensions.Add(key, value);

        return problemDetails;
    }

    /// <summary>
    /// Obtains a ProblemDetails instance from the given exception.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <param name="errors">The validation errors.</param>
    /// <param name="type">A URI reference [RFC3986] that identifies the problem type</param>
    /// <param name="detail">The exception detail.</param>
    /// <param name="extensions">Additional properties containing exception details.</param>
    /// <returns>An instance of ValidationProblemDetails</returns>
    public static ValidationProblemDetails Map(
        this Exception exception,
        IDictionary<string, string[]> errors,
        string? type = null,
        string? detail = null,
        IDictionary<string, object?>? extensions = null
        )
    {
        const int statusCode = StatusCodes.Status400BadRequest;
        
        var problemDetails = new ValidationProblemDetails(errors)
        {
            Type = type ?? $@"https://httpstatuses.com/{statusCode}",
            Status = statusCode,
            Title = exception.Message
        };

        if (detail is not null)
            problemDetails.Detail = detail;

        if (extensions is null)
            return problemDetails;
        
        foreach (var (key, value) in extensions)
            problemDetails.Extensions.Add(key, value);

        return problemDetails;
    }
}