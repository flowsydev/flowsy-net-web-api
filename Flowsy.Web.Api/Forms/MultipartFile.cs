using Flowsy.Content;

namespace Flowsy.Web.Api.Forms;

/// <summary>
/// Represents a file from a multipart request.
/// </summary>
public class MultipartFile : IDisposable, IAsyncDisposable
{
    public MultipartFile(string key, string fileName, Stream stream, ContentDescriptor? contentDescriptor)
    {
        Key = key;
        FileName = fileName;
        ContentDescriptor = contentDescriptor;
        Stream = stream;
    }

    ~MultipartFile()
    {
        Dispose(false);
    }

    private bool _disposed;
    
    /// <summary>
    /// A string value to uniquely identify the file in the request.
    /// </summary>
    public string Key { get; }
    
    /// <summary>
    /// The file name.
    /// </summary>
    public string FileName { get; }
    
    /// <summary>
    /// The file content as a stream.
    /// </summary>
    public Stream Stream { get; }
    
    /// <summary>
    /// The file content descriptor.
    /// </summary>
    public ContentDescriptor? ContentDescriptor { get; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }
    
    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
            Stream.Dispose();

        _disposed = true;
    }
    
    private async Task DisposeAsync(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
            await Stream.DisposeAsync();

        _disposed = true;
    }
}