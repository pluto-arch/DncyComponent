﻿namespace Dotnetydd.QuartzJob.AspNetCore.Models
{
    internal class JobDataResult<T>
    {
        public int Code { get; set; }

        public string Msg { get; set; }

        public T Data { get; set; }

        public int Count { get; set; }
    }
}

