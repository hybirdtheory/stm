using Stm.Core.Aop;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Interceptors
{
    public class MethodExcuteTraceInterceptorOptions
    {
        /// <summary>
        /// 拦截器优先级
        /// </summary>
        //public int Order { get; private set; }


        public List<AspectPredicate> Predicates { get;private set; }

        public MethodExcuteTraceInterceptorOptions ()
        {
            Predicates = new List<AspectPredicate>();
        }

        //public MethodExcuteTraceInterceptorOptions SetOrder(int order)
        //{
        //    Order = order;

        //    return this;
        //}

        public MethodExcuteTraceInterceptorOptions AddPredicate ( AspectPredicate aspectPredicate)
        {
            Predicates.Add( aspectPredicate );

            return this;
        }
    }
}
