using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Security
{
    public class RoleRequiredAttribute : Attribute
    {
        public Role Role { get; private set; }

        public RoleRequiredAttribute ( string code )
        {
            Role = new Role { Code = code };
        }


        public RoleRequiredAttribute ( string code,string name )
        {
            Role = new Role { Code = code , Name=name};
        }
    }
}
