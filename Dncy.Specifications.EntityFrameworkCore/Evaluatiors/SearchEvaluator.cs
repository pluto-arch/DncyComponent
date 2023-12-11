using Dotnetydd.Specifications.EntityFrameworkCore.Extensions;
using Dotnetydd.Specifications.Evaluators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Dotnetydd.Specifications.EntityFrameworkCore.Evaluatiors
{
    public class SearchEvaluator : IEvaluator
    {
        private SearchEvaluator() { }

        public static SearchEvaluator Instance { get; } = new SearchEvaluator();

        public bool IsCriteriaEvaluator { get; } = true;

        public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
        {
            foreach (IGrouping<int, (Expression<Func<T, string>> Selector, string SearchTerm, int SearchGroup)>
                         searchCriteria in specification.SearchCriterias.GroupBy(x => x.SearchGroup))
            {
                IEnumerable<(Expression<Func<T, string>> Selector, string SearchTerm)> criterias =
                    searchCriteria.Select(x => (x.Selector, x.SearchTerm));
                query = query.Search(criterias);
            }

            return query;
        }
    }
}

