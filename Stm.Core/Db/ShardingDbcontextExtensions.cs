using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Db
{
    public static class ShardingDbcontextExtensions
    {
        public static IServiceCollection AddShardingDbcontext<T>( 
            this IServiceCollection serviceCollection, 
            Action<ShardingDbcontextOptions<T>> shardingDbcontextOptions )
            where T:ShardingDbContext
        {
            serviceCollection.AddScoped<T>();

            ShardingDbcontextOptions<T> options = new ShardingDbcontextOptions<T>();
            shardingDbcontextOptions( options );
            serviceCollection.Add( new ServiceDescriptor( typeof( ShardingDbcontextOptions<T> ), options ) );


            return serviceCollection;
        }
    }
}
