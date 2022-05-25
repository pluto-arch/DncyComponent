namespace Dncy.MultiTenancy
{
    internal class DisposeAction : IDisposable
    {
        private readonly Action _action;

        public DisposeAction(Action action)
        {
            _action = action;
        }

        void IDisposable.Dispose()
        {
            _action();
            GC.SuppressFinalize(this);
        }
    }


    internal class AsyncDisposeAction : IAsyncDisposable
    {
        private readonly Action _action;

        public AsyncDisposeAction(Action action)
        {
            _action = action;
        }

        public ValueTask DisposeAsync()
        {
            _action();
            GC.SuppressFinalize(this);
            return ValueTask.CompletedTask;
        }
    }
}