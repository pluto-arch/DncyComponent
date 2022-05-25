using Dncy.Specifications.Evaluators;
using Microsoft.EntityFrameworkCore;

namespace Dncy.Specifications.EntityFrameworkCore.Evaluatiors;

public class AsNoTrackingEvaluator: IEvaluator
{
    private AsNoTrackingEvaluator() { }

    public static AsNoTrackingEvaluator Instance { get; } = new();

    public bool IsCriteriaEvaluator { get; } = true;

    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
    {
        if (specification.AsNoTracking)
        {
            query = query.AsNoTracking();
        }

        return query;
    }
}