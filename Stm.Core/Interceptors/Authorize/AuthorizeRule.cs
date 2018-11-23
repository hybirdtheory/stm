using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Interceptors
{
    /// <summary>
    /// 授权规则
    /// </summary>
    public enum AuthorizeRule
    {
        /// <summary>
        /// 拒绝
        /// </summary>
        Deny,

        /// <summary>
        /// 允许
        /// </summary>
        Allow
    }
}
