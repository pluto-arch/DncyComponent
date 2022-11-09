namespace Dncy.MQMessageActivator
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SubscribeAttribute : Attribute
    {
        public string Template { get; set; }

        public SubscribeAttribute(string template)
        {
            Template = template;
        }
    }
}

