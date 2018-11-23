using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Db
{
    /// <summary>
    /// 数据库分库信息
    /// </summary>
    public class DbShardingItem
    {
        /// <summary>
        /// id最小值
        /// </summary>
        public long IdMin { get; set; } = 0;

        /// <summary>
        /// id最大值
        /// </summary>
        public long IdMax { get; set; } = long.MaxValue;

        /// <summary>
        /// 查找模式
        /// 默认哈希模式，当扩容时应设置为完全模式。
        /// </summary>
        public ShardingReadMode ReadMode { get; set; } = ShardingReadMode.Hash;

        /// <summary>
        /// 数据库
        /// </summary>
        public List<DbInfo> Servers { get; private set; }

        public DbShardingItem ()
        {
            Servers = new List<DbInfo>();
        }
        public DbShardingItem ( List<DbInfo> servers )
        {
            Servers = servers??new List<DbInfo>();
        }
    }
}
