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
    /// ��һ����������ת��Ϊ��һ������ , �޷��̳д���
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
        /// ����PDU���������
        /// </summary>
        /// <param name="str">PDU���������</param>
        /// <returns>����������</returns>
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
        /// Bit 7 ����
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToBit7String(string text)
        {
            //�洢�м��ַ��� �����ƴ�
            string userData = String.Empty;
            //
            Encoding encodingAsscii = Encoding.ASCII;
            //
            byte[] bytes = encodingAsscii.GetBytes(text);
            //�ߵͽ��� �����ƴ�
            for (int i = text.Length; i > 0; i--)
            {
                string temp = System.Convert.ToString(bytes[i - 1], 2);
                //����7λ������
                while (temp.Length < 7)
                {
                    temp = temp.Insert(0, "0");
                }
                userData += temp;
            }
            return userData;
        }
        /// <summary>
        /// ʮ�����Ƶ��ַ���ת��Ϊbyte����
        /// </summary>
        /// <param name="hexString">ת������</param>
        /// <returns>�����Ĵ�</returns>
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
        /// �����Ͷ������ݽ��б���
        /// ����Big-Endian �ֽ�˳��� Unicode ��ʽ���룬���ߵ�λ����
        /// ��ת����Ķ������ݴ���ֽ�����
        /// ȥ���ڽ���Unicode��ʽ�����У������ֽ��е�"-",���磺00-21�����0021
        /// �������������ݵĳ��ȳ�2��������λ16������
        /// </summary>
        /// <param name="text">��Ϣ����</param>
        /// <returns>������Ϣ�����ĳ���</returns>
        public static string ToUcs2(string text)
        {
            //�Զ������ݽ��б��� 
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
        /// Bit 8 ����
        /// ÿ8λȡλһ���ַ� ����ɱ���
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToBit8String(string text)
        {
            string result = String.Empty;
            //ÿ8λȡλһ���ַ� ����ɱ���
            for (int i = text.Length; i > 0; i -= 8)
            {
                result += System.Convert.ToInt32(i > 8 ? text.Substring(i - 8, 8) : text.Substring(0, i), 2).ToString("X2");
            }
            return result;
        }
    }
}
