using System.Threading;

namespace Dncy.PipelinePattern
{


    public class DataContext
    {
        private int _count;

        public int Count => _count;

        public void Increment()
        {
            _count=Interlocked.Increment(ref _count);
        }
    }
}