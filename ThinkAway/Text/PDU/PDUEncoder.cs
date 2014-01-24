using System;
/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Text.PDU
{
    /// <summary>
    /// PDU 编码
    /// </summary>
    public class PduEncoder
    {
        #region 字段属性

        /// <summary>
        /// 协议数据单元类型(1个8位组)
        /// </summary>
        public struct ProtocolDataUnitType
        {
            public const string TP_VP = "11";
        }

        /// <summary>
        /// 所有成功的短信发送参考数目（0..255）
        /// (1个8位组)
        /// </summary>
        public struct MessageReference
        {
            public const string TP_MR = "00";
        }
        /// <summary>
        /// 参数显示消息中心以何种方式处理消息内容
        /// （比如FAX,Voice）(1个8位组)
        /// </summary>
        public struct ProtocolIdentifer
        {
            public const string TP_PID = "00";
        }

        /// <summary>
        /// 参数显示用户数据编码方案(1个8位组)
        /// </summary>
        public struct EncodingCharSet
        {
            public const string Bit7 = "00";
            public const string Bit8 = "18";
            public const string Usc2 = "08";

        }
        /// <summary>
        /// 短消息有效期(0,1,7个8位组)
        /// </summary>
        public struct ValidityPeriod
        {
            /// <summary>
            /// 00 to 8F(0 to 143)          (VP+1)*5 分钟  
            /// </summary>
            public const string Min5 = "00";
            /// <summary>
            /// 90 to A7(144 to 167)        12小时+(VP-143)*30分钟 
            /// </summary>
            public const string Hour12 = "90";
            /// <summary>
            /// A8 to C4(168 to 196)        (VP-166)*1天 
            /// </summary>
            public const string Day1 = "C4";
            /// <summary>
            /// C5 to FF(197 to 255)        (VP-192)*1 周
            /// </summary>
            public const string Week1 = "C5";

        }
        #endregion
        #region
        /// <summary>
        /// 消息服务中心编码
        /// (1-12个8位组)
        /// </summary>
        /// <param name="centerNumber">短信中心号码</param>
        /// <returns></returns>
        public string EncodeCenterNumber(string centerNumber)
        {
            if (!String.IsNullOrEmpty(centerNumber))
            {
                centerNumber = centerNumber.TrimStart('+');
                //如果不是以默认国际出局码 +86 开头 +86
                if (!centerNumber.StartsWith("86"))
                {
                    centerNumber = centerNumber.Insert(0, "86");
                }
                centerNumber = ParityChange(centerNumber);
                centerNumber = centerNumber.Insert(0, "91"); //添加 SMSC地址格式
                string length = (centerNumber.Length / 2).ToString("X2");
                centerNumber = centerNumber.Insert(0, length);//添加 号码长度
            }
            return centerNumber;
        }
        /// <summary>
        /// 接收方地址编码
        /// </summary>
        /// <param name="phoneNumber">手机号码(2-12个8位组)</param>
        /// <returns></returns>
        public string EncodeDestinationNumber(string phoneNumber)
        {
            if (!String.IsNullOrEmpty(phoneNumber))
            {
                phoneNumber = phoneNumber.TrimStart('+');
                if (phoneNumber.StartsWith("86"))
                {
                    //不要包含 86
                    phoneNumber = phoneNumber.TrimStart('8','6');
                }
                //目标地址数字个数 共13个十进制数(不包括91和‘F’)
                string length = (phoneNumber.Length).ToString("X2");
                phoneNumber = ParityChange(phoneNumber);
                //目标地址格式(TON/NPI) 用国际格式号码(在前面加‘+’) A1 ?
                phoneNumber = String.Concat(length, "A1", phoneNumber);
            }
            return phoneNumber;
        }

        /// <summary>
        /// 奇偶互换并补F
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ParityChange(string value)
        {
            string result = String.Empty;
            int length = value.Length;
            for (int i = 1; i < length; i += 2) //奇偶互换
            {
                result += value[i];
                result += value[i - 1];
            }
            if (length % 2 != 0) //不是偶数则加上F，与最后一位互换
            {
                result += 'F';
                result += result[length - 1];
            }
            return result;
        }

        /// <summary>
        /// 用于信息内容的编码
        /// 用户数据(0-140个8位组)
        /// </summary>
        /// <param name="content">信息内容</param>
        /// <param name="encodingCharSet">编码使用的方案</param>
        /// <returns>PDU</returns>
        public string EncodeUserMessage(string content, string encodingCharSet)
        {
            string userData,userDataLenghth = "00";
            switch (encodingCharSet)
            {
                case "08":
                    //Unicode
                    userData = Core.ConvertEx.ToUcs2(content);
                    userDataLenghth = (userData.Length / 2).ToString("X2");
                    break;
                case "18":
                    //8Bit
                    userData = Core.ConvertEx.ToBit8String(content);
                    userDataLenghth = (userData.Length / 2).ToString("X2");
                    break;
                case "00":
                    //7Bit
                    userData = Core.ConvertEx.ToBit7String(content);
                    userDataLenghth = content.Length.ToString("X2");
                    break;
                default:
                    //Default
                    userData = Core.ConvertEx.ToBit7String(content);
                    break;
            }
            
            return userDataLenghth + userData;

        }
        #endregion

        public static string Encoding(string center, string phone, string content, out int length)
        {
            PduEncoder pduEncoding = new PduEncoder();
            return pduEncoding.EncodingSms(center, phone, content, out length);
        }
        public string EncodingSms(string center, string phone, string content, out int length)
        {
            string centerNumber = EncodeCenterNumber(center);
            string phoneNumber = EncodeDestinationNumber(phone);
            const string encodingCharSet = EncodingCharSet.Usc2;
            string userMessage = EncodeUserMessage(content, encodingCharSet);

            string retValue = string.Concat(
                centerNumber,
                ProtocolDataUnitType.TP_VP,
                MessageReference.TP_MR,
                phoneNumber,
                ProtocolIdentifer.TP_PID,
                encodingCharSet,
                ValidityPeriod.Day1,
                userMessage
                );

            string s1 = retValue.Substring(0, 2);
            int i1 = System.Convert.ToInt32(s1, 16);
            i1 = retValue.Length - i1 * 2 - 2;
            length = i1 / 2;  //计算长度
            return retValue;
        }
    }
}