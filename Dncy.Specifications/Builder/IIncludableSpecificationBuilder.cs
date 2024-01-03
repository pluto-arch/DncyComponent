namespace Dotnetydd.Specifications.Builder
{
    public interface IIncludableSpecificationBuilder<T, out TProperty> : ISpecificationBuilder<T> where T : class
    {
    }
}

