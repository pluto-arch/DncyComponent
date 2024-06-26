namespace Dotnetydd.QuartzHost.Models;

public class JobDefined
{
    public Dictionary<string, Type> JobDictionary { get; set; }
}

public class JobSetting
{
    public bool Enabled { get; set; }

    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string GroupName { get; set; }

    public string Description { get; set; }

    public string Cron { get; set; }

}