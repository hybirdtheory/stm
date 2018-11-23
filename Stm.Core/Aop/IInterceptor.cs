using System;
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy;

namespace Stm.Core.Aop
{
    public interface IInterceptor
    {
        bool PreProceed ( AspectContext aspectContext );

        void PostProceed ( AspectContext aspectContext );

        void OnException ( AspectContext aspectContext );
    }


    public class CastleProxyInterceptor : Castle.DynamicProxy.IInterceptor
    {
        private IInterceptor _interceptor;

        public CastleProxyInterceptor(IInterceptor interceptor )
        {
            _interceptor = interceptor;
        }

        public void Intercept ( IInvocation invocation )
        {
            AspectContext aspectContext = new AspectContext
            {
                Method = new ExecutingMethodDescriptor
                {
                    Method = invocation.Method
                }
            };

            var args = invocation.Method.GetParameters();
            for(int i = 0; i < args.Length; i++)
            {
                ExecutingParameterDescriptor parameterDescriptor = new ExecutingParameterDescriptor();
                parameterDescriptor.Name = args[i].Name;
                parameterDescriptor.ParamterType = args[i].ParameterType;
                parameterDescriptor.Value = invocation.Arguments[i];

                aspectContext.Method.Parameters.Add( parameterDescriptor );
            }

            var gonext=  _interceptor.PreProceed( aspectContext );

            if (gonext)
            {
                try
                {
                    invocation.Proceed();
                }
                catch (Exception e)
                {
                    aspectContext.Exception = e;

                    _interceptor.OnException( aspectContext );

                    if (aspectContext.ReturnValueIsSeted)
                    {
                        invocation.ReturnValue = aspectContext.ReturnValue;
                    }

                    if (!aspectContext.ExceptionIsHandled)
                    {
                        throw aspectContext.Exception;
                    }
                }
            }

            _interceptor.PostProceed( aspectContext );

        }
    }

    public static class InterceptorExtensions
    {
        public static Castle.DynamicProxy.IInterceptor CastToCastleInterceptor(this IInterceptor interceptor )
        {
            return new CastleProxyInterceptor( interceptor );
        }
    }
}
