using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Db
{
    /// <summary>
    /// 数据库连接管理
    /// </summary>
    public interface IDbContextFactory
    {
        /// <summary>
        /// 获取DbContext
        /// </summary>
        /// <param name="registration"></param>
        /// <returns></returns>
        DbContext GetDbContext(string registration);
    }
}
