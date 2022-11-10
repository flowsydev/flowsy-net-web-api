using System.Text;
using Microsoft.AspNetCore.Http;

namespace Flowsy.Web.Api.Streaming.Multipart;

/// <summary>
/// Handles multipart requests.
/// </summary>
public interface IMultipartHandler
{
    /// <summary>
    /// Reads the content of a multipart request using UTF-8 encoding.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>An instance of MultipartContent with fields and files from the request.</returns>
    public Task<MultipartContent> GetContentAsync(HttpRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Reads the content of a multipart request using the specified encoding.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <param name="encoding">The character encoding.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>An instance of MultipartContent with fields and files from the request.</returns>
    Task<MultipartContent> GetContentAsync(HttpRequest request, Encoding encoding, CancellationToken cancellationToken);
}