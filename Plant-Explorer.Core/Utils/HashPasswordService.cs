﻿using System.Security.Cryptography;
using System.Text;

namespace Plant_Explorer.Core.Utils
{
    public static class HashPasswordService
    {
        public static string HashPasswordTwice(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var firstHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var secondHash = sha256.ComputeHash(firstHash);
                return Convert.ToBase64String(secondHash);
            }
        }

        public static string HashPasswordBcrypt(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPasswordBcrypt(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
