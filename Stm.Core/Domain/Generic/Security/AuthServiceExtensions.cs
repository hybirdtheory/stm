using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Stm.Core.Domain.Generic;
using Microsoft.Extensions.Configuration;

namespace Stm.Core.Domain.Generic
{
    public static class AuthServiceExtensions
    {
        /// <summary>
        /// Jwt鉴权服务服务
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="secretKey">加密key</param>
        public static void AddJwtAuthService ( this IServiceCollection serviceCollection,string secretKey )
        {
            Action<JwtAuthServiceOptions> options = opt =>
            {
                opt.SecretKey = secretKey;
            };
            serviceCollection.Configure( options );

            serviceCollection.AddSingleton<IAuthService, JwtAuthService>();
        }

        public static void AddJwtAuthServiceSerever ( this IServiceCollection serviceCollection, string secretKey )
        {
            Action<JwtAuthServiceOptions> options = opt =>
            {
                opt.SecretKey = secretKey;
            };
            serviceCollection.Configure( options );

            serviceCollection.AddSingleton<IAuthService, JwtAuthService>();
        }

        public static void AddAuthServiceHttpClient ( this IServiceCollection serviceCollection )
        {
            serviceCollection.AddHttpService<IAuthService>( "http://{lb:authservice}/{service}/{action}" );

        }
    }
}
