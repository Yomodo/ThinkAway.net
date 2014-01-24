using System.IO;
using System.Security.Cryptography;

namespace ThinkAway.Security
{
    public class AES : EncryptClass
    {
        private readonly SymmetricAlgorithm _rijndael;

        public AES()
        {
            //分组加密算法  
            _rijndael = Rijndael.Create();
        }

        /// <summary>
        /// AES加密算法
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public override byte[] Encrypt(byte[] data, byte[] key)
        {
            
            _rijndael.Key = key;
            _rijndael.IV = base.IV;
            MemoryStream memoryStream = new MemoryStream();
            ICryptoTransform transform = _rijndael.CreateEncryptor();
            const CryptoStreamMode mode = CryptoStreamMode.Write;
            CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, mode);
            cryptoStream.Write(data, 0, data.Length);
            cryptoStream.FlushFinalBlock();
            byte[] array = memoryStream.ToArray();
            cryptoStream.Close();
            memoryStream.Close();
            return array;     
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public override byte[] Decrypt(byte[] data, byte[] key)
        {
            _rijndael.Key = key;
            _rijndael.IV = base.IV;
            var bytes = new byte[data.Length];
            MemoryStream memoryStream = new MemoryStream(data);
            ICryptoTransform transform = _rijndael.CreateDecryptor();
            const CryptoStreamMode mode = CryptoStreamMode.Read;
            CryptoStream cs = new CryptoStream(memoryStream, transform, mode);
            cs.Read(bytes, 0, bytes.Length);
            cs.Close();
            memoryStream.Close();
            return bytes;  
        }
        
    }
}
