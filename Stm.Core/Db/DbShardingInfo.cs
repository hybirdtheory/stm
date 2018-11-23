using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stm.Core.Db
{
    /// <summary>
    /// 数据库分库信息
    /// </summary>
    public class DbShardingInfo
    {
        /// <summary>
        /// 表名和分库信息映射表
        /// </summary>
        public Dictionary<String,List<DbShardingItem>> Mappings { get;private set; }

        public DbShardingInfo ()
        {
            Mappings = new Dictionary<string, List<DbShardingItem>>();
        }

    }
}
