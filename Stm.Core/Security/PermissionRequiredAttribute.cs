using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Security
{
    public class PermissionRequiredAttribute: Attribute
    {
        public Permission Permission { get; private set; }

        public PermissionRequiredAttribute(string code )
        {
            Permission = new Permission { Code = code };
        }


        public PermissionRequiredAttribute ( string code,string name )
        {
            Permission = new Permission { Code = code , Name=name};
        }
    }
}
