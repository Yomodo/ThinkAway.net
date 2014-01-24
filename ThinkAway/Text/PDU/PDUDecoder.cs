using System;
using System.IO;
using System.Text;
using System.Globalization;
/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Text.PDU
{
    public class PduDecoder
    {
        private readonly string _pduString = String.Empty;

        /// <summary>
        /// PDU 解码 
        /// </summary>
        /// <param name="pduString">PDU 编码字符串</param>
        /// <param name="centerNumber"></param>
        /// <param name="firstOctet"></param>
        /// <param name="senderNumber"></param>
        /// <param name="protocol"></param>
        /// <param name="characterEncoding"></param>
        /// <param name="time"></param>
        /// <param name="message"></param>
        public static void Decoding(string pduString, out string centerNumber, out string firstOctet, out string senderNumber, out string protocol, out string characterEncoding, out string time, out string message)
        {
            PduDecoder pduDecoder = new PduDecoder(pduString);
            pduDecoder.Decoder(out centerNumber, out firstOctet, out senderNumber, out protocol, out characterEncoding, out time, out message);
        }
        /// <summary>
        /// PDU解码
        /// </summary>
        /// <param name="source"></param>
        public PduDecoder(string source)
        {
            _pduString = source;
        }
        /// <summary>
        /// 转换到SMSEnity
        /// </summary>
        /// <returns>短信息</returns>
        public void Decoder(out string centerNumber, out string firstOctet, out string senderNumber, out string protocol, out string characterEncoding, out string time, out string message)
        {
            StringReader pduReader = new StringReader(_pduString, 0);
            centerNumber = DecodeNumber(ParseCenterNumber(pduReader));
            firstOctet = ParseFirstOctet(pduReader);
            senderNumber = DecodeNumber(ParseSenderNumber(pduReader));
            protocol = ParseProtocol(pduReader);
            characterEncoding = ParseCharsetEncoding(pduReader);
            DateTime timeStamp = DecodeTimestamp(ParseCenterTime(pduReader));
            message = DecodeMessage(ParseUserData(pduReader), characterEncoding);
            time = timeStamp.ToString(CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// 解析号码
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private static string DecodeNumber(string number)
        {
            StringBuilder sb = new StringBuilder();
            StringReader reader = new StringReader(number, 0);
            string str = reader.NextString(2);
            if (str == "91")
            {
                sb.Append("+");
            }
            while (reader.HasNext())
            {
                char one = reader.NextChar();
                char two = reader.NextChar();
                sb.Append(two);
                sb.Append(one);
            }
            if (sb[sb.Length - 1] == 'F' || sb[sb.Length - 1] == 'f')
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 解析时间
        /// </summary>
        /// <param name="timestampString"></param>
        /// <returns></returns>
        private static DateTime DecodeTimestamp(string timestampString)
        {
            DateTime timeStamp;
            //分析接收时间
            timestampString = string.Format("{0}{1}-{2}{3}-{4}{5} {6}{7}:{8}{9}:{10}{11}",
                                            timestampString[1], timestampString[0],
                                            timestampString[3], timestampString[2],
                                            timestampString[5], timestampString[4],
                                            timestampString[7], timestampString[6],
                                            timestampString[9], timestampString[8],
                                            timestampString[11], timestampString[10]
                );
            try
            {
                timeStamp = System.Convert.ToDateTime(timestampString);
            }
            catch
            {
                timeStamp = new DateTime();
            }
            return timeStamp;
        }
        /// <summary>
        /// 解析信息
        /// </summary>
        /// <param name="userDate"></param>
        /// <param name="characterEncoding"></param>
        /// <returns></returns>
        private string DecodeMessage(string userDate, string characterEncoding)
        {
            string message = String.Empty;
            if (!String.IsNullOrEmpty(userDate))
            {
                int code = Int32.Parse(characterEncoding, NumberStyles.HexNumber);
                switch (code)
                {
                    case 0: //bit7    
                        message = Core.ConvertEx.FromBit7String(userDate);
                        break;
                    case 8: //Uncode
                        byte[] bytes1 = new byte[userDate.Length / 2];
                        for (int i = 0, j = 0; j < bytes1.Length; i += 2, j++)
                        {
                            bytes1[j] = Byte.Parse(userDate.Substring(i, 2), NumberStyles.HexNumber);
                        }
                        message = Encoding.BigEndianUnicode.GetString(bytes1);
                        break;
                    default:
                        byte[] bytes2 = new byte[userDate.Length / 2];
                        for (int i = 0, j = 0; j < bytes2.Length; i += 2, j++)
                        {
                            bytes2[j] = Byte.Parse(userDate.Substring(i, 2), NumberStyles.HexNumber);
                        }
                        message = Encoding.ASCII.GetString(bytes2);
                        break;
                }
            }
            return message;
        }

        /// <summary>
        /// 解析短信中心号码
        /// </summary>
        /// <param name="pduReader">PDUReader 读取器</param>
        /// <returns>短信中心号码(编码)</returns>
        private static string ParseCenterNumber(StringReader pduReader)
        {
            string str = pduReader.NextString(2);
            //取回地址信息长度(str * 2 = length)
            int length = Int32.Parse(str, NumberStyles.HexNumber) * 2;
            //偏移 2 //读取号码
            pduReader.Offset = 2;
            //短信中心号码 *编码
            return pduReader.NextString(length);
        }
        /// <summary>
        /// 解析 FirstOctet
        /// </summary>
        /// <param name="pduReader"></param>
        /// <returns></returns>
        private static string ParseFirstOctet(StringReader pduReader)
        {
            return pduReader.NextString(2);
        }
        /// <summary>
        /// 解析发送方号码
        /// </summary>
        /// <param name="pduReader"></param>
        /// <returns></returns>
        private static string ParseSenderNumber(StringReader pduReader)
        {
            string str = pduReader.NextString(2);
            if (str == "FF" || str == "00")
            {
                str = pduReader.NextString(2);
            }
            int length = Int32.Parse(str, NumberStyles.HexNumber);
            if (length % 2 != 0)
            {
                //如果长度不是偶数位 添加 'F' 补齐
                length += 1;
            }
            //取回区域类型 '91'.length = 2            
            return pduReader.NextString(2 + length);
        }
        /// <summary>
        /// 解析协议
        /// </summary>
        /// <param name="pduReader"></param>
        /// <returns></returns>
        private static string ParseProtocol(StringReader pduReader)
        {
            return pduReader.NextString(2);
        }
        /// <summary>
        /// 解析数据编码
        /// </summary>
        /// <param name="pduReader"></param>
        /// <returns></returns>
        private static string ParseCharsetEncoding(StringReader pduReader)
        {
            return pduReader.NextString(2);
        }
        /// <summary>
        /// 解析时间
        /// </summary>
        /// <param name="pduReader"></param>
        /// <returns></returns>
        private static string ParseCenterTime(StringReader pduReader)
        {
            //14 位长日期格式
            return pduReader.NextString(14);
        }
        /// <summary>
        /// 解析用户数据
        /// </summary>
        /// <param name="pduReader"></param>
        /// <returns></returns>
        private static string ParseUserData(StringReader pduReader)
        {
            //获得信息内容长度
            int length = Int32.Parse(pduReader.NextString(2), NumberStyles.HexNumber);
            string userData = pduReader.NextString(length * 2);
            return userData;
        }

        internal static IO.Modem.SMSEnity Decoding(string pduStr)
        {
            throw new NotImplementedException();
        }
    }
}
