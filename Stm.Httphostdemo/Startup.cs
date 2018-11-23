using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Stm.AspNetCore;
using Stm.Core;
using Stm.Core.CodeGeneration;
using Stm.Httphostdemo.Contracts;
using Stm.Httphostdemo.Services;
using Stm.Core.Db;
using Microsoft.EntityFrameworkCore;
using Stm.Core.Domain.Generic;
using System.Net;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Stm.Core.Consul;
using Stm.Core.SoaGovernance;
using Microsoft.Extensions.Logging;

namespace Stm.Httphostdemo
{
    public class Startup
    {
        public Startup ( IConfiguration configuration )
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices ( IServiceCollection services )
        {
            //web基础配置
            services.AddOptions();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUserIpAccessor, WebUserIpAccessor>();

            #region  单应用模式

            //数据库配置
            //services.AddDbContext<CommonDb>( options =>
            //{
            //    options.UseSqlServer( Configuration.GetConnectionString( "common" ) );
            //} );
            //services.AddDbContextFactory( options =>
            //{
            //    options.Register( "common", serviceProvider => serviceProvider.GetRequiredService<CommonDb>() );
            //} );

            ////系统配置服务
            //services.AddDbbasedConfigService( t => t.DbName = "common" );
            ////id生成服务
            //services.AddSnowflakeIdService();
            ////本地文件服务
            //services.AddLocalStorageFileService( AppContext.BaseDirectory + "\\upfiles\\", "http://localhost:10001/upfiles/" );
            ////鉴权服务
            //services.AddJwtAuthService( Configuration["JwtSecretKey"] );

            #endregion

            #region 微服务模式
            //服务注册信息
            services.AddServiceRegInfo( Configuration.GetSection( "ServiceInfo" ) );
            //使用consul服务治理
            services.AddConsulMicroServiceLocator();

            //系统配置服务
            services.AddSysConfigServiceHttpClient();

            //id生成服务
            services.AddIdServiceHttpClient();

            //文件服务
            services.AddResServiceHttpClient();

            //鉴权服务
            services.AddAuthServiceHttpClient();

            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure ( 
            IApplicationBuilder app, 
            IHostingEnvironment env, 
            IApplicationLifetime lifetime, 
            ILoggerFactory loggerFactory, 
            IOptions<ServiceInfoRegisterConfig> consulCfg )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //服务注册
            app.RegisterConsul( lifetime, consulCfg );

            //添加日志
            loggerFactory.AddLog4Net();

            //系统配置同步服务
            app.UseSysConfigService( lifetime, new List<string> { "appversion" } );
           
            app.Map( "/appversion", ap => ap.Run( async context =>
            {
                int appversion = context.RequestServices.GetService<ISysConfigClient>().GetData<int>( "appversion" );
                await context.Response.WriteAsync( appversion + "" );
            } ) );

            app.Map( "/getid", ap => ap.Run( async context =>
            {
                long appversion = context.RequestServices.GetService<INumberIdService>().GetId( "default" );
                await context.Response.WriteAsync( appversion + "" );
            } ) );

            app.Map( "/upfile", ap => ap.Run( async context =>
            {
                var resService = context.RequestServices.GetService<IResService>();

                var path = resService.CreateRes( System.IO.File.ReadAllBytes( "1.png" ), "image/png" );

                var url = resService.GetResUrl( path );

                await context.Response.WriteAsync( url );
            } ) );

            app.Map( "/getauthtoken", ap => ap.Run( async context =>
            {
                var resourceDescriptor = new ResourceDescriptor().Add( new ResourceItemDescriptor( "service:fileservice", "create,modify" ) );

                var authService = context.RequestServices.GetService<IAuthService>();

                string token =await authService.RegisterAsync( resourceDescriptor,null );

                var isvalid1 = await authService.IsValidAsync( token, "service:fileservice", "create" );
                var isvalid2 = await authService.IsValidAsync( token, "service:fileservice", "delete" );

                
                
                await context.Response.WriteAsync(  token + ","+ isvalid1+","+ isvalid2 );
            } ) );
        }
    }


   
}
