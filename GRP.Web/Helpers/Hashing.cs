using System;
using System.Security.Cryptography;
using System.Text;

namespace GrpSample.Web.Helpers
{
    public class Hashing
    {
        /// <summary>
        /// Utility function to generate a MD5 of a string
        /// </summary>
        /// <param name="value">The item to have a MD5 generated for it</param>
        /// <returns>The MD5 digest</returns>
        public static string GetHashMD5OfString(string value)
        {
            using (var cryptoProvider = new MD5CryptoServiceProvider())
            {
                var hash = cryptoProvider.ComputeHash(Encoding.UTF8.GetBytes(value));
                return Convert.ToBase64String(hash);
            }
        }

        /// <summary>
        /// Utility to generate a HMAC of a string
        /// </summary>
        /// <param name="value">The item to have a HMAC generated for it</param>
        /// <param name="key">The 'shared' key to use for the HMAC</param>
        /// <returns>The HMAC for the value using the key</returns>
        public static string GetHashHMACSHA256OfString(string value, string key)
        {
            using (var cryptoProvider = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                var hash = cryptoProvider.ComputeHash(Encoding.UTF8.GetBytes(value));
                return Convert.ToBase64String(hash);
            }
        }

    }
}