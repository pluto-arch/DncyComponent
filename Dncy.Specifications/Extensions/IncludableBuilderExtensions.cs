﻿

using Dotnetydd.Specifications.Builder;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dotnetydd.Specifications.Extensions
{
    public static class IncludableBuilderExtensions
    {
        public static IIncludableSpecificationBuilder<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty,
            TProperty>(
            this IIncludableSpecificationBuilder<TEntity, TPreviousProperty> previousBuilder,
            Expression<Func<TPreviousProperty, TProperty>> thenIncludeExpression)
            where TEntity : class
        {
            IncludeExpressionInfo info = new IncludeExpressionInfo(thenIncludeExpression, typeof(TEntity),
                typeof(TProperty), typeof(TPreviousProperty));

            ((List<IncludeExpressionInfo>)previousBuilder.Specification.IncludeExpressions).Add(info);

            IncludableSpecificationBuilder<TEntity, TProperty> includeBuilder =
                new IncludableSpecificationBuilder<TEntity, TProperty>(previousBuilder.Specification);

            return includeBuilder;
        }

        public static IIncludableSpecificationBuilder<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty,
            TProperty>(
            this IIncludableSpecificationBuilder<TEntity, IEnumerable<TPreviousProperty>> previousBuilder,
            Expression<Func<TPreviousProperty, TProperty>> thenIncludeExpression)
            where TEntity : class
        {
            IncludeExpressionInfo info = new IncludeExpressionInfo(thenIncludeExpression, typeof(TEntity),
                typeof(TProperty), typeof(TPreviousProperty));

            ((List<IncludeExpressionInfo>)previousBuilder.Specification.IncludeExpressions).Add(info);

            IncludableSpecificationBuilder<TEntity, TProperty> includeBuilder =
                new IncludableSpecificationBuilder<TEntity, TProperty>(previousBuilder.Specification);

            return includeBuilder;
        }
    }
}

