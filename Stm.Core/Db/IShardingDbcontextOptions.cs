using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using static Stm.Core.Db.ShardingDbContext;

namespace Stm.Core.Db
{
    public interface IShardingDbcontextOptions
    {
        ShardingConnectionConfigure ShardingConnections { get; set; }

        Action<string, DbContextOptionsBuilder<InnerDbcontext>> InnerDbContextOptionsDelegte { get; set; }
    }

    public class ShardingDbcontextOptions<T> : IShardingDbcontextOptions
        where T:ShardingDbContext
    {
        public ShardingConnectionConfigure ShardingConnections { get; set; }

        public Action<string, DbContextOptionsBuilder<InnerDbcontext>> InnerDbContextOptionsDelegte { get; set; }

        public ShardingDbcontextOptions ( )
        {
        }

        public ShardingDbcontextOptions ( ShardingConnectionConfigure shardingConnections, Action<string, DbContextOptionsBuilder<InnerDbcontext>> innerDbContextOptionsDelegte )
        {
            ShardingConnections = shardingConnections;
            InnerDbContextOptionsDelegte = innerDbContextOptionsDelegte;
        }

        public ShardingDbcontextOptions<T> UseConfigShardingConnectionStrings( IConfiguration configuration,string key )
        {
            this.ShardingConnections = configuration.GetSection( "ShardingConnectionStrings:"+ key ).Get<ShardingConnectionConfigure>();

            return this;
        }
    }
}
