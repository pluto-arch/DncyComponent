using Dncy.Specifications.EntityFrameworkCore.Evaluatiors;
using Dncy.Specifications.Evaluators;
using Microsoft.EntityFrameworkCore;

namespace Dncy.Specifications.EntityFrameworkCore;

public static class DbSetExtensions
{
    public static async Task<List<TSource>> ToListAsync<TSource>(this DbSet<TSource> source,
        ISpecification<TSource> specification, CancellationToken cancellationToken = default) where TSource : class
    {
        List<TSource> result = await EfCoreSpecificationEvaluator.Default.GetQuery(source, specification)
            .ToListAsync(cancellationToken);

        return specification.PostProcessingAction == null
            ? result
            : specification.PostProcessingAction(result).ToList();
    }

    public static async Task<IEnumerable<TSource>> ToEnumerableAsync<TSource>(this DbSet<TSource> source,
        ISpecification<TSource> specification, CancellationToken cancellationToken = default) where TSource : class
    {
        List<TSource> result = await EfCoreSpecificationEvaluator.Default.GetQuery(source, specification)
            .ToListAsync(cancellationToken);

        return specification.PostProcessingAction == null
            ? result
            : specification.PostProcessingAction(result);
    }

    public static IQueryable<TSource> WithSpecification<TSource>(this IQueryable<TSource> source,
        ISpecification<TSource> specification, ISpecificationEvaluator evaluator = null) where TSource : class
    {
        evaluator ??= EfCoreSpecificationEvaluator.Default;
        return evaluator.GetQuery(source, specification);
    }
}