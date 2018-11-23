using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Domain.Generic
{
    /// <summary>
    /// 资源授权信息，用于持久化
    /// </summary>
    public class ResourceGrantInfo
    {
        /// <summary>
        /// token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 授权描述信息
        /// </summary>
        public string ResourceDescriptor { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDt { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpireDt { get; set; }

        /// <summary>
        /// 使用次数
        /// </summary>
        public int UseTimes { get; set; }

    }
}
