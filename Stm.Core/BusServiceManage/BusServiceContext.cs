using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using Stm.Core.BusServiceManage;

namespace Stm.Core
{
    public class BusServiceContext: IBusServiceContext
    {
        private readonly ServiceProvider _scopedProvider;

        //private Dictionary<string, IServiceInterceptor[]> _methodInterceptorMapping;

        public string Id { get; private set; }

        public BusServiceContext ( ServiceProvider scopedProvider )
        {
            this._scopedProvider = scopedProvider;
            this.Id = Guid.NewGuid().ToString( "N" );
            //this._methodInterceptorMapping = new Dictionary<string, IServiceInterceptor[]>();
        }

        public IServiceInterceptor[] GetInterceptorsOfMethod(MethodInfo method )
        {
            //var hashKey = method.DeclaringType.FullName + "." + method.Name+"["+String.Join(",",method.GetParameters().Select(p=>p.ParameterType.FullName))+"]";

            var interceptors= method.GetCustomAttributes(true)
                        .Where(attr=>attr is ServiceInterceptorAttribute)
                        .Cast<ServiceInterceptorAttribute>()
                        .OrderBy(attr=>attr.Index)
                        .SelectMany(attr=>attr.InterceptorTypes)
                        .Select(type=> {
                            var interceptor = Activator.CreateInstance( type ) as IServiceInterceptor;
                            return interceptor;
                            } );

            return interceptors.ToArray();
        }


    }
}
