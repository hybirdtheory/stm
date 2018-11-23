using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Utils
{
    public static class FileUtil
    {
        public static string GetExtensions ( string filename )
        {
            if (string.IsNullOrWhiteSpace( filename )) return filename;

            return System.IO.Path.GetExtension( filename );
        }
        public static string GetExtensionsFromMimeType ( string mimetype )
        {
            if (string.IsNullOrWhiteSpace( mimetype )) return "";

            string retval = "";
            switch (mimetype.ToLower().Trim())
            {
                case "image/jpeg": retval = ".jpg"; break;
                case "image/gif": retval = ".gif"; break;
                case "image/png": retval = ".png"; break;
                case "image/bmp": retval = ".bmp"; break;
                case "text/plain": retval = ".txt"; break;
                case "application/vnd.ms-excel": retval = ".xls"; break;
                case "application/msword": retval = ".doc"; break;
                case "application/zip": retval = ".zip"; break;
                case "audio/mp3": retval = ".mp3"; break;
                case "video/mpeg4": retval = ".mp3"; break;

                default: retval = ""; break;
            }

            if (string.IsNullOrWhiteSpace( retval ))
            {
                throw new BaseException( "不允许使用此类型文件" );
            }
            return retval;

        }
    }
}
