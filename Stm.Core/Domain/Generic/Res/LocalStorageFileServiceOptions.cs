using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Domain.Generic
{
    public class LocalStorageFileServiceOptions
    {
        /// <summary>
        /// 根文件夹
        /// e.g.: c:\intput\wwwroot\
        /// </summary>
        public string DirectoryPath { get; set; }

        /// <summary>
        /// 网络访问根目录
        /// e.g.: upload/
        /// </summary>
        public string UrlRoot { get; set; }
    }
}
