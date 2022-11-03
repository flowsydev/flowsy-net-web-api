namespace Flowsy.Web.Api.Forms;

/// <summary>
/// Describes a given content.
/// </summary>
public class ContentDescriptor
{
    public ContentDescriptor(IEnumerable<string> mimeTypes)
    {
        MimeTypes = mimeTypes;
    }

    /// <summary>
    /// The first MIME type associated to the content.
    /// </summary>
    public string? MimeType => MimeTypes.FirstOrDefault();
    
    /// <summary>
    /// The MIME types associated to the content.
    /// </summary>
    public IEnumerable<string> MimeTypes { get; }
}