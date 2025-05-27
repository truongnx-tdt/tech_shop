// "-----------------------------------------------------------------------
//  <copyright file="PasswordHasher.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"
using System.Security.Cryptography;
using System.Text;

namespace TechShop.Manufacture.Utils
{
    public class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            string salt = AppSettings.Salt;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] saltedPasswordBytes = new byte[saltBytes.Length + passwordBytes.Length];
                Buffer.BlockCopy(saltBytes, 0, saltedPasswordBytes, 0, saltBytes.Length);
                Buffer.BlockCopy(passwordBytes, 0, saltedPasswordBytes, saltBytes.Length, passwordBytes.Length);
                byte[] hashedBytes = sha256.ComputeHash(saltedPasswordBytes);
                string hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                return hashedPassword;
            }
        }
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            string hashedInputPassword = HashPassword(password);
            return string.Equals(hashedInputPassword, hashedPassword);
        }
    }
}
