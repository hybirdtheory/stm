using Microsoft.AspNetCore.Http;
using Stm.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.AspNetCore
{
    public class WebUserIpAccessor : IUserIpAccessor
    {
        private IHttpContextAccessor _httpContextAccessor;

        public WebUserIpAccessor (IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetIp ()
        {
            return _httpContextAccessor.HttpContext.Request.GetIP();
        }
    }
}
