
using Castle.DynamicProxy;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Stm.Core.Aop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Stm.Core.Interceptors
{
    /// <summary>
    /// 方法执行日志拦截器
    /// </summary>
    public class MethodExcuteTraceInterceptor : Castle.DynamicProxy.IInterceptor, Aop.IInterceptor
    {
        private IMethodExcuteTraceRepository _methodExcuteTraceRepository;
        private List<AspectPredicate> _predicates;


        public MethodExcuteTraceInterceptor(IMethodExcuteTraceRepository methodExcuteTraceRepository ,IOptions<MethodExcuteTraceInterceptorOptions> options)
            : base()
        {
            _methodExcuteTraceRepository = methodExcuteTraceRepository;
            _predicates = options.Value.Predicates ?? new List<AspectPredicate>();
            //Order = options.Value.Order;
        }

        public void Intercept ( IInvocation invocation )
        {
            if (!isMatch( invocation.Method ))
            {
                invocation.Proceed();
                return;
            }

            MethodExcuteTraceInfo traceInfo = new MethodExcuteTraceInfo();

            traceInfo.CallDt = DateTime.Now;
            traceInfo.ServiceName = invocation.MethodInvocationTarget?.DeclaringType.Name;
            traceInfo.InterfaceName = invocation.Method.DeclaringType.Name;
            traceInfo.MethodName = invocation.Method.Name;
            if (invocation.Arguments != null && invocation.Arguments.Any())
            {
                //过滤掉接口
                //Dictionary<string, object> parameters = new Dictionary<string, object>();

                traceInfo.ParameterInfo = JsonConvert.SerializeObject( invocation.Arguments );
            }

            try
            {

                invocation.Proceed();
            }
            catch (Exception e)
            {
                traceInfo.IsError = true;
                traceInfo.ErrorMessage = e.Message;
                traceInfo.ErrorStackTrace = e.StackTrace;

                throw;
            }
            finally
            {
                traceInfo.CompleteDt = DateTime.Now;
                if (!traceInfo.IsError && invocation.ReturnValue != null)
                {
                    traceInfo.ResultData = JsonConvert.SerializeObject( invocation.ReturnValue );
                }

                if (_methodExcuteTraceRepository != null)
                {
                    _methodExcuteTraceRepository.SaveMethodExcuteTraceInfo( traceInfo );
                }
            }
        }

        MethodExcuteTraceInfo excuteTraceInfo = null;

        public void OnException ( Aop.AspectContext aspectContext )
        {
            if (!isMatch( aspectContext.Method.Method )) return;

            excuteTraceInfo.IsError = true;
            excuteTraceInfo.ErrorMessage = aspectContext.Exception.Message;
            excuteTraceInfo.ErrorStackTrace = aspectContext.Exception.StackTrace;
        }

        public void PostProceed ( Aop.AspectContext aspectContext )
        {
            if (!isMatch( aspectContext.Method.Method )) return;

            excuteTraceInfo.CompleteDt = DateTime.Now;
            if (!excuteTraceInfo.IsError && aspectContext.ReturnValue != null)
            {
                excuteTraceInfo.ResultData = JsonConvert.SerializeObject( aspectContext.ReturnValue );
            }

            if (_methodExcuteTraceRepository != null)
            {
                _methodExcuteTraceRepository.SaveMethodExcuteTraceInfo( excuteTraceInfo );
            }
        }

        public bool PreProceed ( Aop.AspectContext aspectContext )
        {
            if (!isMatch( aspectContext.Method.Method )) return true;

            excuteTraceInfo = new MethodExcuteTraceInfo();

            excuteTraceInfo.CallDt = DateTime.Now;
            //excuteTraceInfo.ServiceName = invocation.MethodInvocationTarget?.DeclaringType.Name;
            excuteTraceInfo.InterfaceName = aspectContext.Method.Method.DeclaringType.Name;
            excuteTraceInfo.MethodName = aspectContext.Method.Method.Name;
            if (aspectContext.Method.Parameters != null && aspectContext.Method.Parameters.Any())
            {
                excuteTraceInfo.ParameterInfo = JsonConvert.SerializeObject( aspectContext.Method.Parameters );
            }

            return true;
        }

        private bool isMatch(MethodInfo methodInfo )
        {
            return _predicates.Any( t => t( methodInfo ) );
        }
    }
}
