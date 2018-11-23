using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Stm.Core.Aop;

namespace Stm.Core
{
    public abstract class ServiceAdapterBase : IServiceAdapter
    {
        public abstract Type ServiceContractType { get; }

        public abstract string Protocol { get; }
        
        public abstract Task<object> ExecuteServiceAsync ( IServiceProvider serviceProvider, MethodInfo methodInfo, params object[] parameters );

        public Func<IServiceProvider, object> GetProxyObject ( ProxyGenerator proxyGenerator )
        {
            return serviceProvider =>
            {
                List<Castle.DynamicProxy.IInterceptor> interceptors = new List<Castle.DynamicProxy.IInterceptor>();
                //interceptors.AddRange( serviceProvider.GetServices<IInterceptor>());
                interceptors.AddRange( serviceProvider.GetServices<Aop.IInterceptor>().Select(t=>t.CastToCastleInterceptor()) );
                interceptors.Add( new ServiceAdapterInterceptor( this, serviceProvider ) );
                var proxyObj = proxyGenerator.CreateInterfaceProxyWithoutTarget( ServiceContractType, interceptors.ToArray());
                return proxyObj;
            };
        }
    }

    internal class ServiceAdapterInterceptor : Castle.DynamicProxy.IInterceptor
    {
        private ServiceAdapterBase _serviceAdapter;

        private IServiceProvider _serviceProvider;

        public ServiceAdapterInterceptor ( ServiceAdapterBase serviceAdapter , IServiceProvider serviceProvider )
        {
            this._serviceAdapter = serviceAdapter;
            this._serviceProvider = serviceProvider;
        }

        public void Intercept ( IInvocation invocation )
        {
            //获取参数
            var parameters = new List<object>();
            var methodParams = invocation.Method.GetParameters();
            for (var iMethodParam = 0; iMethodParam < methodParams.Length; iMethodParam++)
            {
                var name = methodParams[iMethodParam].Name;
                var value = invocation.GetArgumentValue( iMethodParam );
                parameters.Add( value );
            }

            var result = _serviceAdapter.ExecuteServiceAsync( _serviceProvider, invocation.Method, parameters.ToArray() ).Result;

            invocation.ReturnValue = result;
        }
    }
}
