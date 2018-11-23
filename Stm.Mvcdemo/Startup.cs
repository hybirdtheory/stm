using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Stm.Core;
using Stm.Core.Db;
using Stm.AspNetCore;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Stm.Core.Consul;
using Stm.Core.Domain.Generic;
using Stm.Core.SoaGovernance;
using log4net;
using Stm.Core.Interceptors;
using System.Security.Claims;
using Stm.Core.Aop;
using Stm.Core.Security;
using Stm.Core.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Stm.Mvcdemo
{
    public class Startup
    {
        public Startup ( IConfiguration configuration )
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices ( IServiceCollection services )
        {
            //1. web基础配置
            services.AddOptions();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUserIpAccessor, WebUserIpAccessor>();
            services.Configure<CookiePolicyOptions>( options =>
            {
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            } );
            services.AddMvc( options =>
            {
                options.Filters.Add<StmMvcFilter>();
            } ).SetCompatibilityVersion( CompatibilityVersion.Version_2_1 );

            //数据库配置
            services.AddDbContext<CommonDb>( options =>
            {
                options.UseSqlServer( Configuration.GetConnectionString( "common" ) );
            } );
            services.AddDbContextFactory( options =>
            {
                options.Register( "common", serviceProvider => serviceProvider.GetRequiredService<CommonDb>() );
            } );


            //订单分库
            services.AddShardingDbcontext<OrderShardingDbcontext>( t =>
            {
                t.UseConfigShardingConnectionStrings( Configuration, "order" );
                t.InnerDbContextOptionsDelegte = ( connstring, builder ) => builder.UseSqlServer( connstring );
            } );

            //审计日志
            services.AddDbbasedAuditLogService( options =>
            {
                options.DbName = "common";
            } );
            services.AddAuditInterceptor( options =>
            {
                options.AddPredicate( MethodMatchPredicates.ForMethod( "About$" ) );
            } );


            //权限验证拦截器
            services.AddAuthorizeInterceptor( options =>
                  options.AddAuthorizeItem(
                            new AuthorizeItem( AuthorizeRule.Deny, "无权限进行此操作" )
                            .AddAuthPredicate( ( user, method ) => user.IsAllowExcute( method ) )
                            .AddMethodMatchPredicate( MethodMatchPredicates.ForAuthAttributeMethod() ) )
            );
            services.AddScoped<IAuthenticator, Authenticator>();

            ///cookie存储用户信息
            services.AddClaimsPrincipalCookiePersistor( options => options.SecretKey = Configuration["JwtSecretKey"] );

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure ( IApplicationBuilder app, IHostingEnvironment env )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler( "/Home/Error" );
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc( routes =>
             {
                 routes.MapRoute(
                     name: "default",
                     template: "{controller=Home}/{action=Index}/{id?}" );
             } );
        }
    }


    public class Authenticator : IAuthenticator
    {
        private IStmPrincipalPersistor _stmPrincipalPersistor;

        public Authenticator ( IStmPrincipalPersistor stmPrincipalPersistor )
        {
            _stmPrincipalPersistor = stmPrincipalPersistor;
        }

        public StmPrincipal GetCurrentUser ()
        {
            return _stmPrincipalPersistor.RestorePrincipal();
        }
    }
}
