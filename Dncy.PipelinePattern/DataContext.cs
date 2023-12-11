using System.Collections.Generic;

namespace Dncy.PipelinePattern
{


    public class DataContext
    {

        public DataContext()
        {
            Properties = new Dictionary<string, object>();
        }

        public Dictionary<string, object> Properties { get; set; }

        public string Body { get; set; }
    }
}