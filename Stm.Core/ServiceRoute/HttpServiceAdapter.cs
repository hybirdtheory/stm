using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Stm.Core.SoaGovernance;

namespace Stm.Core
{
    public class HttpServiceAdapter<T> : ServiceAdapterBase
    {
        private Type _serviceContractType;
        private String _serviceUrl;


        public HttpServiceAdapter ( string serviceUrl )
        {
            this._serviceContractType = typeof( T );
            if (!this._serviceContractType.IsInterface)
            {
                throw new Exception( $"type {this._serviceContractType} is not a interface" );
            }
            this._serviceUrl = serviceUrl;
        }

        public override Type ServiceContractType => _serviceContractType;

        public override string Protocol => _serviceUrl.StartsWith("https")?ServiceCallProtocols.SSL: ServiceCallProtocols.HTTP;

        public override async Task<object> ExecuteServiceAsync ( IServiceProvider serviceProvider, MethodInfo methodInfo, params object[] parameters)
        {
            var serviceUrl = _serviceUrl;

            if (serviceUrl.Contains( "{lb:" ))
            {
                var _microServiceLocator = serviceProvider.GetService<IMicroServiceLocator>();

                if (_microServiceLocator != null)
                {
                    //从服务发现获取的服务信息填充url
                    //http://{lb:idservice}/{service}/{action} => http://xxx.xxx.xxx:80/{service}/{action} 
                    serviceUrl = System.Text.RegularExpressions.Regex.Replace( _serviceUrl, @"\{lb\:([\w\d_\-\.]+?)\}", match =>
                         {
                             var serviceName = match.Groups[1].Value;
                             var microServiceInfo = _microServiceLocator.FindService( serviceName );

                             if (microServiceInfo == null)
                             {
                                 throw new BaseException( $"service {serviceName} not found ", StandradErrorCodes.NoServiceFound ).Tag( serviceName );
                             }

                             return microServiceInfo.Host + ":" + microServiceInfo.Port;
                         } );
                }
            }

            //替换service
            var service = _serviceContractType.Name;
            if (service.StartsWith( "I" )) service = service.Substring( 1, service.Length - 1 );
            if (service.EndsWith( "Service" )) service = service.Substring( 0, service.Length - 7 );
            if (_serviceContractType.GetCustomAttribute<NameAttribute>() != null)
            {
                service = _serviceContractType.GetCustomAttribute<NameAttribute>().Value;
            }
            serviceUrl = serviceUrl.Replace( "{service}", service );

            //替换action
            var actionName = methodInfo.Name;
            if (methodInfo.GetCustomAttribute<NameAttribute>() != null)
            {
                actionName = methodInfo.GetCustomAttribute<NameAttribute>().Value;
            }
            serviceUrl = serviceUrl.Replace( "{action}", actionName );

            HttpClient httpClient = new HttpClient();
            httpClient.MaxResponseContentBufferSize = 256000;
            httpClient.DefaultRequestHeaders.Add( "user-agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143 Safari/537.36" );

            var methodParameters = methodInfo.GetParameters();

            //是否包含文件
            var hasFile = methodParameters.Any( t => t.ParameterType == typeof( byte[] ) );

            HttpContent content = null;

            if (hasFile)
            {
                MultipartFormDataContent mulContent = new MultipartFormDataContent( "----WebKitFormBoundaryrXRBKlhEeCbfHIY" );
                for (int i = 0; i < methodParameters.Length; i++)
                {
                    var pi = methodParameters[i];
                    mulContent.Add( new StringContent( ServiceJsonConvert.SerializeObject( parameters[i] ) ), pi.Name );
                   
                }
                content = mulContent;
            }
            else
            {
                //设置post参数
                RemoteServiceParameterCollection requestParameters = new RemoteServiceParameterCollection();
                for (int i = 0; i < methodParameters.Length; i++)
                {
                    var pi = methodParameters[i];
                    requestParameters.Add( pi.Name, ServiceJsonConvert.SerializeObject( parameters[i] ) );
                }
                content = new FormUrlEncodedContent( requestParameters );
            }

            

            //发送请求
            HttpResponseMessage httpResponse = await httpClient.PostAsync( serviceUrl, content );
            var statuscode = (int)httpResponse.StatusCode;
            var statusname = httpResponse.StatusCode.ToString();
            String responseResult = await httpResponse.Content.ReadAsStringAsync();

            //如果是预期异常
            if (!httpResponse.IsSuccessStatusCode&&httpResponse.Headers.Contains( "stm_remote_statuscode" ))
            {
                if (!string.IsNullOrWhiteSpace( responseResult ))
                {
                    var jsonRsp = responseResult.Trim();
                    if (jsonRsp.StartsWith( "{" ) && jsonRsp.EndsWith( "}" )&&jsonRsp.Contains( "stm_remote_statuscode"))
                    {
                        var rspData = JsonConvert.DeserializeObject<Dictionary<String, String>>( jsonRsp );
                        int remoteStatusCode = int.Parse( rspData["stm_remote_statuscode"]);
                        string remoteFailMessage = rspData[ "stm_remote_message" ];
                        string remoteFailStackTrace = rspData[ "stm_remote_stacktrace" ];

                        throw new BaseException( remoteFailMessage, remoteStatusCode ).Tag( "{stacktrace:\"" + JsonConvert.SerializeObject( remoteFailStackTrace ) + "\"}" );
                    }
                }

            }

            //远程服务报错
            if (httpResponse.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                var tag = "{statuscode:" + statuscode + ",statusname:\"" + statusname + "\",response:\"" + JsonConvert.SerializeObject( responseResult ) + "\"}";
                throw new BaseException( $"service {serviceUrl} error", StandradErrorCodes.ServiceError ).Tag( tag );
            }

            //远程服务运行超时
            if (httpResponse.StatusCode== System.Net.HttpStatusCode.GatewayTimeout||
               httpResponse.StatusCode== System.Net.HttpStatusCode.RequestTimeout)
            {
                var tag = "{statuscode:" + statuscode + ",statusname:\"" + statusname + "\"}";
                throw new BaseException( $"service {serviceUrl} timeout", StandradErrorCodes.ServiceTimeout ).Tag( tag );
            }

            //其他错误
            if (!httpResponse.IsSuccessStatusCode)
            {
                var tag = "{statuscode:" + statuscode + ",statusname:\"" + statusname + "\"}";
                throw new BaseException( $"service {serviceUrl} Unreachable", StandradErrorCodes.ServiceUnreachable ).Tag( tag );
            }

            //如果没返回stm_remote_statuscode 头，认为是访问到了错误的页面
            if(!httpResponse.Headers.Contains( "stm_remote_statuscode" ))
            {
                var tag = "{statuscode:" + statuscode + ",statusname:\"" + statusname + "\",response:\"" + JsonConvert.SerializeObject( responseResult ) + "\"}";
                throw new BaseException( $"service {serviceUrl} Unreachable", StandradErrorCodes.ServiceUnreachable ).Tag( tag );
            }
            
            //如果没返回stm_remote_statuscode 头!=0，认为是服务端程序有bug
            if (!httpResponse.Headers.Contains( "stm_remote_statuscode" ))
            {
                var tag = "{statuscode:" + statuscode + ",statusname:\"" + statusname + "\",response:\"" + JsonConvert.SerializeObject( responseResult ) + "\"}";
                throw new BaseException( $"service {serviceUrl} error,stm_remote_statuscode isn`t '0',but Http StatusCode is 200", StandradErrorCodes.ServiceError ).Tag( tag );
            }

            //如果远程服务返回成功，并且服务返回类型是void类型，直接返回null
            if (methodInfo.ReturnType == typeof( void ))
            {
                return null;
            }
            if (methodInfo.ReturnType == typeof( Task ))
            {
                return Task.CompletedTask;
            }

            //如果是Task<>类型，获取Task<>类型的类型参数
            bool isGenericTypeTask=false;
            var resultType = methodInfo.ReturnType;
            if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof( Task<> ))
            {
                resultType = resultType.GetGenericArguments()[0];
                isGenericTypeTask = true;
            }

            //远程服务返回数据
            Object response = null;
            try
            {
                response = JsonConvert.DeserializeObject( responseResult, resultType );
            }
            catch(Exception e)
            {
                String tag = "{returnType:\""+ resultType.FullName+"\",responseData:\""+ JsonConvert.SerializeObject( responseResult ) + "\"}";
                throw new BaseException( $"service {serviceUrl} result format error", StandradErrorCodes.ServiceResultFormatError, e ).Tag( tag );
            }

            //如果返回类型是Task<>,动态构造返回结果：Task<?>.FromResult<?>( response)
            if (isGenericTypeTask)
            {
                var methodFromResult = typeof( Task).GetMethod( "FromResult", BindingFlags.Static|BindingFlags.Public );

                var genericMethod = methodFromResult.MakeGenericMethod( resultType );

                response = genericMethod.Invoke( null,new object[] { response } );
            }
            
            return response;
        }
    }
}
