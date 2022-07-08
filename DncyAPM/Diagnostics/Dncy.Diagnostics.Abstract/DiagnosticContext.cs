using System.Diagnostics;

namespace Dncy.Diagnostics.Abstract;


public class DiagnosticContextAccessor
{
    private static readonly AsyncLocal<DiagnosticContext> context = new AsyncLocal<DiagnosticContext>();


    public DiagnosticContext? Context
    {
        get => context.Value;
        set
        {
            if (value == null)
                return;
            context.Value = value;
        }
    }

}



public class DiagnosticContext:IDisposable,IAsyncDisposable
{
    public IDictionary<string, object> Items;
    private IServiceProvider requestServices;
    public readonly Stopwatch Stopwatch;
    public bool IsAvalable = false;

    public DiagnosticContext(IServiceProvider requestServices)
    {
        this.requestServices = requestServices;
        Stopwatch = new Stopwatch();
        Items=new Dictionary<string, object>();
        IsAvalable = true;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        IsAvalable = false;
        Stopwatch.Reset();
        Stopwatch.Stop();
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        IsAvalable = false;
        Stopwatch.Reset();
        Stopwatch.Stop();
        return ValueTask.CompletedTask;
    }
}