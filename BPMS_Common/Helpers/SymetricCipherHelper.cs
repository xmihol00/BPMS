
using System.Security.Cryptography;
using System.Text;
using BPMS_Common;
using BPMS_Common.Interfaces;
using Newtonsoft.Json;

namespace BPMS_Common.Helpers
{
    public static class SymetricCipherHelper
    {
        private const int _keyBitSize = 256;
        private const int _keyByteSize = 32;
        private const int _IVByteSize = 16;
        private static readonly byte[] _thisKey = new byte[_keyByteSize] { 0xd4, 0x28, 0x60, 0x8a, 0xfe, 0xa3, 0x27, 0xf3, 0xca, 0x99, 0x6c, 0x4b, 0x11, 0x88, 0x39, 0xe5, 0xc0, 0x5c, 0xf4, 0x7b, 0xb4, 0xf8, 0xbb, 0x80, 0x31, 0x25, 0x57, 0x29, 0x25, 0xeb, 0x17, 0x5f };
        private static readonly byte[] _thisIV = new byte[_IVByteSize] { 0x70, 0x15, 0x92, 0x40, 0x6a, 0xef, 0x68, 0x8c, 0x02, 0x1f, 0x5e, 0xc8, 0xeb, 0x0c, 0x6f, 0x35 };
        private static readonly byte[] _keyKey = new byte[_keyByteSize] { 0xea, 0x31, 0xdc, 0xb2, 0x02, 0x05, 0x38, 0x51, 0xf1, 0x52, 0x7a, 0xc8, 0x7c, 0x27, 0x5e, 0x85, 0x76, 0x87, 0x0f, 0x6e, 0xa2, 0xac, 0x4a, 0xdd, 0xa6, 0x16, 0x96, 0xd9, 0xb3, 0x60, 0xdc, 0x33 }; 
        private static readonly byte[] _keyIV = new byte[_IVByteSize] { 0xd3, 0xd6, 0xcf, 0x42, 0x18, 0xc3, 0xc6, 0x91, 0x53, 0x3d, 0x3e, 0xc0, 0x54, 0xbd, 0xd4, 0xc6 };

        public static async Task<string> AuthEncrypt(IAddressAuth data)
        {
            using Aes aes = Aes.Create();
            ICryptoTransform encryptor = aes.CreateEncryptor(data.Key != null ? await DecryptKey(data.Key) : _thisKey, _thisIV);

            using (MemoryStream memStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                    {
                        streamWriter.Write(JsonConvert.SerializeObject(data));
                    }

                    string result = Convert.ToBase64String(data.SystemId.ToByteArray().Concat(memStream.ToArray()).ToArray());
                    return result;
                }
            }
        }

        public static async Task<T?> AuthDecrypt<T>(byte[] cipherArray, byte[] key) where T : class
        {
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = _thisIV;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            
            using (MemoryStream memStream = new MemoryStream(cipherArray))
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

        public static Task<string> DecryptMessage(string cipherText, byte[] key, byte[] iv)
        {
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            
            using (MemoryStream memStream = new MemoryStream(Convert.FromBase64String(cipherText)))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader streamReader = new StreamReader(cryptoStream))
                    {
                        return streamReader.ReadToEndAsync();
                    }
                }
            }
        }

        public static async Task<string> EncryptMessage(string payload, IAddressAuth auth)
        {
            using Aes aes = Aes.Create();
            aes.KeySize = _keyBitSize;
            aes.GenerateKey();
            aes.GenerateIV();
            auth.PayloadKey = aes.Key;
            auth.PayloadIV = aes.IV;
            
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            
            using (MemoryStream memStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                    {
                        await streamWriter.WriteAsync(payload);
                    }
                }

                return Convert.ToBase64String(memStream.ToArray());
            }
        }

        public static async Task<byte[]> EncryptKey(byte[]? key = null)
        {
            key = key ?? _thisKey;
            using Aes aes = Aes.Create();
            
            ICryptoTransform encryptor = aes.CreateEncryptor(_keyKey, _keyIV);
            
            using (MemoryStream memStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
                {
                    await cryptoStream.WriteAsync(key, 0, _keyByteSize);
                }

                return memStream.ToArray();
            }
        }

        public static async Task<byte[]> DecryptKey(byte[] key)
        {
            using Aes aes = Aes.Create();
            ICryptoTransform decryptor = aes.CreateDecryptor(_keyKey, _keyIV);
            
            using (MemoryStream memStream = new MemoryStream(key))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Write))
                {
                    await cryptoStream.WriteAsync(key, 0, key.Length);
                    await cryptoStream.FlushFinalBlockAsync();
                }

                return memStream.ToArray();
            }
        }

        public static Task<byte[]> NewKey()
        {
            using Aes crypto = Aes.Create();
            crypto.KeySize = _keyBitSize;
            crypto.GenerateKey();
            return EncryptKey(crypto.Key);
        }

        public static (Guid, byte[]) ExtractGuid(string cipherText)
        {
            byte[] data = Convert.FromBase64String(cipherText);

            return (new Guid(data[0..16]), data[16..]);
        }

        public static bool ArraysMatch(ReadOnlySpan<byte> array1, ReadOnlySpan<byte> array2)
        {
            return array1.SequenceEqual(array2);
        }
    }
}
