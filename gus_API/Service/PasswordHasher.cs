using System.Security.Cryptography;

namespace gus_API.Service
{
    public class PasswordHasher
    {
        private const int SaltSize = 16;

        private const int HashSize = 32;

        private const int Iteration = 10;

        public static string HashPassword(string password, out string salt)
        {
            byte[] saltBytes = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            salt = Convert.ToBase64String(saltBytes);

            using (var rfc2998 = new Rfc2898DeriveBytes(password, saltBytes, Iteration, HashAlgorithmName.SHA256))
            {
                byte[] hash = rfc2998.GetBytes(HashSize);
                return Convert.ToBase64String(hash);
            }

        }

        public static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            byte[] saltBytes = Convert.FromBase64String(storedSalt);

            using (var rfc2898 = new Rfc2898DeriveBytes(password, saltBytes, Iteration, HashAlgorithmName.SHA256))
            {
                byte[] hash = rfc2898.GetBytes(HashSize);
                return Convert.ToBase64String(hash) == storedHash;
            }
        }


    }
}
