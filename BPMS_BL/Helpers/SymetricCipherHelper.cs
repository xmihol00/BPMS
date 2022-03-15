
using System.Security.Cryptography;
using System.Text;
using BPMS_Common;
using BPMS_Common.Interfaces;
using Newtonsoft.Json;

namespace BPMS_BL.Helpers
{
    public static class SymetricCipherHelper
    {
        public static string JsonEncrypt(IAddressAuth data)
        {
            using Aes aes = Aes.Create();
            aes.Key = data.Key;
            aes.IV = StaticData.IV;
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream memStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                    {
                        streamWriter.Write(JsonConvert.SerializeObject(data));
                    }

                    string result = Convert.ToBase64String(memStream.ToArray());
                    string guid = data.SystemId.ToString();
                    result = result.Insert(0, guid.Substring(0, 8));
                    result = result.Insert(32, guid.Substring(9, 14).Replace("-", ""));
                    result += guid.Substring(24);
                    return result;
                }
            }
        }

        public static async Task<T?> JsonDecrypt<T>(string cipherText, byte[] key) where T : class
        {
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = StaticData.IV;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            
            using (MemoryStream memStream = new MemoryStream(Convert.FromBase64String(cipherText)))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader streamReader = new StreamReader(cryptoStream))
                    {
                        return JsonConvert.DeserializeObject<T>(await streamReader.ReadToEndAsync());
                    }
                }
            }
        }

        public static Task<string> Decrypt(Stream cipherStream, byte[] key)
        {
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = StaticData.IV;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            
            using (CryptoStream cryptoStream = new CryptoStream(cipherStream, decryptor, CryptoStreamMode.Read))
            {
                using (StreamReader streamReader = new StreamReader(cryptoStream))
                {
                    return streamReader.ReadToEndAsync();
                }
            }
        }

        public static byte[] NewKey()
        {
            using Aes crypto = Aes.Create();
            crypto.KeySize = StaticData.KeySize;
            crypto.GenerateKey();
            return crypto.Key;
        }

        public static (Guid, string) ExtractGuid(string cipherText)
        {
            int endIndex = cipherText.Length - 12;
            string guid = cipherText.Substring(0, 8);
            guid += "-" + cipherText.Substring(32, 4);
            guid += "-" + cipherText.Substring(36, 4);
            guid += "-" + cipherText.Substring(40, 4);
            guid += "-" + cipherText.Substring(endIndex);

            cipherText = cipherText.Remove(endIndex);
            cipherText = cipherText.Remove(32, 12);
            cipherText = cipherText.Remove(0, 8);

            return (Guid.Parse(guid), cipherText);
        }
    }
}
