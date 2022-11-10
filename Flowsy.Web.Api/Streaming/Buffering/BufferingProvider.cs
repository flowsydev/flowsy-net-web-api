using System.Buffers;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Flowsy.Web.Api.Streaming.Buffering;

public class BufferingProvider : IBufferingProvider
{
    private readonly FileBufferingOptions? _fileBufferingOptions;

    public BufferingProvider(IOptions<FileBufferingOptions>? fileBufferingOptions)
    {
        _fileBufferingOptions = fileBufferingOptions?.Value;
    }

    public FileBufferingReadStream CreateFileBufferingReadStream(
        Stream innerStream,
        FileBufferingOptions? bufferingOptions = null
        )
    {
        var options = _fileBufferingOptions ?? bufferingOptions;
        return new FileBufferingReadStream(
            innerStream,
            options?.MemoryThreshold ?? 1024 * 30,
            options?.BufferLimit,
            options?.TempFileDirectory ?? options?.TempFileDirectoryAccessor?.Invoke() ?? Path.GetTempPath(),
            options?.BytePool ?? ArrayPool<byte>.Shared
        );
    }

    public FileBufferingWriteStream CreateFileBufferingWriteStream(FileBufferingOptions? bufferingOptions = null)
    {
        var options = _fileBufferingOptions ?? bufferingOptions;
        return new FileBufferingWriteStream(
            options?.MemoryThreshold ?? 1024 * 30,
            options?.BufferLimit,
            () => 
                options?.TempFileDirectory ?? 
                options?.TempFileDirectoryAccessor?.Invoke() ??
                Path.GetTempPath()
            );
    }
}