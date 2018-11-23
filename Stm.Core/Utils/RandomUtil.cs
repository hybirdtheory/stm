namespace Stm.Core.Utils
{
    /// <summary>
    /// 随机字符
    /// </summary>
    public class RandomUtil
    {
        /// <summary>
        /// 字母和数字
        /// </summary>
        public static string NUMBER_AND_CHAR = "1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,J,K,M,N,P,Q,R,S,T,U,W,X,Y,Z";

        /// <summary>
        /// 数字
        /// </summary>
        public static string NUMBER = "0,1,2,3,4,5,6,7,8,9";

        /// <summary>
        /// 字母
        /// </summary>
        public static string CHAR = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";

        /// <summary>
        /// 获取随机数字
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        public static int GetNumber(int min, int max)
        {
            System.Random rand = new System.Random(System.Guid.NewGuid().GetHashCode());
            return rand.Next(max + 1 - min) + min;
        }

        /// <summary>
        /// 获取随机字符串
        /// </summary>
        /// <param name="length">字符串长度</param>
        /// <param name="list">候选序列</param>
        /// <returns></returns>
        public static string GetRandomString(int length, string list)
        {
            string[] allCharArray = list.Split(',');
            string randomCode = "";
            for (int i = 0; i < length; i++)
            {
                int t = GetNumber(0, allCharArray.Length - 1);
                string c = allCharArray[t];
                if (i == 0 && (c == "0" || c == "O"))
                {
                    c = GetRandomString(1, list);
                }
                randomCode += c;
            }
            return randomCode;
        }

        /// <summary>
        /// 获取随机字符串(字母和数字)
        /// </summary>
        /// <param name="length">字符串长度</param>
        /// <returns></returns>
        public static string GetRandomString(int length)
        {
            return GetRandomString(length, NUMBER_AND_CHAR);
        }
    }
}
