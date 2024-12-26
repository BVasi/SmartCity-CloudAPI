using System.Security.Cryptography;
using System.Text;

namespace utils;

public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashedBytes = sha256.ComputeHash(passwordBytes);
            return Convert.ToBase64String(hashedBytes);
        }
    }

    public static bool CheckPassword(string hashedPassword, string passwordToCheck)
    {
        string computedHash = HashPassword(passwordToCheck);
        return computedHash == hashedPassword;
    }
}