using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Stm.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Stm.Core.CodeGeneration;

namespace Stm.AspNetCore
{
    /// <summary>
    /// http微服务中间件
    /// </summary>
    public class StmHttpMicroServiceServerMiddleware
    {
        private readonly RequestDelegate _next;
        private ServiceHandleMap _serviceHandleMap;
        private HttpExceptionPolicy _exceptionPolicyItemBundle;
        private ExceptionPolicy<HttpContext, VOID> _exceptionPolicy;
        private string _codegenParamName = "gen";

        public StmHttpMicroServiceServerMiddleware ( RequestDelegate next, ServiceHandleMap serviceHandleMap, HttpExceptionPolicy policyItemBundle )
        {
            _next = next;
            _serviceHandleMap = serviceHandleMap;
            _exceptionPolicyItemBundle = policyItemBundle;
            _exceptionPolicy = new ExceptionPolicy<HttpContext, VOID>()
                        .Import( _exceptionPolicyItemBundle );
        }

        public async Task Invoke ( HttpContext context, IServiceProvider serviceProvider )
        {
            var requestPath = context.Request.Path.Value.TrimStart( '/' ).ToLower().Trim();

            if (_serviceHandleMap.Handlers.ContainsKey( requestPath ))
            {
                var method = _serviceHandleMap.Handlers[requestPath];

                var interfaceType = method.DeclaringType;

                //输出代码生成
                if(context.Request.Query.ContainsKey( _codegenParamName ))
                {
                    var genType = context.Request.Query[_codegenParamName].FirstOrDefault()?.ToLower();

                    var codeGenerator=  serviceProvider.GetServices< IApiCodeGenerator>().FirstOrDefault(t=>t.Languege.ToLower()==genType);

                    if (codeGenerator == null)
                    {
                        throw new NotSupportedException( $"This language is not supported : '{genType}'" );
                    }

                    var rspHtml = codeGenerator.Generate( method , context.Request.Query["package"].FirstOrDefault()??"api" );

                    await context.Response.WriteAsync( rspHtml );

                    return;
                }

                
                //获取实现类
                var impl = serviceProvider.GetService( interfaceType );

                if (impl != null)
                {
                    //在容错沙盒里运行
                    _exceptionPolicy.Execute( () =>
                    {

                        var parameters = context.Request.GetServiceParameters( method );

                        var result =method.Invoke( impl, parameters );

                        if (result != null)
                        {
                            var resultType = method.ReturnType;
                            if(result is Task)
                            {
                                //如果是Task<>类型，获取Task<>的result
                                if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof( Task<> ))
                                {
                                    var resultProperty = resultType.GetProperty( "Result" );

                                    result = resultProperty.GetValue( result );
                                }
                                else
                                {
                                    result = null;
                                }
                            }
                        }

                        //添加调用成功状态码
                        context.Response.Headers.Add( "stm_remote_statuscode", "0" );

                        context.Response.WriteAsync( result == null ? "" : JsonConvert.SerializeObject( result ) ).Wait();
                    }, context );


                    return;
                }
                else
                {
                    throw new NotSupportedException( $"Not implement service found of interface '{interfaceType}'" );
                }
            }



            // Call the next delegate/middleware in the pipeline
            await this._next( context );
        }
    }

    public static class StmHttpMicroServiceServerMiddlewareExtensions
    {
        public static IApplicationBuilder UseStmHttpMicroServiceServer(
            this IApplicationBuilder app , 
            ServiceHandleMap serviceHandleMap,
            ExceptionPolicyItemBundle<HttpContext, VOID> policyItemBundle )
        {
            app.UseMiddleware<StmHttpMicroServiceServerMiddleware>( serviceHandleMap, policyItemBundle );

            return app;
        }
    }

    public class StmHttpMicroServiceServerMiddlewareOptionBuilder
    {
        private ServiceHandleMap _serviceHandleMap;
        private HttpExceptionPolicy _exceptionPolicyItemBundle;
        private ExceptionPolicy<HttpContext, VOID> _exceptionPolicy;
        private string _codegenParamName = "gen";

        public StmHttpMicroServiceServerMiddlewareOptionBuilder ()
        {
            _exceptionPolicy = new ExceptionPolicy<HttpContext, VOID>();
        }

        public StmHttpMicroServiceServerMiddlewareOptionBuilder SetServiceHandleMap( ServiceHandleMap serviceHandleMap )
        {
            this._serviceHandleMap = serviceHandleMap;
            return this;
        }

        public StmHttpMicroServiceServerMiddlewareOptionBuilder SetHttpExceptionPolicy ( HttpExceptionPolicy httpExceptionPolicy )
        {
            this._exceptionPolicyItemBundle = httpExceptionPolicy;
            this._exceptionPolicy = new ExceptionPolicy<HttpContext, VOID>().Import( httpExceptionPolicy );
            return this;
        }

        public StmHttpMicroServiceServerMiddlewareOptionBuilder SetCodeGenParam ( string paramName )
        {
            this._codegenParamName = paramName;
            return this;
        }

    }

    public class HttpExceptionPolicy : ExceptionPolicyItemBundle<HttpContext, VOID> { }
}

