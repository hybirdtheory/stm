using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Linq;
using System.IO;
using System.Security.Principal;

namespace Stm.Core.Security
{
    public class StmPrincipal: ClaimsPrincipal
    {
        public StmPrincipal () : base() { }

        public StmPrincipal ( IEnumerable<ClaimsIdentity> identities ) : base( identities ) { }
   
        public StmPrincipal ( BinaryReader reader ) : base( reader ) { }
        
        public StmPrincipal ( IIdentity identity ) : base( identity ) { }

        public StmPrincipal ( IPrincipal principal ) : base( principal ) { }

        public bool HasPermission(string permissionCode )
        {
            return string.IsNullOrWhiteSpace( permissionCode)||
                    this.HasClaim( claim => claim.Type == ClaimTypes.Permissions && (claim.Value ?? "").Split( ',' ).Contains( permissionCode ) );
        }

        public override bool IsInRole ( string role )
        {
            return string.IsNullOrWhiteSpace( role ) ||
                    this.HasClaim( claim => claim.Type == ClaimTypes.Roles && (claim.Value ?? "").Split( ',' ).Contains( role ) );
        }
    }
}
