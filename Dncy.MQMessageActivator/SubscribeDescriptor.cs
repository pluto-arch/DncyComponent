using System.Reflection;

namespace Dncy.MQMessageActivator
{
    public class SubscribeDescriptor
    {
        public SubscribeAttribute AttributeRouteInfo { get; set; } = default!;

        public MethodInfo MethodInfo { get; set; } = default!;

        public IList<ParameterInfo> Parameters { get; set; } = Array.Empty<ParameterInfo>();
    }
}

