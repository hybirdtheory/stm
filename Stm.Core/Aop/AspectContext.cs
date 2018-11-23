using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Aop
{
    /// <summary>
    /// 切面上下文
    /// </summary>
    public class AspectContext
    {
        public ExecutingMethodDescriptor Method { get; set; }


        private object _returnValue=null;

        private bool _returnValueIsSeted = false;

        /// <summary>
        /// 设置或获取方法的返回结果
        /// </summary>
        public Object ReturnValue {
            get { return _returnValue; }
            set { _returnValue = value;_returnValueIsSeted = true; }
        }

        public bool ReturnValueIsSeted
        {
            get { return _returnValueIsSeted; }
            set { _returnValueIsSeted = true; }
        }

        /// <summary>
        /// 异常信息
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// 异常是否已处理
        /// </summary>
        public bool ExceptionIsHandled { get; set; }
    }
}
