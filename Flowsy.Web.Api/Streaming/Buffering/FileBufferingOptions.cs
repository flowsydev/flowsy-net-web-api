using System.Buffers;

namespace Flowsy.Web.Api.Streaming.Buffering;

/// <summary>
/// File buffering options.
/// </summary>
public class FileBufferingOptions
{
    /// <summary>
    /// The maximum size to buffer in memory.
    /// </summary>
    public int? MemoryThreshold { get; set; }
    
    /// <summary>
    /// The maximum size that will be buffered before the Stream throws.
    /// </summary>
    public int? BufferLimit { get; set; }
    
    /// <summary>
    /// The temporary directory to which files are buffered to.
    /// </summary>
    public string? TempFileDirectory { get; set; }
    
    /// <summary>
    /// Provides the temporary directory to which files are buffered to.
    /// </summary>
    public Func<string>? TempFileDirectoryAccessor { get; set; }

    /// <summary>
    /// The array pool to use.
    /// </summary>
    public ArrayPool<byte>? BytePool { get; set; }
}