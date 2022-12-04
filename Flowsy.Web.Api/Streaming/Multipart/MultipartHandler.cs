using System.Text;
using Flowsy.Content;
using Flowsy.Localization;
using Flowsy.Web.Api.Streaming.Buffering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace Flowsy.Web.Api.Streaming.Multipart;

/// <summary>
/// Handles multipart requests.
/// </summary>
public class MultipartHandler : IMultipartHandler
{
    private readonly IBufferingProvider? _bufferingProvider;
    private readonly IContentInspector? _contentInspector;
    private readonly IEnumerable<string> _allowedMimeTypes;

    public MultipartHandler(
        IBufferingProvider? bufferingProvider,
        IContentInspector? contentInspector,
        IEnumerable<string>? allowedMimeTypes
        )
    {
        _bufferingProvider = bufferingProvider;
        _contentInspector = contentInspector;
        _allowedMimeTypes = allowedMimeTypes ?? Array.Empty<string>();
    }

    /// <summary>
    /// Reads the content of a multipart request using UTF-8 encoding.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>An instance of MultipartContent with fields and files from the request.</returns>
    public Task<MultipartContent> GetContentAsync(
        HttpRequest request,
        CancellationToken cancellationToken
        )
        => GetContentAsync(request, Encoding.UTF8, cancellationToken);

    /// <summary>
    /// Reads the content of a multipart request using the specified encoding.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <param name="encoding">The character encoding.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>An instance of MultipartContent with fields and files from the request.</returns>
    public async Task<MultipartContent> GetContentAsync(HttpRequest request, Encoding encoding, CancellationToken cancellationToken)
    {
        var files = new Dictionary<string, MultipartFile>();
        
        var boundary = HeaderUtilities.RemoveQuotes(
            MediaTypeHeaderValue.Parse(request.ContentType).Boundary
        ).Value;

        var reader = new MultipartReader(boundary, request.Body);
        var accumulator = new KeyValueAccumulator();
        var invalidFiles = new List<string>();

        while (await reader.ReadNextSectionAsync(cancellationToken) is { } section)
        {
            var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(
                section.ContentDisposition, out var contentDisposition
            );
            if (!hasContentDispositionHeader || contentDisposition is null)
                continue;

            if (
                contentDisposition.DispositionType.Equals("form-data") &&
                !string.IsNullOrEmpty(contentDisposition.Name.Value) &&
                !string.IsNullOrEmpty(contentDisposition.FileName.Value)
            )
            {
                Stream? stream = null;
                try
                {
                    if (_bufferingProvider is null)
                    {
                        stream = new MemoryStream();
                        await section.Body.CopyToAsync(stream, cancellationToken);
                        stream.Seek(0, SeekOrigin.Begin);
                    }
                    else
                        stream = _bufferingProvider.CreateFileBufferingReadStream(section.Body);

                    ContentDescriptor? contentDescriptor = null;
                    if (_contentInspector is not null)
                        contentDescriptor = await _contentInspector.InspectAsync(
                            stream, 
                            Path.GetExtension(contentDisposition.FileName.Value), 
                            cancellationToken
                            );
                    
                    if (contentDescriptor is not null)
                    {
                        contentDescriptor.Name = contentDisposition.FileName.Value;
                        contentDescriptor.CreationDate = contentDisposition.CreationDate?.DateTime;
                        contentDescriptor.ModificationDate = contentDisposition.ModificationDate?.DateTime;
                        contentDescriptor.ReadDate = contentDisposition.ReadDate?.DateTime;

                        if (_allowedMimeTypes.Any())
                        {
                            var intersection = contentDescriptor.MimeTypes.Intersect(_allowedMimeTypes);
                            if (!intersection.Any())
                            {
                                invalidFiles.Add(contentDisposition.FileName.Value);
                                stream.Dispose();
                                continue;
                            }
                        }
                    }

                    files.Add(contentDisposition.Name.Value, new MultipartFile(
                        contentDisposition.Name.Value,
                        stream,
                        contentDescriptor
                    ));
                }
                catch
                {
                    stream?.Dispose();
                    throw;
                }
            }
            else if (!invalidFiles.Any())
            {
                var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name).Value;

                using var streamReader = new StreamReader(
                    section.Body,
                    encoding: encoding,
                    detectEncodingFromByteOrderMarks: true,
                    bufferSize: 1024,
                    leaveOpen: true
                );
                        
                var value = await streamReader.ReadToEndAsync();
                if (string.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
                    value = string.Empty;
                        
                accumulator.Append(key, value);
            }
        }

        if (!invalidFiles.Any())
            return new MultipartContent(accumulator.GetResults(), files);
        
        var message = "InvalidFileType".Localize();
        var invalidFilesJoined = string.Join(" | ", invalidFiles.Select(f => $"'{f}'"));
        throw new InvalidDataException($"{message}: {invalidFilesJoined}");
    }
}