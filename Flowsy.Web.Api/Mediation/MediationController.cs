using Flowsy.Mediation;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Flowsy.Web.Api.Mediation;

/// <summary>
/// Provides validation and mediation functionallity for API controllers.
/// </summary>
public abstract class MediationController : ControllerBase
{
    /// <summary>
    /// An instance of IMediator to send requests or notifications.
    /// </summary>
    protected virtual IMediator Mediator => HttpContext.RequestServices.GetRequiredService<IMediator>();

    /// <summary>
    /// Gets a validator if one is registered.
    /// </summary>
    /// <typeparam name="T">The type of object to validate.</typeparam>
    /// <returns>The validator.</returns>
    protected virtual IValidator<T>? GetValidator<T>()
        => HttpContext.RequestServices.GetService<IValidator<T>>();
    
    /// <summary>
    /// Gets a validator.
    /// An exception is thrown if no validation is registered for an instance of T.
    /// </summary>
    /// <typeparam name="T">The type of object to validate.</typeparam>
    /// <returns>The validator.</returns>
    protected virtual IValidator<T> GetRequiredValidator<T>()
        => HttpContext.RequestServices.GetRequiredService<IValidator<T>>();
    
    /// <summary>
    /// Validates an object if an associated validator is registered.
    /// </summary>
    /// <param name="instance">The object to validate.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of object to validate.</typeparam>
    /// <returns>The validation result.</returns>
    protected virtual async Task<ValidationResult?> TryValidateAsync<T>(T instance, CancellationToken cancellationToken)
    {
        var validator = GetValidator<T>();
        
        if (validator is null)
            return null;
        
        return await validator.ValidateAsync(instance, cancellationToken);
    }
    
    /// <summary>
    /// Validates an object if an associated validator is registered and throws an exception if input is invalid.
    /// </summary>
    /// <param name="instance">The object to validate.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of object to validate.</typeparam>
    protected virtual async Task TryValidateAndThrowAsync<T>(T instance, CancellationToken cancellationToken)
    {
        var validator = GetValidator<T>();
        
        if (validator is null)
            return;
        
        await validator.ValidateAndThrowAsync(instance, cancellationToken);
    }

    /// <summary>
    /// Validates an object if an associated validator is registered.
    /// An exception is thrown if no validation is registered for an instance of T.
    /// </summary>
    /// <param name="instance">The object to validate.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of object to validate.</typeparam>
    /// <returns>The validation result.</returns>
    protected virtual Task<ValidationResult> ValidateAsync<T>(T instance, CancellationToken cancellationToken)
    {
        var validator = GetRequiredValidator<T>();
        return validator.ValidateAsync(instance, cancellationToken);
    }
    
    /// <summary>
    /// Validates an object if an associated validator is registered and throws an exception if input is invalid.
    /// An exception is thrown if no validation is registered for an instance of T.
    /// </summary>
    /// <param name="instance">The object to validate.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="T">The type of object to validate.</typeparam>
    protected virtual async Task ValidateAndThrowAsync<T>(T instance, CancellationToken cancellationToken)
    {
        var validator = GetRequiredValidator<T>();
        await validator.ValidateAndThrowAsync(instance, cancellationToken);
    }

    /// <summary>
    /// Sends a request to be processed with no result expected.
    /// </summary>
    /// <param name="request">The request to be processed.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="TRequest">The type of request.</typeparam>
    /// <returns>An instance of OkObjectResult with a 200 status code or BadRequestObjectResult with a 400 status code if a validation exception is thrown.</returns>
    protected virtual Task<IActionResult> MediateAsync<TRequest>(
        TRequest request,
        CancellationToken cancellationToken
        ) where TRequest : Request<Unit>
        => MediateAsync(request, StatusCodes.Status200OK, cancellationToken);

    /// <summary>
    /// Sends a request to be processed with no result expected.
    /// </summary>
    /// <param name="request">The request to be processed.</param>
    /// <param name="statusCode">The status code for the response.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="TRequest">The type of request.</typeparam>
    /// <returns>An instance of ObjectResult with the specified status code or BadRequestObjectResult with a 400 status code if a validation exception is thrown.</returns>
    protected virtual Task<IActionResult> MediateAsync<TRequest>(
        TRequest request,
        int statusCode,
        CancellationToken cancellationToken
        ) where TRequest : Request<Unit>
        => MediateAsync<TRequest, Unit>(request, statusCode, cancellationToken);

    /// <summary>
    /// Sends a request to be processed with some expected result.
    /// </summary>
    /// <param name="request">The request to be processed.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="TRequest">The type of request.</typeparam>
    /// <typeparam name="TResult">The type of result.</typeparam>
    /// <returns>An instance of OkObjectResult with a 200 status code or BadRequestObjectResult with a 400 status code if a validation exception is thrown.</returns>
    protected virtual Task<IActionResult> MediateAsync<TRequest, TResult>(
        TRequest request,
        CancellationToken cancellationToken
        ) where TRequest : Request<TResult>
        => MediateAsync<TRequest, TResult>(request, StatusCodes.Status200OK, cancellationToken);

    /// <summary>
    /// Sends a request to be processed with some expected result.
    /// </summary>
    /// <param name="request">The request to be processed.</param>
    /// <param name="statusCode">The status code for the response.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <typeparam name="TRequest">The type of request.</typeparam>
    /// <typeparam name="TResult">The type of result.</typeparam>
    /// <returns>An instance of ObjectResult with the specified status code or BadRequestObjectResult with a 400 status code if a validation exception is thrown.</returns>
    protected virtual async Task<IActionResult> MediateAsync<TRequest, TResult>(
        TRequest request,
        int statusCode,
        CancellationToken cancellationToken
        ) where TRequest : Request<TResult>
    {
        try
        {
            return StatusCode(statusCode, await Mediator.Send(request, cancellationToken));
        }
        catch (ValidationException exception)
        {
            return ValidationProblem(exception);
        }
    }

    /// <summary>
    /// Obtains a validation problem from a validation result.
    /// </summary>
    /// <param name="validationResult">The validation result.</param>
    /// <returns>The validation problem.</returns>
    protected virtual IActionResult ValidationProblem(ValidationResult validationResult)
    {
        validationResult.AddToModelState(ModelState, string.Empty);
        return ValidationProblem();
    }

    /// <summary>
    /// Obtains a validation problem from a validation exception.
    /// </summary>
    /// <param name="exception">The validation exception</param>
    /// <returns>The validation problem.</returns>
    protected virtual IActionResult ValidationProblem(ValidationException exception)
    {
        foreach (var error in exception.Errors)
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        
        return ValidationProblem();
    }
}