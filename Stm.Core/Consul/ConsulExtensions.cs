using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Stm.Core.Domain.Generic;
using Microsoft.Extensions.Configuration;
using Stm.Core.SoaGovernance;

namespace Stm.Core.Consul
{
    public static class ConsulExtensions
    {

        public static void AddConsulMicroServiceLocator ( this IServiceCollection serviceCollection )
        {
            serviceCollection.AddSingleton<IMicroServiceLocator, ConsulMicroServiceLocator>();
        }

    }
}
