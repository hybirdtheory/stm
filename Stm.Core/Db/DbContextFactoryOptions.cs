using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Db
{
    public class DbContextFactoryOptions
    {
        public Dictionary<string, Func<IServiceProvider, DbContext>> Registrations { get;private set; }

        public DbContextFactoryOptions()
        {
            Registrations = new Dictionary<string, Func<IServiceProvider, DbContext>>();
        }

        public DbContextFactoryOptions Register(string key, Func<IServiceProvider, DbContext> func)
        {
            Registrations.Add(key, func);

            return this;
        }
    }
}
