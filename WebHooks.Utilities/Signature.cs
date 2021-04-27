using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WebHooks.Utilities
{
    public static class Signature
    {
        public static string GenerateSignature(byte[] data, string key)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var hash = ComputeHash(data, keyBytes);
            var hexHash = GenerateHexString(hash);

            return "sha256=" + hexHash;
        }

        private static byte[] ComputeHash(byte[] data, byte[] key)
        {
            using var hmac = new HMACSHA256(key);
            byte[] computedHash = hmac.ComputeHash(data);

            return computedHash;
        }

        private static string GenerateHexString(byte[] data)
        {
            var hexString = string.Concat(data
                .Select(t => t.ToString("X2")));

            return hexString;
        }
    }
}
