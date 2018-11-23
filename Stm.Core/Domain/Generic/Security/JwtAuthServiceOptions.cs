using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Domain.Generic
{
    public class JwtAuthServiceOptions
    {
        /// <summary>
        /// jwt加密key
        /// </summary>
        public string SecretKey { get; set; }
    }
}
