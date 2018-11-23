using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Stm.Core
{
    public class ServiceHandleMap
    {

        public Dictionary<string, MethodInfo> Handlers { get; private set; }

        public ServiceHandleMap ()
        {
            Handlers = new Dictionary<string, MethodInfo>();
        }

        public ServiceHandleMap AddHandle<IInterface>(string prefix = null)
        {
            var type = typeof( IInterface );
            if (!type.IsInterface)
            {
                throw new Exception( $"type {type.FullName} is not a interface" );
            }

            var serviceName = type.Name;
            if (serviceName.StartsWith( "I" )) serviceName = serviceName.Substring( 1, serviceName.Length - 1 );
            if (serviceName.EndsWith( "Service" )) serviceName = serviceName.Substring( 0, serviceName.Length - 7 );
            if (type.GetCustomAttribute<NameAttribute>() != null)
            {
                serviceName = type.GetCustomAttribute<NameAttribute>().Value;
            }

            foreach (var method in type.GetMethods())
            {
                var actionName = method.Name;
                if (method.GetCustomAttribute<NameAttribute>() != null)
                {
                    actionName = method.GetCustomAttribute<NameAttribute>().Value;
                }

                Handlers.Add( (prefix + serviceName + "/" + actionName).ToLower() ,method);
            }

            return this;
        }

    }


}
