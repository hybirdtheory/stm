using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Security
{
    /// <summary>
    /// 权限信息
    /// </summary>
    public class Permission
    {
        /// <summary>
        /// 权限码
        /// </summary>
        public string Code { get; set; }

        private string _name;
        public String Name
        {
            get
            {
                return _name ?? Code;
            }
            set
            {
                _name = value;
            }
        }

    }
}
