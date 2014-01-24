using System;
using System.Globalization;
using System.Text;
using ThinkAway.Security;
using ThinkAway.Text;

/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Core
{
    /// <summary>
    /// 将一个基本类型转换为另一个类型 , 无法继承此类
    /// </summary>
    internal sealed class ConvertEx
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] FromBase64(string str)
        {
            return Base64.Decode(str);
        }

        internal static string ToBase64(byte[] from)
        {
            return Base64.Encode(from);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToBase64(string str)
        {
            return Base64.Encode(Encoding.Default.GetBytes(str));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToMDString(string str)
        {
            return MD5.MDString(str);
        }

        /// <summary>
        /// 解释PDU编译的内容
        /// </summary>
        /// <param name="str">PDU编译的内容</param>
        /// <returns>翻译后的内容</returns>
        public static string FromBit7String(string str)
        {
            byte[] src = HexStringToBytes(str);
            if (src.Length == 0)
            {
                return String.Empty;
            }
            int srcLength = src.Length;
            int dstLength = srcLength * 8 / 7;
            byte[] dst = new byte[dstLength];
            int a, b;
            for (a = 0, b = 0; b < srcLength; a++, b++)
            {
                int k = a % 8;
                if (a > 0)
                {
                    dst[a] = (byte)(((src[b] << k) & 0x7f) | (src[b - 1] >> 8 - k));
                }
                else
                {
                    dst[a] = (byte)(src[b] & 0x7f);
                }
                if (k == 7 && a > 0)
                {
                    dst[++a] = (byte)(src[b] & 0x7f);
                }
            }
            return Encoding.ASCII.GetString(dst);
        }

        /// <summary>
        /// Bit 7 编码
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToBit7String(string text)
        {
            //存储中间字符串 二进制串
            string userData = String.Empty;
            //
            Encoding encodingAsscii = Encoding.ASCII;
            //
            byte[] bytes = encodingAsscii.GetBytes(text);
            //高低交换 二进制串
            for (int i = text.Length; i > 0; i--)
            {
                string temp = System.Convert.ToString(bytes[i - 1], 2);
                //不够7位，补齐
                while (temp.Length < 7)
                {
                    temp = temp.Insert(0, "0");
                }
                userData += temp;
            }
            return userData;
        }
        /// <summary>
        /// 十六进制的字符串转换为byte数组
        /// </summary>
        /// <param name="hexString">转换内容</param>
        /// <returns>翻译后的串</returns>
        private static byte[] HexStringToBytes(string hexString)
        {
            int hexStringLength = hexString.Length;
            if (hexStringLength < 2 || hexStringLength % 2 != 0)
            {
                return new byte[0];
            }
            byte[] data = new byte[hexStringLength / 2];
            for (int i = 0, j = 0; i < hexStringLength; i += 2, j++)
            {
                data[j] = Byte.Parse(hexString.Substring(i, 2), NumberStyles.HexNumber);
            }
            return data;
        }


        /// <summary>
        /// 将发送短信内容进行编码
        /// 采用Big-Endian 字节顺序的 Unicode 格式编码，将高低位互换
        /// 将转换后的短信内容存进字节数组
        /// 去掉在进行Unicode格式编码中，两个字节中的"-",例如：00-21，变成0021
        /// 将整条短信内容的长度除2，保留两位16进制数
        /// </summary>
        /// <param name="text">消息内容</param>
        /// <returns>返回消息编码后的长度</returns>
        public static string ToUcs2(string text)
        {
            //对短信内容进行编码 
            string result = String.Empty;
            Encoding encoding = Encoding.BigEndianUnicode;

            byte[] bytes = encoding.GetBytes(text);

            for (int i = 0; i < bytes.Length; i++)
            {
                result += BitConverter.ToString(bytes, i, 1);
            }
            return result;
        }
        /// <summary>
        /// Bit 8 编码
        /// 每8位取位一个字符 即完成编码
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToBit8String(string text)
        {
            string result = String.Empty;
            //每8位取位一个字符 即完成编码
            for (int i = text.Length; i > 0; i -= 8)
            {
                result += System.Convert.ToInt32(i > 8 ? text.Substring(i - 8, 8) : text.Substring(0, i), 2).ToString("X2");
            }
            return result;
        }
    }
}
