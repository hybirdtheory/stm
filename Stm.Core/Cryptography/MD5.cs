using System;
using System.Security.Cryptography;

namespace Stm.Core.Cryptography
{
    public class MD5
    {
        public static string M_123456
        {
            get
            {
                return Encrypt("123456");
            }
        }

        public static string Encrypt2(string encryptString)
        {
            //创建MD5密码服务提供程序
            var md5 = new MD5CryptoServiceProvider();

            //获取加密字符串
            string result = BitConverter.ToString(md5.ComputeHash(System.Text.Encoding.GetEncoding("gb2312").GetBytes(encryptString)));

            //释放资源
            md5.Clear();

            //返回MD5值的字符串表示
            return result.Replace("-", "").ToLower();
        }

        public static string Encrypt(string myString)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] fromData = System.Text.Encoding.UTF8.GetBytes(myString);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");
            }

            return byte2String;
        }
    }
}
