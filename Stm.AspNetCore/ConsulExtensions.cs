using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Stm.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Http;
using Stm.Core.SoaGovernance;

namespace Stm.AspNetCore
{
    public static class ConsulExtensions
    {
        // 服务注册
        public static IApplicationBuilder RegisterConsul ( this IApplicationBuilder app, IApplicationLifetime lifetime, IOptions<ServiceInfoRegisterConfig> consulCfg )
        {
            var consulClient = new ConsulClient( x => x.Address = new Uri( $"http://{consulCfg.Value.ServiceCenterHost}:{consulCfg.Value.ServiceCenterPort}" ) );//请求注册的 Consul 地址
            var httpCheck = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds( 5 ),//服务启动多久后注册
                Interval = TimeSpan.FromSeconds( 10 ),//健康检查时间间隔，或者称为心跳间隔
                HTTP = $"http://{consulCfg.Value.Host}:{consulCfg.Value.Port}{consulCfg.Value.CheckUrl}",//健康检查地址
                Timeout = TimeSpan.FromSeconds( consulCfg.Value.CheckInterval )
            };

            foreach(var service in consulCfg.Value.Services)
            {
                var registration = new AgentServiceRegistration()
                {
                    Checks = new[] { httpCheck },
                    ID = service + "-" + consulCfg.Value.Host + "-" + consulCfg.Value.Port,
                    Name = service,
                    Address = consulCfg.Value.Host,
                    Port = consulCfg.Value.Port,
                    Tags = consulCfg.Value.Tags
                };

                //服务启动时注册，内部实现其实就是使用 Consul API 进行注册（HttpClient发起）
                consulClient.Agent.ServiceRegister( registration ).Wait();

                lifetime.ApplicationStopping.Register( () =>
                {
                    consulClient.Agent.ServiceDeregister( registration.ID ).Wait();//服务停止时取消注册
                } );
            }


            app.Map( consulCfg.Value.CheckUrl, srv => srv.Run( async context =>
            {
                await context.Response.WriteAsync( "OK" );
            } ) );


            return app;
        }

    }

}
