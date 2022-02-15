
using System.Security.Cryptography;
using System.Text;
using BPMS_Common;
using Newtonsoft.Json;

namespace BPMS_BL.Helpers
{
    public static class SymetricCypherHelper
    {
        public static string JsonEncrypt(object data)
        {
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(StaticData.SymetricKey);
            aes.IV = StaticData.IV;
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using MemoryStream memoryStream = new MemoryStream();
            using CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            using StreamWriter streamWriter = new StreamWriter(cryptoStream);

            streamWriter.Write(JsonConvert.SerializeObject(data)); 

            return Convert.ToBase64String(memoryStream.ToArray());
        }

        public static T? JsonDecrypt<T>(string cipherText) where T : class
        {
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(StaticData.SymetricKey);
            aes.IV = StaticData.IV;
            ICryptoTransform decryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(cipherText));
            using CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            using StreamReader streamReader = new StreamReader(cryptoStream);

            return JsonConvert.DeserializeObject<T>(streamReader.ReadToEnd());
        }
    }
}
