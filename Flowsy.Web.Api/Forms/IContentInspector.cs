using System.Collections.Immutable;

namespace Flowsy.Web.Api.Forms;

/// <summary>
/// Inspects the given content to obtain its descriptor.
/// </summary>
public interface IContentInspector
{
    /// <summary>
    /// Inspects content of a file given its path.
    /// </summary>
    /// <param name="filePath">The full path to the file.</param>
    /// <returns>An instance of FileContentDescriptor.</returns>
    public FileContentDescriptor InspectFile(string filePath);
    
    /// <summary>
    /// Inspects content of a file given a stream.
    /// </summary>
    /// <param name="stream">The file content as a stream.</param>
    /// <returns>An instance of FileContentDescriptor.</returns>
    public FileContentDescriptor InspectFile(Stream stream);
    
    /// <summary>
    /// Inspects content of a file given a byte array.
    /// </summary>
    /// <param name="array">The file content as a byte array.</param>
    /// <returns>An instance of FileContentDescriptor.</returns>
    public FileContentDescriptor InspectFile(ImmutableArray<byte> array);
}