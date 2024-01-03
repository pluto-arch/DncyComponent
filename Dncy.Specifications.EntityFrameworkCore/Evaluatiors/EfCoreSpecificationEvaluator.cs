using Dotnetydd.Specifications.Evaluators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dotnetydd.Specifications.EntityFrameworkCore.Evaluatiors
{
    public class EfCoreSpecificationEvaluator : ISpecificationEvaluator
    {
        private readonly List<IEvaluator> evaluators = new List<IEvaluator>();

        public EfCoreSpecificationEvaluator()
        {
            evaluators.AddRange(new IEvaluator[]
            {
                WhereEvaluator.Instance, SearchEvaluator.Instance, IncludeEvaluator.Instance,
                OrderEvaluator.Instance, PaginationEvaluator.Instance, AsNoTrackingEvaluator.Instance
            });
        }

        public EfCoreSpecificationEvaluator(IEnumerable<IEvaluator> evaluators)
        {
            this.evaluators.AddRange(evaluators);
        }

        // Will use singleton for default configuration. Yet, it can be instantiated if necessary, with default or provided evaluators.
        public static EfCoreSpecificationEvaluator Default { get; } = new EfCoreSpecificationEvaluator();

        public virtual IQueryable<TResult> GetQuery<T, TResult>(IQueryable<T> query,
            ISpecification<T, TResult> specification) where T : class
        {
            query = GetQuery(query, (ISpecification<T>)specification);

            if (specification.Selector is null)
            {
                throw new InvalidOperationException();
            }

            return query.Select(specification.Selector);
        }

        public virtual IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification,
            bool evaluateCriteriaOnly = false) where T : class
        {
            IEnumerable<IEvaluator> evaluators = evaluateCriteriaOnly
                ? this.evaluators.Where(x => x.IsCriteriaEvaluator)
                : this.evaluators;

            foreach (IEvaluator evaluator in evaluators)
            {
                query = evaluator.GetQuery(query, specification);
            }

            return query;
        }
    }
}

