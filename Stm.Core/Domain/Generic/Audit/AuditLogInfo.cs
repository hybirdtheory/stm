using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Domain.Generic
{
    public class AuditLogInfo
    {
        public string AuditLogId { get; set; }

        public DateTime LogDt { get; set; }

        public string Content { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string Ip { get; set; }
    }
}
