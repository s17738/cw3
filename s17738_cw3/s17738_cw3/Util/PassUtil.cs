using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace s17738_cw3.Util
{
    public class PassUtil
    {
        public static bool ValidatePassword(string password, string salt, string hash)
        {
            return GeneratePasswordHash(password, salt) == hash;
        }

        public static string GeneratePasswordHash(string password, string salt)
        {
            var bytes = KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.UTF8.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);

            return Convert.ToBase64String(bytes);
        }

        public static string GenerateSalt()
        {
            byte[] randomBytes = new byte[128 / 8];
            using (var gen = RandomNumberGenerator.Create())
            {
                gen.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }

        }
    }
}
