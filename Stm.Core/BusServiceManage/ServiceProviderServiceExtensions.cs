using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.BusServiceManage
{
    public static class ServiceProviderServiceExtensions
    {
        public static IBusServiceContext CreateScope ( this IServiceProvider provider )
        {
            return provider.GetRequiredService<IBusServiceContext>();
        }
    }
}
