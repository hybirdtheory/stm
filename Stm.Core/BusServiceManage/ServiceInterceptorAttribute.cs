using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core
{
    /// <summary>
    /// 服务拦截标记
    /// </summary>
    public class ServiceInterceptorAttribute: Attribute
    {
        /// <summary>
        /// 拦截类
        /// </summary>
        public List<Type> InterceptorTypes { get; private set; }

        /// <summary>
        /// 排序，多个拦截器，index越小越先执行
        /// </summary>
        public int Index { get; set; }

        public ServiceInterceptorAttribute( List<Type> types )
        {
            if (types == null)
            {
                InterceptorTypes = new List<Type>();
                return;
            }
            foreach(var type in types)
            {
                if (!typeof( IServiceInterceptor ).IsAssignableFrom( type ))
                {
                    throw new Exception( "Property 'InterceptorType' of ServiceInterceptorAttribute must extends IInterceptor" );
                }
            }
            InterceptorTypes = types;
        }
    }
}
