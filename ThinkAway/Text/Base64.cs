using System;
using System.Collections;
using System.Text;

namespace ThinkAway.Text
{
    /// <summary>
    ///Base64 编码/解码
    /// </summary>
    public class Base64
    {
        /// <summary>
        /// Base64Code
        /// </summary>
        const string Base64Code = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
        /// <summary>
        /// Base64编码
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Encode(byte[] data)
        {
            char[] base64Code = Base64Code.ToCharArray();
            const byte empty = (byte)0;
            ArrayList byteMessage = new ArrayList(data);
            int messageLen = byteMessage.Count;
            int page = messageLen / 3;
            int use;
            if ((use = messageLen % 3) > 0)
            {
                for (int i = 0; i < 3 - use; i++)
                    byteMessage.Add(empty);
                page++;
            }
            StringBuilder stringBuilder = new StringBuilder(page * 4);
            for (int i = 0; i < page; i++)
            {
                byte[] instr = new byte[3];
                instr[0] = (byte)byteMessage[i * 3];
                instr[1] = (byte)byteMessage[i * 3 + 1];
                instr[2] = (byte)byteMessage[i * 3 + 2];
                int[] outstr = new int[4];
                outstr[0] = instr[0] >> 2;
                outstr[1] = ((instr[0] & 0x03) << 4) ^ (instr[1] >> 4);
                if (!instr[1].Equals(empty))
                    outstr[2] = ((instr[1] & 0x0f) << 2) ^ (instr[2] >> 6);
                else
                    outstr[2] = 64;
                if (!instr[2].Equals(empty))
                    outstr[3] = (instr[2] & 0x3f);
                else
                    outstr[3] = 64;
                stringBuilder.Append(base64Code[outstr[0]]);
                stringBuilder.Append(base64Code[outstr[1]]);
                stringBuilder.Append(base64Code[outstr[2]]);
                stringBuilder.Append(base64Code[outstr[3]]);
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Base64解码
        /// </summary>
        /// <param name="str"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public static byte[] Decode(string str)
        {
            str = str.Replace(" ", null);
            str = str.Replace("\r", null);
            str = str.Replace("\n", null);

            if ((str.Length%4) != 0)
            {
                throw new ArgumentException("BASE64编码长度不正确.", "str");
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(str, "^[A-Z0-9/+=]*$",
                                                              System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            {
                throw new FormatException("输入的不是有效的 Base-64 字符串，因为它包含非 Base-64 字符、两个以上的填充字符，或者填充字符间包含非空白字符。");
            }
            int page = str.Length/4;
            ArrayList outMessage = new ArrayList(page*3);
            char[] message = str.ToCharArray();
            for (int i = 0; i < page; i++)
            {
                byte[] instr = new byte[4];
                instr[0] = (byte) Base64Code.IndexOf(message[i*4]);
                instr[1] = (byte) Base64Code.IndexOf(message[i*4 + 1]);
                instr[2] = (byte) Base64Code.IndexOf(message[i*4 + 2]);
                instr[3] = (byte) Base64Code.IndexOf(message[i*4 + 3]);
                byte[] outstr = new byte[3];
                outstr[0] = (byte) ((instr[0] << 2) ^ ((instr[1] & 0x30) >> 4));
                if (instr[2] != 64)
                {
                    outstr[1] = (byte) ((instr[1] << 4) ^ ((instr[2] & 0x3c) >> 2));
                }
                else
                {
                    outstr[2] = 0;
                }
                if (instr[3] != 64)
                {
                    outstr[2] = (byte) ((instr[2] << 6) ^ instr[3]);
                }
                else
                {
                    outstr[2] = 0;
                }
                outMessage.Add(outstr[0]);
                if (outstr[1] != 0)
                    outMessage.Add(outstr[1]);
                if (outstr[2] != 0)
                    outMessage.Add(outstr[2]);
            }
            byte[] outbyte = (byte[])outMessage.ToArray(typeof(byte));
            return outbyte;
        }

        internal static string Decode(string str, Encoding encoding)
        {
            if (str == null)
                throw new ArgumentNullException("str");

            if (encoding == null)
                throw new ArgumentNullException("encoding");
            return encoding.GetString(Decode(str));
        }
    }
}