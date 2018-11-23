using Castle.DynamicProxy;
using Microsoft.Extensions.Options;
using Stm.Core.Aop;
using Stm.Core.Domain.Generic;
using Stm.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Stm.Core.Interceptors
{
    /// <summary>
    /// 鉴权验证项
    /// </summary>
    public class AuthorizeItem
    {
        /// <summary>
        /// 方法匹配谓词
        /// </summary>
        private readonly List<AspectPredicate> _methodMatchPredicates = new List<AspectPredicate>();

        /// <summary>
        /// 执行规则（允许执行/拒绝执行）
        /// 如果为允许执行：满足鉴权谓词里任意条件放行,否则抛错
        /// 如果为拒绝执行：满足鉴权谓词里任意条件抛错,否则放行
        /// </summary>
        private AuthorizeRule _rule = AuthorizeRule.Allow;

        /// <summary>
        /// 鉴权谓词
        /// </summary>
        private readonly List<AuthorizePredicate> _authPredicates = new List<AuthorizePredicate>();

        /// <summary>
        /// 无权限时的错误提醒信息
        /// </summary>
        private string _errorMessage = "权限不足";


        public string ErrorMessage { get => _errorMessage; }

        public List<AspectPredicate> MethodMatchPredicates { get => _methodMatchPredicates; }

        /// <summary>
        /// 添加鉴权谓词
        /// </summary>
        /// <param name="predicates"></param>
        /// <returns></returns>
        public AuthorizeItem AddAuthPredicate(params AuthorizePredicate[] predicates)
        {
            _authPredicates.AddRange(predicates);

            return this;
        }

        /// <summary>
        /// 方法匹配谓词
        /// </summary>
        /// <param name="predicates"></param>
        /// <returns></returns>
        public AuthorizeItem AddMethodMatchPredicate(params AspectPredicate[] predicates)
        {
            _methodMatchPredicates.AddRange(predicates);

            return this;
        }

        public AuthorizeItem(AuthorizeRule rule)
        {
            _rule = rule;
        }

        public AuthorizeItem(AuthorizeRule rule,string errormsg)
        {
            _errorMessage = errormsg;
        }

        /// <summary>
        /// 此项是否能用于此方法
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public bool IsMatch(MethodInfo method)
        {
            return _methodMatchPredicates.Any(p => p(method));
        }

        /// <summary>
        /// 用户是否有权限
        /// </summary>
        /// <param name="userIdentity"></param>
        /// <returns></returns>
        public bool IsValid( StmPrincipal userIdentity,MethodInfo method )
        {
            return (_rule== AuthorizeRule.Allow&& _authPredicates.Any(p => p(userIdentity, method ) ))||
                   (_rule == AuthorizeRule.Deny && _authPredicates.All(p => !p(userIdentity, method ) ));
        }
    }

    /// <summary>
    /// 鉴权拦截器
    /// </summary>
    public class AuthorizeInterceptor : Castle.DynamicProxy.IInterceptor, Aop.IInterceptor
    {
        /// <summary>
        /// 验证谓词
        /// </summary>
        private readonly List<AuthorizePredicate> _predicates = new List<AuthorizePredicate>();

        /// <summary>
        /// 鉴权项
        /// </summary>
        private readonly List<AuthorizeItem> _authorizeItems = new List<AuthorizeItem>();

        /// <summary>
        /// 鉴权器
        /// </summary>
        private IAuthenticator _authenticator;

        public AuthorizeInterceptor(IAuthenticator authenticator, IOptions<AuthorizeInterceptorOptions> options)
        {
            _authenticator = authenticator;
            _authorizeItems.AddRange(options.Value.Items);
            //Order = options.Value.Order;
        }

        public void Intercept ( IInvocation invocation )
        {
            if (_authenticator == null) throw new Exception( "_authenticator is null" );

            var user = _authenticator.GetCurrentUser();

            foreach (var authitem in _authorizeItems)
            {
                //匹配接口方法和实现方法
                if (authitem.IsMatch( invocation.Method ) )
                {
                    if (!authitem.IsValid( user , invocation.Method ))
                    {
                        throw new PermissionDeniedException( authitem.ErrorMessage );
                    }
                }
            }

            invocation.Proceed();
        }

        public void OnException ( Aop.AspectContext aspectContext )
        {
        }

        public void PostProceed ( Aop.AspectContext aspectContext )
        {
        }

        public bool PreProceed ( Aop.AspectContext aspectContext )
        {
            if (_authenticator == null) throw new Exception( "_authenticator is null" );

            var user = _authenticator.GetCurrentUser();

            foreach (var authitem in _authorizeItems)
            {
                //匹配接口方法和实现方法
                if (authitem.IsMatch( aspectContext.Method.Method ))
                {
                    if (!authitem.IsValid( user, aspectContext.Method.Method ))
                    {
                        throw new PermissionDeniedException( authitem.ErrorMessage );
                    }
                }
            }

            return true;
        }
    }

    
}
