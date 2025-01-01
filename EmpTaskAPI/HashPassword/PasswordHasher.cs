using System.Security.Cryptography;
using System.Text;

namespace EmpTaskAPI.HashPassword
{
    public class PasswordHasher
    {
        public static string HashPassword(string password, string salt)
        {
            using var sha256 = SHA256.Create();
            var saltedPassword = $"{salt}{password}";
            var bytes = Encoding.UTF8.GetBytes(saltedPassword);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public static string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }
    }
}
