﻿using Dotnetydd.Specifications.Builder;
using Dotnetydd.Specifications.Evaluators;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dotnetydd.Specifications
{

    public abstract class Specification<T, TResult> : Specification<T>, ISpecification<T, TResult>
    {
        protected Specification()
            : this(InMemorySpecificationEvaluator.Default)
        {
        }

        protected Specification(IInMemorySpecificationEvaluator inMemorySpecificationEvaluator)
            : base(inMemorySpecificationEvaluator)
        {
            Query = new SpecificationBuilder<T, TResult>(this);
        }

        protected new virtual ISpecificationBuilder<T, TResult> Query { get; }

        public new virtual IEnumerable<TResult> Evaluate(IEnumerable<T> entities)
        {
            return Evaluator.Evaluate(entities, this);
        }

        public Expression<Func<T, TResult>> Selector { get; internal set; }

        public new Func<IEnumerable<TResult>, IEnumerable<TResult>> PostProcessingAction { get; internal set; } = null;
    }


    public abstract class Specification<T> : ISpecification<T>
    {
        protected Specification()
               : this(InMemorySpecificationEvaluator.Default)
        {
        }

        protected Specification(IInMemorySpecificationEvaluator inMemorySpecificationEvaluator)
        {
            Evaluator = inMemorySpecificationEvaluator;
            Query = new SpecificationBuilder<T>(this);
        }

        protected IInMemorySpecificationEvaluator Evaluator { get; }
        protected virtual ISpecificationBuilder<T> Query { get; }

        public virtual IEnumerable<T> Evaluate(IEnumerable<T> entities)
        {
            return Evaluator.Evaluate(entities, this);
        }

        public IEnumerable<Expression<Func<T, bool>>> WhereExpressions { get; } = new List<Expression<Func<T, bool>>>();

        public IEnumerable<(Expression<Func<T, object>> KeySelector, OrderTypeEnum OrderType)>
            OrderExpressions
        { get; } =
            new List<(Expression<Func<T, object>> KeySelector, OrderTypeEnum OrderType)>();

        public IEnumerable<IncludeExpressionInfo> IncludeExpressions { get; } = new List<IncludeExpressionInfo>();

        public IEnumerable<string> IncludeStrings { get; } = new List<string>();

        public IEnumerable<(Expression<Func<T, string>> Selector, string SearchTerm, int SearchGroup)> SearchCriterias =>
            new List<(Expression<Func<T, string>> Selector, string SearchTerm, int SearchGroup)>();

        public int? Take { get; internal set; } = null;

        public int? Skip { get; internal set; } = null;

        public Func<IEnumerable<T>, IEnumerable<T>> PostProcessingAction { get; internal set; } = null;

        public string CacheKey { get; internal set; }

        public bool CacheEnabled { get; internal set; }

        public bool AsNoTracking { get; internal set; } = false;

        public bool AsSplitQuery { get; internal set; } = false;

        public bool AsNoTrackingWithIdentityResolution { get; internal set; } = false;
    }

}
