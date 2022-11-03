namespace Flowsy.Web.Api.Forms;

/// <summary>
/// Describes the content of a file.
/// </summary>
public class FileContentDescriptor : ContentDescriptor
{
    public FileContentDescriptor(IEnumerable<string> mimeTypes, IEnumerable<string> fileExtensions) : base(mimeTypes)
    {
        FileExtensions = fileExtensions;
    }

    /// <summary>
    /// The first extension associated with the file content.
    /// </summary>
    public string? FileExtension => FileExtensions.FirstOrDefault();
    
    /// <summary>
    /// The extensions associated with the file content.
    /// </summary>
    public IEnumerable<string> FileExtensions { get; }
}