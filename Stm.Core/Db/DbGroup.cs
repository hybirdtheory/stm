using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Db
{
    /// <summary>
    /// 可用数据库组
    /// 包含一个master，N个salver
    /// </summary>
    public class DbGroup
    {
        public DbInfo Master { get; set; }

        public List<DbInfo> Salvers { get; set; }
    }
}
