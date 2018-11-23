using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Db
{
    public static class DbContextFactoryExtensions
    {
        public static IServiceCollection AddDbContextFactory(this IServiceCollection serviceCollection, Action<DbContextFactoryOptions> dbContextFactoryOptions)
        {
            serviceCollection.AddScoped<IDbContextFactory, DbContextFactory>();
            //serviceCollection.AddTransient(typeof(RepositoryOptions<>));
            serviceCollection.Configure(dbContextFactoryOptions);

            return serviceCollection;
        }
    }
}
