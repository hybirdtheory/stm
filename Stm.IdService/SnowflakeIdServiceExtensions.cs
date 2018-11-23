using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Stm.Core.Domain.Generic;
using Microsoft.Extensions.Configuration;
using Stm.Core.SoaGovernance;

namespace Stm.IdService
{
    public static class SnowflakeIdServiceExtensions
    {
        public static void AddSnowflakeIdService ( this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<INumberIdService, SnowflakeIdService>();
        }
        public static void AddHttpSnowflakeIdService ( this IServiceCollection serviceCollection )
        {
            serviceCollection.AddHttpService<INumberIdService>( "http://{lb:numberidservice}/{service}/{action}" );

        }
    }
}
