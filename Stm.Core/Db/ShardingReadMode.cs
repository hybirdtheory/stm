using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Db
{
    /// <summary>
    /// 分库读取模式
    /// </summary>
    public enum ShardingReadMode
    {
        /// <summary>
        /// 哈希模式，只从hash对应的表查找
        /// </summary>
        Hash = 0,

        /// <summary>
        /// 完全查找模式，从所有的库查找
        /// </summary>
        Full=1
    }
}
