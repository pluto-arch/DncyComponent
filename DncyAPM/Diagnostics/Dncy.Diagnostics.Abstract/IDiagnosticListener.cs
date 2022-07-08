namespace Dncy.Diagnostics.Abstract
{
    public interface IDiagnosticListener: IObserver<KeyValuePair<string, object>>
    {
        string Name { get; }  
    }
}