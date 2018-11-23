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
using Stm.ServiceApidemo.Dbcontexts;
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

namespace Stm.ServiceApidemo
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

            //数据库配置
            services.AddDbContext<CommonDb>( options =>
            {
                options.UseSqlServer( Configuration.GetConnectionString( "common" ) );
            } );
            services.AddDbContextFactory( options =>
            {
                options.Register( "common", serviceProvider => serviceProvider.GetRequiredService<CommonDb>() );
            } );

            services.AddScoped<IMethodExcuteTraceRepository, MethodExcuteTraceLogger>();
            services.AddScoped<IAuthenticator, Authenticator>();
            services.AddMethodExcuteTraceInterceptor( options => {
                options.AddPredicate( MethodMatchPredicates.ForService( "Service$" ) );
            });

            //权限验证拦截器
            services.AddAuthorizeInterceptor( options =>
                  options.AddAuthorizeItem(
                            new AuthorizeItem( AuthorizeRule.Deny, "无权限进行此操作" )
                            .AddAuthPredicate( (user,method) => user.IsAllowExcute(method) )
                            .AddMethodMatchPredicate( MethodMatchPredicates.ForAuthAttributeMethod() ) )
            );

            services.AddDbbasedAuditLogService( options =>
            {
                options.DbName = "common";
            } );
            services.AddAuditInterceptor( options =>
            {
                options.AddPredicate( MethodMatchPredicates.ForService( "Service$" ) );
            } );

            //基础功能
            services.AddServiceRegInfo( Configuration.GetSection( "ServiceInfo" ) );
            services.AddConsulMicroServiceLocator();

            //系统服务
            services.AddSysConfigServiceServer( t => t.DbName = "common" );
            //services.AddSnowflakeIdServiceServer();
            services.AddJwtAuthServiceSerever( Configuration["JwtSecretKey"] );
            services.AddLocalStorageFileServiceServer( AppContext.BaseDirectory + "\\upfiles\\", "http://localhost:10001/upfiles/" );

            new BusManagedServiceCollection()
                .AddLocalScoped<INumberIdService, SnowflakeIdService>()
                .RegisterTo( services );

            ///cookie存储用户信息
            services.AddClaimsPrincipalCookiePersistor( options => options.SecretKey = Configuration["JwtSecretKey"] );

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure ( IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime, ILoggerFactory loggerFactory, IOptions<ServiceInfoRegisterConfig> consulCfg )
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //服务注册
            app.RegisterConsul( lifetime, consulCfg );

            //添加日志
            loggerFactory.AddLog4Net();

            //配置微服务服务端
            app.UseStmHttpMicroServiceServer(
                new ServiceHandleMap()
                    .AddHandle<ISysConfigService>()
                    .AddHandle<INumberIdService>()
                    .AddHandle<IResService>()
                    .AddHandle<IAuthService>(),

                new HttpExceptionPolicy()
                //把其他异常转换成BaseException
                .TransException<FormatException>( 101 )
                //只接受列表里的错误，并把错误码发给客户端
                .HandleBaseException(
                    new int[] {
                        1,  //一般性错误，可直接提示用户
                        12, //权限不足
                        13,
                        14,
                        15,
                        14,
                        17,
                        18,
                        19,
                        20,
                        21
                    }, 
                    ctx =>
                    {
                        var exp = ctx.Exception.GetExceptionOfType<BaseException>();

                        var rsp = new
                        {
                            stm_remote_statuscode = exp.Code,
                            stm_remote_message = exp.Message
                        };
                        ctx.Environment.Response.StatusCode = 500;
                        ctx.Environment.Response.ContentType = "application/json; charset=utf-8";
                        ctx.Environment.Response.Headers.Add( "stm_remote_statuscode", exp.Code.ToString() );
                        ctx.Environment.Response.WriteAsync( JsonConvert.SerializeObject( rsp ) ).Wait();
                        ctx.ExceptionIsHandled = true;
                    }, true )
                //其余异常全部返回99错误码
                .Handle( ctx => true, ctx =>
                {

                    var exp = ctx.Exception;

                    var rsp = new
                    {
                        stm_remote_statuscode = StandradErrorCodes.UnkonwError,
                        stm_remote_message = exp.Message,
                        stm_remote_stacktrace = exp.StackTrace
                    };
                    ctx.Environment.Response.StatusCode = 500;
                    ctx.Environment.Response.ContentType = "application/json; charset=utf-8";
                    ctx.Environment.Response.Headers.Add( "stm_remote_statuscode", StandradErrorCodes.UnkonwError.ToString() );
                    ctx.Environment.Response.WriteAsync( JsonConvert.SerializeObject( rsp ) ).Wait();
                    ctx.ExceptionIsHandled = true;
                }, true )
             );


            app.Map( "/setp", ap => ap.Run( async context =>
            {
                StmPrincipal principal = new StmPrincipal();
                ClaimsIdentity identity = new ClaimsIdentity();
                identity.AddClaim( new Claim( Core.Security.ClaimTypes.Permissions, "GetId" ) );
                principal.AddIdentity( identity );

                var stmPrincipalPersistor = context.RequestServices.GetService<IStmPrincipalPersistor>();

                stmPrincipalPersistor.SavePrincipal( principal );

                await context.Response.WriteAsync( "已授权getid" );
            } ) );

            //其他流量
            app.Run( async httpcontext =>
            {
                var db = httpcontext.RequestServices.GetService<CommonDb>();
                //var manager = ((Microsoft.EntityFrameworkCore.Internal.IDbContextDependencies)db).StateManager;
                await httpcontext.Response.WriteAsync( "ST API SERVICE" );
            } );

        }
    }


    public class Authenticator : IAuthenticator
    {
        private IStmPrincipalPersistor _stmPrincipalPersistor;

        public Authenticator( IStmPrincipalPersistor stmPrincipalPersistor )
        {
            _stmPrincipalPersistor = stmPrincipalPersistor;
        }

        public StmPrincipal GetCurrentUser ()
        {
            return _stmPrincipalPersistor.RestorePrincipal();
        }
    }
}
