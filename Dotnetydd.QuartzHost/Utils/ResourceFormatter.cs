namespace Dotnetydd.QuartzHost.Utils;

internal static class ResourceFormatter
{
    internal static string GetName(string name, string uid)
    {
        return $"{name}-{uid.Substring(0, Math.Min(uid.Length, 7))}";
    }
}