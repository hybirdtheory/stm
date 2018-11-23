using Stm.Core.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Domain.Generic
{
    public interface IAuditLogService
    {
        /// <summary>
        /// 记录审计日志
        /// </summary>
        /// <param name="stmPrincipal"></param>
        /// <param name="content"></param>
        System.Threading.Tasks.Task WriteAuditLogAsync ( StmPrincipal stmPrincipal,string content, AuditAdditional auditAdditional );
    }
}
