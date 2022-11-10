using Microsoft.AspNetCore.WebUtilities;

namespace Flowsy.Web.Api.Streaming.Buffering;

/// <summary>
/// Provides data buffering functionality.
/// </summary>
public interface IBufferingProvider
{
    /// <summary>
    /// Creates a Stream that wraps another stream and enables rewinding by buffering the content as it is read.
    /// The content is buffered in memory up to a certain size and then spooled to a temp file on disk.
    /// The temp file will be deleted on Dispose.
    /// </summary>
    /// <param name="innerStream">The wrapped stream.</param>
    /// <param name="bufferingOptions">Optional buffering options.</param>
    /// <returns>An instance of FileBufferingReadStream.</returns>
    FileBufferingReadStream CreateFileBufferingReadStream(
        Stream innerStream,
        FileBufferingOptions? bufferingOptions = null
        );

    /// <summary>
    /// Creates a Stream that buffers content to be written to disk. Use DrainBufferAsync(Stream, CancellationToken)
    /// to write buffered content to a target Stream.
    /// </summary>
    /// <param name="bufferingOptions"></param>
    /// <returns></returns>
    FileBufferingWriteStream CreateFileBufferingWriteStream(
        FileBufferingOptions? bufferingOptions = null
        );
}