namespace Dncy.RateLimit.AspNetCore.Options;

public class LimitConfigurationOption
{
    public Ratelimit RateLimit { get; set; }
}

public class Ratelimit
{
    public RequestPath[] RequestPath { get; set; }


    public IP[] IP { get; set; }


    public IpWithRequestPath[] IPWithRequestPath { get; set; }


    public GeneralRules GeneralRules { get; set; }
}

public class GeneralRules
{
    public string[] FW { get; set; }
}

public class RequestPath
{
    public string Target { get; set; }
    public string Alog { get; set; }
    public string[] Rule { get; set; }
}

public class IP
{
    public string Target { get; set; }
    public string Alog { get; set; }
    public string[] Rule { get; set; }
}

public class IpWithRequestPath
{
    public string Target { get; set; }
    public string Alog { get; set; }
    public string[] Rule { get; set; }
}
