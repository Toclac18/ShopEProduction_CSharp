using System;
using System.Security.Cryptography;
using System.Text;

namespace ShopEProduction.Security.Password
{
    public class PasswordPBKDF2
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 10000;

        public byte[] GenerateSalt()
        {
            byte[] salt = new byte[SaltSize];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public string HashPasswordPBKDF2(string password, byte[] salt)
        {
            using (var deriveBytes = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(password), salt, Iterations, HashAlgorithmName.SHA256))
            {
                byte[] hash = deriveBytes.GetBytes(KeySize);
                return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
            }
        }

        public bool VerifyPasswordPBKDF2(string password, string storedHash)
        {
            string[] parts = storedHash.Split(':');
            if (parts.Length != 2)
            {
                return false;
            }

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] storedHashBytes = Convert.FromBase64String(parts[1]);

            using (var deriveBytes = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(password), salt, Iterations, HashAlgorithmName.SHA256))
            {
                byte[] newHash = deriveBytes.GetBytes(KeySize);

                if (newHash.Length != storedHashBytes.Length)
                {
                    return false;
                }

                return CryptographicOperations.FixedTimeEquals(newHash, storedHashBytes);
            }
        }

    }
}
