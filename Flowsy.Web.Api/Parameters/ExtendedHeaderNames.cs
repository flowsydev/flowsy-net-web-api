using System.Text.RegularExpressions;

namespace Flowsy.Web.Api.Parameters;

public static class ExtendedHeaderNames
{
    public const string ApiKey = "X-{ClientId}-ApiKey";
    public static Regex ApiKeyRegex { get; } = new (ApiKey.Replace("{ClientId}", "(.+)"), RegexOptions.IgnoreCase);
    public static string Version { get; set; } = "X-Version";
}