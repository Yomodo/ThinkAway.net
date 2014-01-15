namespace ThinkAway.Security
{
    public abstract class EncryptClass
    {
        /// <summary>
        /// 获取或设置对称加密算法的默认向量
        /// </summary>
        public virtual byte[] IV { get; set; }

        /// <summary>
        /// 根据字符串生成 对称加密算法 密钥向量
        /// </summary>
        /// <returns></returns>
        public byte[] InitIV(string key,int length)
        {
            byte[] rgbKey = new byte[length];
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(key);
            for (int i = 0; i < rgbKey.Length; i++)
            {
                if (i < bytes.Length)
                {
                    rgbKey[i] = bytes[i];
                }
            }
            return rgbKey;
        }

        /// <summary>
        /// 使用指定的 机密密钥 对数据进行加密运算
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract byte[] Encrypt(byte[] data, byte[] key);
        /// <summary>
        /// 使用指定的 机密密钥 对数据进行解运算
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract byte[] Decrypt(byte[] data, byte[] key);

        
    }
}
