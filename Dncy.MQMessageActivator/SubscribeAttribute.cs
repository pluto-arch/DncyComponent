using System;

namespace Dncy.MQMessageActivator
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SubscribeAttribute : Attribute
    {
        public string Template { get; set; }

        private const string prefix = "/";

        public SubscribeAttribute(string template)
        {
            if (template.StartsWith(prefix))
            {
                Template = template;
            }
            else
            {
                Template = $"/{template}";
            }
        }
    }
}

