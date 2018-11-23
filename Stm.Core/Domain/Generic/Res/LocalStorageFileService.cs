
using Microsoft.Extensions.Options;
using Stm.Core.Utils;
using System;

namespace Stm.Core.Domain.Generic
{
    /// <summary>
    /// 本机存储服务，用于单机应用
    /// </summary>
    public class LocalStorageFileService : IResService
    {
        /// <summary>
        /// 根文件夹
        /// e.g.: c:\intput\wwwroot\
        /// </summary>
        private string _appRootFolder;

        /// <summary>
        /// 网络访问根目录
        /// e.g.: upload/
        /// </summary>
        private string _urlRoot;


        /// <summary>
        /// 子文件夹生成格式
        /// e.g.:yyyy/MM/dd
        /// </summary>
        private string _subfolderFormat="yyyy/MM/dd";


        public LocalStorageFileService(
            IOptions<LocalStorageFileServiceOptions> options )
        {
            string appRootFolder = options.Value.DirectoryPath;
            string urlRoot = options.Value.UrlRoot;
            appRootFolder = appRootFolder.Replace("/", "\\");
            if (!appRootFolder.EndsWith("\\")) appRootFolder += "\\";

            urlRoot = urlRoot.Replace("\\", "/");
            if(!urlRoot.EndsWith("/")) urlRoot += "/";

            _appRootFolder = appRootFolder;
            _urlRoot = urlRoot;
        }

        /// <summary>
        /// 设置子文件夹生成格式
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public LocalStorageFileService SetSubfolderFormat(string format)
        {
            _subfolderFormat = format;

            return this;
        }

        /// <summary>
        /// 用当前时间获取存储子文件夹
        /// e.g.: 2018/01/01/
        /// </summary>
        /// <returns></returns>
        private string GetSubFolder()
        {
            var format = _subfolderFormat;
            if (string.IsNullOrWhiteSpace(format)) return "";

            if (!format.EndsWith("/")) format = format + "/";

            return DateTime.Now.ToString(format);
        }

        public string CreateRes ( byte[] filedata, string mimetype)
        {

            var path = GetSubFolder() + Guid.NewGuid().ToString() + FileUtil.GetExtensionsFromMimeType( mimetype );

            var fullpath = _appRootFolder + path.Replace('/','\\');

            //如果文件夹不存在就创建文件夹
            var dir = new System.IO.FileInfo(fullpath).Directory;
            if (!dir.Exists)
            {
                System.IO.Directory.CreateDirectory(dir.FullName);
            }

            //保存文件
            System.IO.File.WriteAllBytes(fullpath, filedata);

            return path;
        }


        public string GetResUrl ( string path )
        {

            return _urlRoot+ path;
        }
    }
}
