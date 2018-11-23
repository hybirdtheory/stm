using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Stm.Core.Db;
using Stm.Core.Security;
using System.Linq;

namespace Stm.Core.Domain.Generic
{
    public class DbBasedAuditLogService : IAuditLogService
    {
        private DbContext _dbContext;

        public DbBasedAuditLogService ( IDbContextFactory dbContextFactory, RepositoryOptions<DbBasedAuditLogService> options )
        {
            _dbContext = dbContextFactory.GetDbContext( options.DbName );
        }

        public async System.Threading.Tasks.Task WriteAuditLogAsync ( StmPrincipal stmPrincipal, string content, AuditAdditional auditAdditional )
        {
            AuditLogInfo auditLogInfo = new AuditLogInfo();
            auditLogInfo.AuditLogId = Guid.NewGuid().ToString( "N" );
            auditLogInfo.Content = (content ?? "").Length < 255 ? content : content.Substring( 0, 255 );
            auditLogInfo.LogDt = DateTime.Now;
            auditLogInfo.UserId = stmPrincipal?.Claims.FirstOrDefault( t => t.Type == ClaimTypes.Id )?.Value;
            auditLogInfo.UserName = stmPrincipal?.Claims.FirstOrDefault( t => t.Type == ClaimTypes.Username )?.Value;
            auditLogInfo.Ip = auditAdditional?.Ip;

            _dbContext.Add( auditLogInfo );

            await _dbContext.SaveChangesAsync();
        }
    }
}
