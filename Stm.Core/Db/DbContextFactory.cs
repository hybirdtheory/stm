using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Stm.Core.Db
{
    public class DbContextFactory : IDbContextFactory
    {
        protected readonly Dictionary<string, Func<IServiceProvider,DbContext>> Registrations;

        protected IServiceProvider ServiceProvider;

        public DbContextFactory(IServiceProvider serviceProvider,IOptions<DbContextFactoryOptions> options)
        {
            Registrations = new Dictionary<string, Func<IServiceProvider,DbContext>>();
            if (options != null)
            {
                foreach(var item in options.Value.Registrations)
                {
                    AddRegistration(item.Key, item.Value);
                }
            }
            ServiceProvider = serviceProvider;
        }

        public void AddRegistration(string key, Func<IServiceProvider, DbContext> func)
        {
            Registrations.Add(key, func);
        }

        public DbContext GetDbContext(string registration)
        {
            var func = Registrations[registration];

            if (func == null)
            {
                throw new Exception($"no registration for {registration} found");
            }

            return func(ServiceProvider);
        }
    }
}
