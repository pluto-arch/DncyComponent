using System;
using System.Collections.Generic;
using System.Threading;

namespace Dncy.PipelinePattern
{


    public class DataContext
    {

        public DataContext()
        {
            Properties=new Dictionary<string, object>();
        }

        public Dictionary<string,object> Properties { get; set; }

        public string Body { get; set; }
    }
}