using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Stm.Core.Security;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;

namespace Stm.AspNetCore
{
    /// <summary>
    /// 身份信息Cookie方式持久化器
    /// </summary>
    public class ClaimsPrincipalCookiePersistor: IStmPrincipalPersistor
    {
        private IHttpContextAccessor _httpContextAccessor;

        private string _keyname;

        private string _secretKey;

        private int _expireMinutes;

        public ClaimsPrincipalCookiePersistor ( IHttpContextAccessor httpContextAccessor, IOptions<ClaimsPrincipalCookiePersistorOptions> options )
        {
            _httpContextAccessor = httpContextAccessor;
            _keyname = options.Value.KeyName;
            _secretKey = options.Value.SecretKey;
            _expireMinutes = options.Value.ExpireMinutes;
        }

        public StmPrincipal RestorePrincipal ()
        {
            var ticket = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace( ticket ))
            {
                ticket = _httpContextAccessor.HttpContext.Request.Cookies[_keyname];
            }
            if (string.IsNullOrWhiteSpace( ticket )
                && _httpContextAccessor.HttpContext.Request.Method == HttpMethods.Post 
                && (_httpContextAccessor.HttpContext.Request.ContentLength ?? 0) > 0
                && _httpContextAccessor.HttpContext.Request.Form!=null
                && _httpContextAccessor.HttpContext.Request.Form.Count>0)
            {
                ticket = _httpContextAccessor.HttpContext.Request.Form[_keyname];
            }
            if (string.IsNullOrWhiteSpace( ticket ))
            {
                ticket = _httpContextAccessor.HttpContext.Request.Query[_keyname];
            }

            if (string.IsNullOrWhiteSpace( ticket )) return null;

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

                claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken( ticket, tokenValidationParameters, out jwtToken );

            }
            catch (Exception e)
            {
                return null;
            }

            return new StmPrincipal( claimsPrincipal );
        }

        public void SavePrincipal ( StmPrincipal principal )
        {
            var key = new SymmetricSecurityKey( Encoding.UTF8.GetBytes( _secretKey ) );
            var creds = new SigningCredentials( key, SecurityAlgorithms.HmacSha256 );

            var jwttoken = new JwtSecurityToken(
                null,
                null,
                principal.Claims,
                DateTime.Now,
                DateTime.Now.AddMinutes( _expireMinutes),
                creds
                );

            var token = new JwtSecurityTokenHandler().WriteToken( jwttoken );

            _httpContextAccessor.HttpContext.Response.Cookies.Append( _keyname, token, new CookieOptions {
                HttpOnly = true 
                //,IsEssential = true
            } );
        }
    }
}
