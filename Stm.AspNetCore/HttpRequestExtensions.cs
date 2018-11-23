using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Stm.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json;

namespace Stm.AspNetCore
{
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// 获得当前页面客户端的IP
        /// </summary>
        /// <returns>当前页面客户端的IP</returns>
        public static string GetIP ( this HttpRequest request )
        {
            return request.HttpContext.Connection.RemoteIpAddress.ToString(); ;
        }

        public static bool IsAjax ( this HttpRequest req )
        {
            bool result = false;

            var xreq = req.Headers.ContainsKey( "x-requested-with" );
            if (xreq)
            {
                result = req.Headers["x-requested-with"] == "XMLHttpRequest";
            }

            return result;
        }

        public static object[] GetServiceParameters(this HttpRequest httpRequest,MethodInfo methodInfo )
        {
            var declareParameters = methodInfo.GetParameters();

            if (declareParameters.Length == 0) return null;

            object[] result = new object[declareParameters.Length];

            var valueCollection = new List<KeyValuePair<string, StringValues>>();

            if(httpRequest.Query!=null&& httpRequest.Query.Any())  valueCollection.AddRange( httpRequest.Query );
            if(httpRequest.Method==HttpMethods.Post&&(httpRequest.ContentLength??0)>0)valueCollection.AddRange( httpRequest.Form );

            for (int i = 0; i < declareParameters.Length ; i++)
            {
                var param = declareParameters[i];

                Type parameterType = param.ParameterType;
                String paramName = param.Name;
                
                String paramValue = null;
                //序号参数查找
                if (valueCollection.Any(t=>t.Key== ":" + i ))
                {
                    paramValue = valueCollection.First(t=>t.Key==":" + i).Value.First();
                }
                //按名称查找
                else if (valueCollection.Any(t=>t.Key== paramName ))
                {
                    paramValue = valueCollection.First( t => t.Key == paramName ).Value.First();
                }


                if (string.IsNullOrWhiteSpace( paramValue ))
                {
                    if (parameterType.IsClass)
                    {
                        result[i] = null;
                    }
                    else
                    {
                        result[i] = Activator.CreateInstance( parameterType );
                    }
                    continue;
                }

                //判断是否是json
                if ((         (paramValue.StartsWith( "{" ) && paramValue.EndsWith( "}" )) ||
                              (paramValue.StartsWith( "[" ) && paramValue.EndsWith( "]" ))||
                              parameterType == typeof( byte[] )
                     ) &&       parameterType != typeof( String ))
                {
                    try
                    {
                        var value = JsonConvert.DeserializeObject( paramValue, parameterType );

                        result[i] = value;
                    }catch(JsonReaderException jsonReaderExeption)
                    {
                        throw new System.FormatException( $"{jsonReaderExeption.Message} on param '{param.Name}' of action {methodInfo.DeclaringType.Name}/{methodInfo.Name}" );
                    }
                }
                else if (typeof( IServiceDto ).IsAssignableFrom(parameterType))
                {
                    try
                    {
                        var obj = Activator.CreateInstance( parameterType );
                        ((IServiceDto)obj).FromDtoString( paramValue );
                        var value = obj;

                        result[i] = value;
                    }
                    catch (Exception)
                    {
                        throw new System.FormatException( $"value '{paramValue}' was not recognized as a valid {parameterType.Name} on param '{param.Name}' of action {methodInfo.DeclaringType.Name}/{methodInfo.Name}" );
                    }
                }
                else
                {
                    try
                    {
                        var value = Convert.ChangeType( paramValue, parameterType );

                        result[i] = value;
                    }catch(System.FormatException)
                    {
                        throw new System.FormatException( $"value '{paramValue}' was not recognized as a valid {parameterType.Name} on param '{param.Name}' of action {methodInfo.DeclaringType.Name}/{methodInfo.Name}" );
                    }
                }
            }

            return result;
        }
    }
}
