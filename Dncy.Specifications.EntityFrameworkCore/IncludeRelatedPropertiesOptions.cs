using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dncy.Specifications.EntityFrameworkCore
{
    public class IncludeRelatedPropertiesOptions
    {
        private readonly IDictionary<Type, object> _includeOptions = new Dictionary<Type, object>();

        public void ConfigIncludes<TEntity>(Func<IQueryable<TEntity>, IQueryable<TEntity>> action)
            where TEntity : class
        {
            _includeOptions.TryAdd(typeof(TEntity), action);
        }

        public Func<IQueryable<TEntity>, IQueryable<TEntity>> Get<TEntity>() where TEntity : class
        {
            if (_includeOptions.TryGetValue(typeof(TEntity), out object value))
            {
                return (Func<IQueryable<TEntity>, IQueryable<TEntity>>)value;
            }

            return query => query;
        }
    }
}
