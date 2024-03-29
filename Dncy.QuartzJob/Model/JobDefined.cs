﻿using System;
using System.Collections.Generic;

namespace Dotnetydd.QuartzJob.Model
{
    public class JobDefined
    {
        public Dictionary<string, Type> JobDictionary { get; set; }
    }

    public class JobSetting
    {
        public bool IsOpen { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string GroupName { get; set; }

        public string Description { get; set; }

        public string Cron { get; set; }

    }

}

