using System.Buffers;

namespace Flowsy.Web.Api.Forms;

public class FileBufferingOptions
{
    public int MemoryThreshold { get; set; }
    public int? BufferLimit { get; set; }
    public string TempFileDirectory { get; set; } = string.Empty;
    public ArrayPool<byte> BytePool { get; set; } = ArrayPool<byte>.Create();
}