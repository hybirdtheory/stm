using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Stm.Core.Domain.Generic;
using Microsoft.Extensions.Configuration;

namespace Stm.Core.Domain.Generic
{
    public static class ResServiceExtensions
    {
        /// <summary>
        /// 本地文件存储服务
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="dir">文件存放根目录</param>
        /// <param name="urlroot"></param>
        public static void AddLocalStorageFileService ( this IServiceCollection serviceCollection,string dir,string urlroot )
        {
            Action<LocalStorageFileServiceOptions> options = opt =>
            {
                opt.DirectoryPath = dir;
                opt.UrlRoot = urlroot;
            };
            serviceCollection.Configure( options );

            serviceCollection.AddSingleton<IResService, LocalStorageFileService>();
        }
        public static void AddLocalStorageFileServiceServer ( this IServiceCollection serviceCollection, string dir, string urlroot )
        {
            Action<LocalStorageFileServiceOptions> options = opt =>
            {
                opt.DirectoryPath = dir;
                opt.UrlRoot = urlroot;
            };
            serviceCollection.Configure( options );

            serviceCollection.AddSingleton<IResService, LocalStorageFileService>();
        }

        public static void AddResServiceHttpClient ( this IServiceCollection serviceCollection )
        {
            serviceCollection.AddHttpService<IResService>( "http://{lb:resservice}/{service}/{action}" );

        }
    }
}
