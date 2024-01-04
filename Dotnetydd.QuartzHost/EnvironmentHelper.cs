namespace Dotnetydd.QuartzHost;

internal class EnvironmentHelper
{
    internal static Uri[] GetAddressUris(string variableName, string defaultValue)
    {
        var urls = Environment.GetEnvironmentVariable(variableName) ?? defaultValue;
        try
        {
            return urls.Split(';').Select(url => new Uri(url)).ToArray();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error parsing URIs from environment variable '{variableName}'.", ex);
        }
    }
}