using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Stm.Core.Cryptography
{
    public class DES
    {
        private static byte[] Keys = null;
        private static byte[] IV =null;

        
        static DES()
        {
            //Keys = Encoding.ASCII.GetBytes("chichuang");
            //IV = Encoding.ASCII.GetBytes("chichuang");
        }

        /// <summary>
        /// 设置偏移key
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="iv"></param>
        public static void SetKeysAndIV(string keys,string iv)
        {
            Keys = Encoding.ASCII.GetBytes(keys);
            IV = Encoding.ASCII.GetBytes(iv);
        }

        /// <summary>

        /// DES加密字符串

        /// </summary>

        /// <param name="encryptString">待加密的字符串</param>

        /// <param name="encryptKey">加密密钥,要求为8位</param>

        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>

       public static string Encrypt(string encryptString)
        {
            if (Keys == null || IV == null) throw new Exception( "Keys or IV not set" );

            try
            {
                byte[] rgbKey = Keys;

                byte[] rgbIV = IV;

                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);

                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();

                MemoryStream mStream = new MemoryStream();

                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);

                cStream.Write(inputByteArray, 0, inputByteArray.Length);

                cStream.FlushFinalBlock();
                byte[] msbty;
                //if (compress)
                //{
                //    msbty = Compress.EnCompress(mStream.ToArray());
                //}
                //else
                //{
                msbty = mStream.ToArray();
                //}
                StringBuilder ret = new StringBuilder();
                foreach (byte b in msbty)
                {
                    ret.AppendFormat("{0:X2}", b);
                }
                ret.ToString();
                return  ret.ToString();

            }

            catch
            {
                return encryptString;

            }

        } 

        /// <summary>

        /// DES解密字符串

        /// </summary>

        /// <param name="decryptString">待解密的字符串</param>

        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>

        public static string Decrypt(string decryptString)
        {
            if (Keys == null || IV == null) throw new Exception( "Keys or IV not set" );
            
            byte[] rgbKey = Keys;

            byte[] rgbIV = IV;

            byte[] inputByteArray = new byte[decryptString.Length / 2];
            for (int x = 0; x < decryptString.Length / 2; x++)
            {
                int i = (Convert.ToInt32(decryptString.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }
            byte[] btys;
            //if (compress)
            //{
            //    btys = Compress.DeCompress(inputByteArray);
            //}
            //else
            //{
            btys = inputByteArray;
            //byte[] inputByteArray = Convert.FromBase64String(decryptString);

            DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();

            MemoryStream mStream = new MemoryStream();

            CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);

            cStream.Write(btys, 0, btys.Length);

            cStream.FlushFinalBlock();

            return Encoding.UTF8.GetString(mStream.ToArray());
           

        }

    }
}
