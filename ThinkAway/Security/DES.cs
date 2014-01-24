using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ThinkAway.Security
{
    public class DES : EncryptClass
    {
        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="data"> </param>
        /// <param name="rgbKey"></param>
        /// <returns></returns>
        public override byte[] Encrypt(byte[] data, byte[] rgbKey)
        {
            DESCryptoServiceProvider desCryptoServiceProvider = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream();
            ICryptoTransform transform = desCryptoServiceProvider.CreateEncryptor(rgbKey, IV);
            const CryptoStreamMode mode = CryptoStreamMode.Write;
            CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, mode);
            cryptoStream.Write(data, 0, data.Length);
            cryptoStream.FlushFinalBlock();
            return memoryStream.ToArray();
        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="data"> </param>
        /// <param name="rgbKey"> </param>
        /// <returns></returns>
        public override byte[] Decrypt(byte[] data, byte[] rgbKey)
        {
            DESCryptoServiceProvider desCryptoServiceProvider = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream();
            ICryptoTransform transform = desCryptoServiceProvider.CreateDecryptor(rgbKey, IV);
            const CryptoStreamMode mode = CryptoStreamMode.Write;
            CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, mode);
            cryptoStream.Write(data, 0, data.Length);
            cryptoStream.FlushFinalBlock();
            return memoryStream.ToArray();
           
        }
    }
}
