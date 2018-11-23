using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Stm.Core.Aop;
using Stm.Core.Domain.Generic;
using Stm.Core.Security;
using System.Linq;
using Microsoft.Extensions.Options;

namespace Stm.Core.Interceptors.Audit
{
    public class AuditInterceptor : Aop.IInterceptor
    {
        private IAuditLogService _auditLogService;
        private IAuthenticator _authenticator;
        private List<AspectPredicate> _predicates;
        private IUserIpAccessor _userIpAccessor;

        public AuditInterceptor( 
            IAuditLogService auditLogService ,
            IAuthenticator authenticator,
            IUserIpAccessor userIpAccessor,
            IOptions<AuditInterceptorOptions> options)
        {
            _auditLogService = auditLogService;
            _authenticator = authenticator;
            _userIpAccessor = userIpAccessor;
            _predicates = options.Value.Predicates ?? new List<AspectPredicate>();
        }

        public void OnException ( Aop.AspectContext aspectContext )
        {
        }

        public void PostProceed ( Aop.AspectContext aspectContext )
        {
            if (!isMatch( aspectContext.Method.Method )) return ;

            var user = _authenticator.GetCurrentUser();

            var auditContent = aspectContext.Method.Method.DeclaringType.FullName + "." + aspectContext.Method.Method.Name + "()";
            var auditAttr = aspectContext.Method.Method.GetCustomAttribute<AuditAttribute>();
            if (auditAttr != null)
            {
                auditContent = auditAttr.LogContentFormat;
            }


            auditContent = System.Text.RegularExpressions.Regex.Replace( auditContent, @"@\{(.*?)\}", match =>
            {
                var paramter = match.Groups[1].Captures[0].Value;

                paramter = paramter.Trim();

                var value = aspectContext.Method.Parameters?.FirstOrDefault( t => t.Name == paramter )?.Value;

                if (value == null)
                {
                    return "";
                }

                return ServiceJsonConvert.SerializeObject( value );

            } );


            _auditLogService.WriteAuditLogAsync( user, auditContent,new AuditAdditional { Ip= _userIpAccessor.GetIp() } ).Wait();

        }

        public bool PreProceed ( Aop.AspectContext aspectContext )
        {
            return true;
        }
        private bool isMatch ( MethodInfo methodInfo )
        {
            return _predicates.Any( t => t( methodInfo ) );
        }
    }
}
