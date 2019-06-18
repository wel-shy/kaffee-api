using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;

namespace Kaffee.Providers
{
    public class HashProvider
    {
        public static string GetHash(string plain, string salt)
        {
            var saltBytes = Encoding.Default.GetBytes(salt);
            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: plain,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }

        public static string GetSalt() 
        {
            // generate a 128-bit salt using a secure PRNG
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
                return BitConverter
                    .ToString(salt)
                    .Replace("-", string.Empty);
            }
        }
    }
}