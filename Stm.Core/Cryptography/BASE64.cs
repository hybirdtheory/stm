using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stm.Core.Cryptography
{
    public class BASE64
    {
        public static string Decrypt ( string value )
        {
            if (value == null || value == "")
            {
                return "";
            }
            byte[] bytes = Convert.FromBase64String( value );
            return Encoding.UTF8.GetString( bytes );

        }

        public static string Encrypt ( string value )
        {
            if (value == null || value == "")
            {
                return "";
            }
            byte[] bytes = Encoding.UTF8.GetBytes( value );
            return Convert.ToBase64String( bytes );

        }
    }
}
