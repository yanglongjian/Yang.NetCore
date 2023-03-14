using Furion.DistributedIDGenerator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yang.Core
{
    /// <summary>
    ///分布式Id生成(SnowFlake)
    /// </summary>
    public abstract class Id
    {
        /// <summary>
        ///GUID
        /// </summary>
        /// <returns></returns>
        public static string NextId()
        {
            return IDGen.NextID().ToString("N");
        }

        /// <summary>
        /// 短ID
        /// </summary>
        /// <returns></returns>
        public static string ShortId(int length, bool useNumbers = true)
        {
            return ShortIDGen.NextID(new GenerationOptions
            {
                UseNumbers = useNumbers, //使用数字
                UseSpecialCharacters = false, // 使用特殊字符
                Length = length    // 长度
            });
        }


        /// <summary>
        /// 特定长度随机字符串
        /// </summary>
        public static string RandomCode(int length, ValidateCodeType codeType = ValidateCodeType.NumberAndLetter)
        {
            return codeType switch
            {
                ValidateCodeType.Number => GetRandomNums(length),
                ValidateCodeType.Hanzi => GetRandomHanzis(length),
                _ => GetRandomNumsAndLetters(length),
            };
        }


        #region 
        private static string GetRandomNums(int length)
        {
            int[] ints = new int[length];
            for (int i = 0; i < length; i++)
            {
                ints[i] = new Random().Next(0, 9);
            }
            return ints.ExpandAndToString("");
        }

        private static string GetRandomNumsAndLetters(int length)
        {
            const string allChar = "2,3,4,5,6,7,8,9," +
                "A,B,C,D,E,F,G,H,J,K,M,N,P,Q,R,S,T,U,V,W,X,Y,Z," +
                "a,b,c,d,e,f,g,h,k,m,n,p,q,r,s,t,u,v,w,x,y,z";
            string[] allChars = allChar.Split(',');
            List<string> result = new();
            while (result.Count < length)
            {
                int index = new Random().Next(allChars.Length);
                string c = allChars[index];
                result.Add(c);
            }
            return result.ExpandAndToString("");
        }

        /// <summary>
        /// 获取汉字验证码
        /// </summary>
        /// <param name="length">验证码长度</param>
        /// <returns></returns>
        private static string GetRandomHanzis(int length)
        {
            //汉字编码的组成元素，十六进制数
            string[] baseStrs = "0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f".Split(',');
            Encoding encoding = Encoding.GetEncoding("GB2312");
            string result = null;

            //每循环一次产生一个含两个元素的十六进制字节数组，并放入bytes数组中
            //汉字由四个区位码组成，1、2位作为字节数组的第一个元素，3、4位作为第二个元素
            for (int i = 0; i < length; i++)
            {
                Random rnd = new Random();
                int index1 = rnd.Next(11, 14);
                string str1 = baseStrs[index1];

                int index2 = index1 == 13 ? rnd.Next(0, 7) : rnd.Next(0, 16);
                string str2 = baseStrs[index2];

                int index3 = rnd.Next(10, 16);
                string str3 = baseStrs[index3];

                int index4 = index3 == 10 ? rnd.Next(1, 16) : (index3 == 15 ? rnd.Next(0, 15) : rnd.Next(0, 16));
                string str4 = baseStrs[index4];

                //定义两个字节变量存储产生的随机汉字区位码
                byte b1 = Convert.ToByte(str1 + str2, 16);
                byte b2 = Convert.ToByte(str3 + str4, 16);
                byte[] bs = { b1, b2 };

                result += encoding.GetString(bs);
            }
            return result;
        }

        #endregion


        
    }

   
    /// <summary>
    /// 验证码类型
    /// </summary>
    public enum ValidateCodeType
    {
        /// <summary>
        /// 纯数值
        /// </summary>
        Number,

        /// <summary>
        /// 数值与字母的组合
        /// </summary>
        NumberAndLetter,

        /// <summary>
        /// 汉字
        /// </summary>
        Hanzi
    }
}


