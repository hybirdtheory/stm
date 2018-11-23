using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core
{
    /// <summary>
    /// aop切面上下文
    /// </summary>
    public class AspectContext
    {
        /// <summary>
        /// 类型
        /// 0:接口拦截
        /// 1:类拦截
        /// </summary>
        public int Type { get; private set; }

        public object ReturnValue { get; set; }
    }
}
