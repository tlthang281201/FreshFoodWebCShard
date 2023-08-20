
using System.Security.Cryptography;
using System.Text;

namespace ShoeShop.Exten 
{
    public static class HashMD5
    {
        public static string CreateMD5(this string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sbHash = new StringBuilder();
                foreach(byte b in hashBytes)
                {
                    sbHash.Append(String.Format("{0:x2}", b));
                }
                return sbHash.ToString();
            }
        }
    }
}
