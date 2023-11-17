namespace MQMessageActivatorTest.Stores;

public class DemoStore
{
    private readonly ILogger<DemoStore> _logger;

    public DemoStore(ILogger<DemoStore> logger)
    {
        _logger = logger;
    }

    public void OutPutHashCode()
    {
        _logger.LogInformation("demo store hashcode: {0}",this.GetHashCode());
    }
}


public class TranDemoStore
{
    private readonly ILogger<DemoStore> _logger;

    public TranDemoStore(ILogger<DemoStore> logger)
    {
        _logger = logger;
    }

    public void OutPutHashCode()
    {
        _logger.LogInformation("tran demo store hashcode: {0}",this.GetHashCode());
    }
}