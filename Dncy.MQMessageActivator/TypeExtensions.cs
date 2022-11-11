namespace Dncy.MQMessageActivator
{
    internal static class TypeExtensions
    {
        internal static bool IsOneOf(this Type type, params Type[] possibleTypes)
        {
            return possibleTypes.Any(possibleType => possibleType == type);
        }

        internal static bool IsAssignableTo(this Type type, Type baseType)
        {
            return baseType.IsAssignableFrom(type);
        }

        internal static bool IsAssignableToOneOf(this Type type, params Type[] possibleBaseTypes)
        {
            return possibleBaseTypes.Any(possibleBaseType => possibleBaseType.IsAssignableFrom(type));
        }

        internal static bool IsConstructedFrom(this Type type, Type genericType, out Type? constructedType)
        {
            constructedType = new[] { type }
                .Union(type.GetInheritanceChain())
                .Union(type.GetInterfaces())
                .FirstOrDefault(i => i.IsConstructedGenericType && i.GetGenericTypeDefinition() == genericType);

            return ( constructedType != null );
        }

        internal static bool IsReferenceOrNullableType(this Type type)
        {
            return ( !type.IsValueType || Nullable.GetUnderlyingType(type) != null );
        }

        internal static object? GetDefaultValue(this Type type)
        {
            return type.IsValueType
                ? Activator.CreateInstance(type)
                : null;
        }

        internal static Type[] GetInheritanceChain(this Type type)
        {
            var inheritanceChain = new List<Type>();

            var current = type;
            while (current.BaseType != null)
            {
                inheritanceChain.Add(current.BaseType);
                current = current.BaseType;
            }

            return inheritanceChain.ToArray();
        }
    }
}

