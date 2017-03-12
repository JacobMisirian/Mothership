using System;
using System.IO;
using System.Security.Cryptography;

namespace Mothership.Crypto
{
    public class AES
    {
        public static byte[] Encrypt(byte[] key, byte[] iv, byte[] data)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memStream, new RijndaelManaged().CreateEncryptor(key, iv), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length);
                }

                return memStream.ToArray();
            }
        }

        public static byte[] Decrypt(byte[] key, byte[] iv, byte[] data)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memStream, new RijndaelManaged().CreateDecryptor(key, iv), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length);
                }

                return memStream.ToArray();
            }
        }

        public static byte[] Generate16ByteArrayFromSeed(int seed)
        {
            Random rnd = new Random(seed);
            byte[] data = new byte[16];
            rnd.NextBytes(data);

            return data;
        }
    }
}
