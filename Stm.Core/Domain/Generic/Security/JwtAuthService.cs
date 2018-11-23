using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using Stm.Core.Db;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Options;

namespace Stm.Core.Domain.Generic
{
    /// <summary>
    /// 基于Jwt的鉴权服务
    /// </summary>
    public class JwtAuthService : IAuthService
    {
        private string _secretKey;

        public JwtAuthService ( IOptions<JwtAuthServiceOptions> options )
        {
            _secretKey = options.Value.SecretKey;
        }

        /// <summary>
        /// 申请操作token
        /// </summary>
        /// <param name="resourceDescriptor">欲操作资源描述</param>
        /// <param name="regToken">登记客户端token</param>
        /// <returns></returns>
        public async Task<string> RegisterAsync ( ResourceDescriptor resourceDescriptor, string regToken )
        {
            var claims = new Claim[]
                 {
                    new Claim("stm/auth/token",resourceDescriptor.ToString())
                 };
            var key = new SymmetricSecurityKey( Encoding.UTF8.GetBytes( _secretKey ) );
            var creds = new SigningCredentials( key, SecurityAlgorithms.HmacSha256 );

            var jwttoken = new JwtSecurityToken(
                null,
                null,
                claims,
                DateTime.Now,
                resourceDescriptor.GetExpireDt(),
                creds
                );

            var token= new JwtSecurityTokenHandler().WriteToken( jwttoken );

            return token;
        }

        /// <summary>
        /// 检测token是否可以对资源进行操作
        /// </summary>
        /// <param name="token"></param>
        /// <param name="resourceName">资源名称</param>
        /// <param name="action">操作</param>
        /// <returns></returns>
        public async Task<bool> IsValidAsync ( string token, string resourceName, string action )
        {
            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey( Encoding.UTF8.GetBytes( _secretKey ) ),
                ValidateIssuer = false,//是否验证Issuer
                ValidateAudience = false,//是否验证Audience
                ValidateLifetime = true,//是否验证失效时间

            };

            ClaimsPrincipal claimsPrincipal = null;

            try
            {
                SecurityToken jwtToken;// = new JwtSecurityTokenHandler().ReadJwtToken( token );

                claimsPrincipal = new JwtSecurityTokenHandler ().ValidateToken( token, tokenValidationParameters, out jwtToken );

            }catch(Exception e)
            {
                return false;
            }


            var resourceDescriptorStr = claimsPrincipal.Claims.FirstOrDefault( t => t.Type == "stm/auth/token" )?.Value;

            if (string.IsNullOrWhiteSpace( resourceDescriptorStr )) return false;

            var resourceDescriptor = ResourceDescriptor.FromString( resourceDescriptorStr );

            if (resourceDescriptor == null || resourceDescriptor.GetExpireDt() < DateTime.Now) return false;

            return resourceDescriptor.IsValid( resourceName, action );
        }
    }
}
