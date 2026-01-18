using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using BCrypt.Net;

namespace kurs4v28
{
    public class Pass
    {
        public static string HashPasswordWithBcrypt(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, 12); 
        }
        public static bool VerifyBcryptPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
