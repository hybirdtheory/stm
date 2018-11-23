using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Linq;

namespace Stm.Core.Security
{
    public static class SecurityExtensions
    {
        public static bool IsAllowExcute(this StmPrincipal user, MethodInfo method )
        {
            var permissions = method.GetCustomAttributes<PermissionRequiredAttribute>().Select( attr => attr.Permission );

            var roles = method.GetCustomAttributes<RoleRequiredAttribute>().Select( attr => attr.Role );

            if (!permissions.Any() && !roles.Any())
            {
                return true;
            }

            if (user == null) return false;
            if (user.Identity == null) return false;
            if (!user.Identity.IsAuthenticated) return false;

            if (permissions.Any()&& permissions.Any( p => user.HasPermission( p.Code ) ))
            {
                return true;
            }
            if (roles.Any() && roles.Any( role=> user.IsInRole( role.Code ) ))
            {
                return true;
            }

            return false;
        }
    }
}
