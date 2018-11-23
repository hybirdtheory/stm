using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Domain.Generic
{
    /// <summary>
    /// 需要做审计的方法贴此标签
    /// </summary>
    public class AuditAttribute: Attribute
    {
        /// <summary>
        /// 审计信息
        /// </summary>
        public string LogContentFormat { get; set; }

        public AuditAttribute(string content )
        {
            LogContentFormat = content;
        }
    }
}
