using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Stm.Core
{
    /// <summary>
    /// 服务适配信息
    /// </summary>
    public interface IServiceAdapter
    {
        /// <summary>
        /// 服务协议类型
        /// </summary>
        Type ServiceContractType  { get; }

        /// <summary>
        /// 传输协议
        /// 参考 ServiceCallProtocols
        /// </summary>
        string Protocol { get; }


        System.Threading.Tasks.Task<object> ExecuteServiceAsync (IServiceProvider serviceProvider, MethodInfo methodInfo,params object[] parameters );

        /// <summary>
        /// 获取代理对象
        /// </summary>
        /// <typeparam name="IInterface"></typeparam>
        /// <param name="proxyGenerator"></param>
        /// <returns></returns>
        Func<IServiceProvider, object> GetProxyObject ( ProxyGenerator proxyGenerator );
    }
}
