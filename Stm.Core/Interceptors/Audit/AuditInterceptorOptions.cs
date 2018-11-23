using Stm.Core.Aop;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Interceptors
{
    public class AuditInterceptorOptions
    {
        /// <summary>
        /// 拦截器优先级
        /// </summary>
        //public int Order { get; private set; }


        public List<AspectPredicate> Predicates { get;private set; }

        public AuditInterceptorOptions ()
        {
            Predicates = new List<AspectPredicate>();
        }

        //public MethodExcuteTraceInterceptorOptions SetOrder(int order)
        //{
        //    Order = order;

        //    return this;
        //}

        public AuditInterceptorOptions AddPredicate ( AspectPredicate aspectPredicate)
        {
            Predicates.Add( aspectPredicate );

            return this;
        }
    }
}
