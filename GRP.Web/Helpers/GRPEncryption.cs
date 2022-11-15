using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GRP.Web.Helpers
{
    public static class GRPEncryption
    {
        public static string EncryptString(string text, string key)
        {
            try
            {
                byte[] baSrcaSalt = Encoding.UTF8.GetBytes("srca_217");
                byte[] baPwd = Encoding.UTF8.GetBytes(key);
                byte[] baPwdHash = SHA256Managed.Create().ComputeHash(baPwd);
                byte[] baText = Encoding.UTF8.GetBytes(text);

                byte[] baSalt = GetRandomBytes(8);
                byte[] baEncrypted = new byte[baSalt.Length + baText.Length];

                // Salt + Text
                for (int i = 0; i < baSalt.Length; i++)
                    baEncrypted[i] = baSalt[i];
                for (int i = 0; i < baText.Length; i++)
                    baEncrypted[i + baSalt.Length] = baText[i];

                baEncrypted = AES_Encrypt(baEncrypted, baPwdHash, baSrcaSalt);

                return Convert.ToBase64String(baEncrypted);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        private static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes, byte[] saltBytes)
        {
            byte[] encryptedBytes = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        public static string DecryptText(string input, string key)
        {
            try
            {
                byte[] baSrcaSalt = Encoding.UTF8.GetBytes("srca_217");
                byte[] baPwd = Encoding.UTF8.GetBytes(key);
                byte[] baPwdHash = SHA256Managed.Create().ComputeHash(baPwd);

                byte[] baInput = Convert.FromBase64String(input);
                byte[] baDecrypted = AES_Decrypt(baInput, baPwdHash, baSrcaSalt);

                // Remove salt
                int saltLength = 8;
                byte[] baResult = new byte[baDecrypted.Length - saltLength];
                for (int i = 0; i < baResult.Length; i++)
                    baResult[i] = baDecrypted[i + saltLength];

                return Encoding.UTF8.GetString(baResult);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        private static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes, byte[] saltBytes)
        {
            byte[] decryptedBytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }

        public static byte[] GetRandomBytes(int BytesLength)
        {
            byte[] ba = new byte[BytesLength];
            RNGCryptoServiceProvider.Create().GetBytes(ba);
            return ba;
        }
    }
}