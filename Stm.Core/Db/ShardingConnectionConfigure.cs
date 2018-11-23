using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Stm.Core.Db
{

    public  class ShardingConnectionConfigure: List<ShardingConnectionConfigureItem>
    {
        public List<DbShardingItem> GetDbShardingItems ( )
        {
            List<DbShardingItem> dbShardingItems = new List<DbShardingItem>();
            foreach(var item in this)
            {
                DbShardingItem dbShardingItem = new DbShardingItem( item.Servers.Select( t => new DbInfo
                {
                    Name = t.Key,
                    ConnectionString = t.Value
                } ).ToList() );
                dbShardingItem.IdMax = item.IdMax;
                dbShardingItem.IdMin = item.IdMin;
                dbShardingItem.ReadMode = item.ReadMode;

                dbShardingItems.Add( dbShardingItem );
            }

            return dbShardingItems;
        }

    }


    public class ShardingConnectionConfigureItem
    {
        public long IdMin { get; set; }
        public long IdMax { get; set; }
        public ShardingReadMode ReadMode { get; set; }
        public Dictionary<string, string> Servers { get; set; }
    }
}
