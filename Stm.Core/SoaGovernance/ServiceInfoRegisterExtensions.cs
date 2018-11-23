using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Stm.Core.Domain.Generic;
using Microsoft.Extensions.Configuration;
using Stm.Core.SoaGovernance;

namespace Stm.Core.SoaGovernance
{
    public static class ServiceInfoRegisterExtensions
    {
        public static void AddServiceRegInfo ( this IServiceCollection serviceCollection, IConfiguration consulConfig )
        {
            serviceCollection.Configure<ServiceInfoRegisterConfig>( consulConfig );
        }
    }
}
