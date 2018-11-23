
using System;

namespace Stm.Core.Domain.Generic
{
    /// <summary>
    /// 文件服务
    /// </summary>
    public interface IResService
    {
        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="filedata">文件数据</param>
        /// <param name="mimetype">mime类型</param>
        /// <returns></returns>
        string CreateRes (byte[] filedata, string mimetype);

        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="path">存储地址</param>
        /// <returns></returns>
        string GetResUrl ( string path );

    }
}
