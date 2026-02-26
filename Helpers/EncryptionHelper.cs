using System.Security.Cryptography;
using System.Text;

namespace NationalCardBookingSystemWithoutCleanArch.Helpers
{
    public static class EncryptionHelper
    {
        private static readonly string Key = "1234567890123456"; // 16 char
        private static readonly string IV = "1234567890123456";  // 16 char

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(Key);
            aes.IV = Encoding.UTF8.GetBytes(IV);

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            var buffer = Encoding.UTF8.GetBytes(plainText);

            var cipher = encryptor.TransformFinalBlock(buffer, 0, buffer.Length);
            return Convert.ToBase64String(cipher);
        }

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(Key);
            aes.IV = Encoding.UTF8.GetBytes(IV);

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            var buffer = Convert.FromBase64String(cipherText);

            var plain = decryptor.TransformFinalBlock(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(plain);
        }
    }

}
