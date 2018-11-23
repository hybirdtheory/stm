using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Stm.Core.Aop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stm.Mvcdemo
{
    public class StmMvcFilter :  IActionFilter, IExceptionFilter
    {
        private List<IInterceptor> _interceptors=null;
        private int excuteingIndex = -1;
        private bool isExcuteSuccess = false;

        private AspectContext _aspectContext;

        public StmMvcFilter(IServiceProvider serviceProvider )
        {
            _interceptors = serviceProvider.GetServices<IInterceptor>().ToList();
        }


        public void OnActionExecuted ( ActionExecutedContext context )
        {
            if (_interceptors == null || !_interceptors.Any()) return;

            isExcuteSuccess = true;

            _aspectContext.ReturnValue = context.Result;
            _aspectContext.ReturnValueIsSeted = false;

            for (var i = excuteingIndex; i >= 0; i--)
            {
                var interceptor = _interceptors[i];

                interceptor.PostProceed( _aspectContext );

                if (_aspectContext.ReturnValueIsSeted)
                {
                    context.Result = _aspectContext.ReturnValue as IActionResult;

                }
            }

        }

        public void OnActionExecuting ( ActionExecutingContext context )
        {
            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

            if (controllerActionDescriptor == null) return;
            
            if (_interceptors == null||!_interceptors.Any()) return;

            

            _aspectContext = new AspectContext();
            _aspectContext.Method = new ExecutingMethodDescriptor
            {
                Method = controllerActionDescriptor.MethodInfo,
                Parameters = controllerActionDescriptor.Parameters.Select( p => new ExecutingParameterDescriptor
                {
                    Name = p.Name,
                    ParamterType = p.ParameterType,
                    Value = context.ActionArguments[p.Name]
                } ).ToList()
            };

            for (var i=0;i<_interceptors.Count;i++)
            {
                var interceptor = _interceptors[i];

                var gonext= interceptor.PreProceed( _aspectContext );

                if (_aspectContext.ReturnValueIsSeted)
                {
                    context.Result = _aspectContext.ReturnValue as IActionResult;

                }

                if (!gonext)
                {
                    break;
                }

                excuteingIndex = i;
            }

        }

        public void OnException ( ExceptionContext context )
        {
            if (_interceptors == null || !_interceptors.Any()) return;

            if (isExcuteSuccess) return;

            _aspectContext.Exception = context.Exception;
            _aspectContext.ReturnValue = null;
            _aspectContext.ReturnValueIsSeted = false;


            for (var i = excuteingIndex; i >=0; i--)
            {
                var interceptor = _interceptors[i];

                interceptor.OnException( _aspectContext );

                context.Exception = _aspectContext.Exception;

                if (_aspectContext.ExceptionIsHandled)
                {
                    context.ExceptionHandled = true;

                    if (_aspectContext.ReturnValueIsSeted)
                    {
                        context.Result = _aspectContext.ReturnValue as IActionResult;

                    }
                }
            }
        }
    }
}
