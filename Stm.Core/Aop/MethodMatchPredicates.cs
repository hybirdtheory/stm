using Stm.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Stm.Core.Aop
{
    public static class MethodMatchPredicates
    {
        public static AspectPredicate ForNameSpace ( string nameSpace )
        {
            if (nameSpace == null)
            {
                throw new ArgumentNullException( "nameSpace" );
            }
            return ( MethodInfo method ) => method.DeclaringType.Namespace.Matches( nameSpace );
        }

        public static AspectPredicate ForService ( string service )
        {
            if (service == null)
            {
                throw new ArgumentNullException( "service" );
            }
            return delegate ( MethodInfo method )
            {
                if (method.DeclaringType.Name.Matches( service ))
                {
                    return true;
                }
                Type declaringType = method.DeclaringType;
                return (declaringType.FullName ?? $"{declaringType.Name}.{declaringType.Name}").Matches( service );
            };
        }

        public static AspectPredicate ForMethod ( string method )
        {
            if (method == null)
            {
                throw new ArgumentNullException( "method" );
            }
            return ( MethodInfo methodInfo ) => methodInfo.Name.Matches( method );
        }

        /// <summary>
        /// 验证方法权限
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static AspectPredicate ForAuthAttributeMethod ()
        {
            return ( MethodInfo methodInfo ) =>
                methodInfo.GetCustomAttributes<PermissionRequiredAttribute>( true ).Any()
                || methodInfo.GetCustomAttributes<RoleRequiredAttribute>( true ).Any();
        }

        public static AspectPredicate ForMethod ( string service, string method )
        {
            if (service == null)
            {
                throw new ArgumentNullException( "service" );
            }
            if (method == null)
            {
                throw new ArgumentNullException( "method" );
            }
            return delegate ( MethodInfo methodInfo )
            {
                if (ForService( service )( methodInfo ))
                {
                    return methodInfo.Name.Matches( method );
                }
                return false;
            };
        }

        public  static bool Matches ( this string input, string pattern )
        {
            if (string.IsNullOrEmpty( input ))
            {
                throw new ArgumentNullException( "input" );
            }
            if (string.IsNullOrEmpty( pattern ))
            {
                throw new ArgumentNullException( "pattern" );
            }

            return System.Text.RegularExpressions.Regex.IsMatch( input, pattern );
        }

    }

}
