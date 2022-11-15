using System;
using System.Linq;
using System.Text;

namespace GRP.Web.Helpers
{
    public static class GRPEncoding
    {
        public static string EncodeString(string InputString)
        {
            string encStr = Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(InputString));

            encStr = encStr.Replace("=", string.Empty);
            encStr = encStr.Replace("+", "-");
            encStr = encStr.Replace("/", "_");

            return new string(encStr.Reverse().ToArray());
        }

        public static string DecodeString(string EncodedString)
        {
            string decStr = new string(EncodedString.Reverse().ToArray());

            decStr = decStr.Replace("-", "+");
            decStr = decStr.Replace("_", "/");
            switch (decStr.Length % 4)
            {
                case 0: break;                  // No padding
                case 2: decStr += "=="; break;  // Two chars padding
                case 3: decStr += "="; break;   // One char padding
                default: throw new Exception("Invalid encoded string!");
            }

            return UTF8Encoding.UTF8.GetString(Convert.FromBase64String(decStr));
        }
    }
}