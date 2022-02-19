

using System.Security.Cryptography;
using System.Text;
using BPMS_Common;
using Newtonsoft.Json;

namespace BPMS_Common.Helpers
{
    public static class PasswordHelper
    {
        private const int _hashedPwdSize = 66;
        private const int _saltSize = 33;
        private const int _hashPwdSize = 33;

        public static string HashPassword(string password)
        {
            if (String.IsNullOrEmpty(password))
            {
                throw new NullReferenceException();
            }

            byte[] salt = RandomNumberGenerator.GetBytes(_saltSize);

            byte[] hashPwd = new Rfc2898DeriveBytes(password, salt, 10000).GetBytes(_hashPwdSize);

            byte[] pwdToStore = new byte[_hashedPwdSize];
            for (int i = 0, j = 0; i < _hashedPwdSize - 1; i += 2, j++)
            {
                pwdToStore[i] = hashPwd[j];
                pwdToStore[i + 1] = salt[j];
            }

            return Convert.ToBase64String(pwdToStore);
        }

        public static bool Authenticate(string databasePassword, string password)
        {
            byte[] databaseHash = Convert.FromBase64String(databasePassword);
            
            byte[] salt = new byte[_saltSize];
            byte[] hash = new byte[_hashPwdSize];
            for (int i = 0, j = 0; i < _hashedPwdSize - 1 && i < databaseHash.Length; i += 2, j++)
            {
                hash[j] = databaseHash[i];
                salt[j] = databaseHash[i + 1];
            }

            byte[] hashCurrentPwd = new Rfc2898DeriveBytes(password, salt, 10000).GetBytes(_hashPwdSize);
            for (int i = 0; i < _hashPwdSize; i++)
            {
                if (hash[i] != hashCurrentPwd[i])
                {
                    return false;
                }
            }

            return true;
        }    
    }
}
