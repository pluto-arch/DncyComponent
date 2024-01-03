using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Dotnetydd.Specifications.Evaluators
{
    public class WhereEvaluator : IEvaluator, IInMemoryEvaluator
    {
        private WhereEvaluator() { }

        public static WhereEvaluator Instance { get; } = new WhereEvaluator();

        public bool IsCriteriaEvaluator { get; } = true;

        public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
        {
            foreach (Expression<Func<T, bool>> criteria in specification.WhereExpressions)
            {
                query = query.Where(criteria);
            }

            return query;
        }

        public IEnumerable<T> Evaluate<T>(IEnumerable<T> query, ISpecification<T> specification)
        {
            foreach (Expression<Func<T, bool>> criteria in specification.WhereExpressions)
            {
                query = query.Where(criteria.Compile());
            }

            return query;
        }
    }
}

