using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Dotnetydd.MultiTenancy.ConnectionStrings
{
    public class DefaultConnectionStringResolver : IConnectionStringResolver
    {
        protected readonly IConfiguration _configuration;

        public DefaultConnectionStringResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public virtual Task<string> GetAsync(string connectionStringName = null)
        {
            if (string.IsNullOrEmpty(connectionStringName))
            {
                throw new InvalidOperationException("connection string name can not be empty!");
            }
            return Task.FromResult(_configuration.GetConnectionString(connectionStringName));
        }
    }

}



