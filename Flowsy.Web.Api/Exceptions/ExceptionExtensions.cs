using Microsoft.AspNetCore.Mvc;

namespace Flowsy.Web.Api.Exceptions;

/// <summary>
/// Extension methods for exceptions.
/// </summary>
public static class ExceptionExtensions
{
    /// <summary>
    /// Obtains a ProblemDetails instance from an exception.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <param name="statusCode">The status code.</param>
    /// <param name="includeDatails">
    /// Whether or not to include exception details.
    /// The recommended value for the production environment is false.
    /// </param>
    /// <returns></returns>
    public static ProblemDetails Map(this Exception exception, int statusCode, bool includeDatails = false)
    {
        var problemDetails = new ProblemDetails
        {
            Type = $@"https://httpstatuses.com/{statusCode}",
            Status = statusCode,
            Title = exception.Message
        };

        if (includeDatails)
            problemDetails.Detail = exception.StackTrace;

        return problemDetails;
    }
}