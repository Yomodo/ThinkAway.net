using System.Security.Cryptography;

namespace ThinkAway.Security
{
    public class SHA1
    {

        public byte[] Hash(byte[] data)
        {
            System.Security.Cryptography.SHA1 sha1 = new SHA1CryptoServiceProvider();
            return sha1.ComputeHash(data);
        }
    }
}
