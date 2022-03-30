
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
        private const int _hashCycleCount = 1000;
        private static readonly byte[] _thisKey = new byte[_keyByteSize] { 0xd4, 0x28, 0x60, 0x8a, 0xfe, 0xa3, 0x27, 0xf3, 0xca, 0x99, 0x6c, 0x4b, 0x11, 0x88, 0x39, 0xe5, 0xc0, 0x5c, 0xf4, 0x7b, 0xb4, 0xf8, 0xbb, 0x80, 0x31, 0x25, 0x57, 0x29, 0x25, 0xeb, 0x17, 0x5f };
        private static readonly byte[] _thisIV = new byte[_IVByteSize] { 0x70, 0x15, 0x92, 0x40, 0x6a, 0xef, 0x68, 0x8c, 0x02, 0x1f, 0x5e, 0xc8, 0xeb, 0x0c, 0x6f, 0x35 };
        private static readonly byte[] _keyKey = new byte[_keyByteSize] { 0xea, 0x31, 0xdc, 0xb2, 0x02, 0x05, 0x38, 0x51, 0xf1, 0x52, 0x7a, 0xc8, 0x7c, 0x27, 0x5e, 0x85, 0x76, 0x87, 0x0f, 0x6e, 0xa2, 0xac, 0x4a, 0xdd, 0xa6, 0x16, 0x96, 0xd9, 0xb3, 0x60, 0xdc, 0x33 }; 
        private static readonly byte[] _keyIV = new byte[_IVByteSize] { 0xd3, 0xd6, 0xcf, 0x42, 0x18, 0xc3, 0xc6, 0x91, 0x53, 0x3d, 0x3e, 0xc0, 0x54, 0xbd, 0xd4, 0xc6 };
        private static readonly byte[] _secretKey = new byte[_keyByteSize] { 0xef, 0xb7, 0x3b, 0xeb, 0x07, 0xf5, 0x1c, 0x59, 0xef, 0x8e, 0x0f, 0x17, 0xd3, 0x9a, 0xcb, 0x25, 0xc7, 0x8e, 0xdd, 0xcb, 0xcc, 0x1a, 0x60, 0xca, 0xfa, 0xb2, 0x9c, 0xa5, 0x70, 0xa1, 0xc3, 0xb4 };
        private static readonly byte[] _secretIV = new byte [_IVByteSize] { 0xb3, 0xdf, 0x3a, 0xc2, 0xd9, 0x01, 0xbb, 0x06, 0x11, 0xf9, 0x75, 0x33, 0x5d, 0x8c, 0xa9, 0x0c };

        public static async Task<string> AuthEncrypt(IAuth data)
        {
            using Aes aes = Aes.Create();
            if (data.Key == null)
            {
                throw new Exception();
            }
            
            ICryptoTransform encryptor = aes.CreateEncryptor(await DecryptKey(data.Key), _thisIV);

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
            key = await DecryptKey(key);
            ICryptoTransform decryptor = aes.CreateDecryptor(key, _thisIV);
            
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

        public static async Task<string> EncryptMessage(string payload, IAuth auth)
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

        public static async Task<byte[]> EncryptSecret(string secret)
        {
            using Aes aes = Aes.Create();
            ICryptoTransform encryptor = aes.CreateEncryptor(_secretKey, _secretIV);
            
            using (MemoryStream memStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                    {
                        await streamWriter.WriteAsync(secret);
                    }
                }

                return memStream.ToArray();
            }
        }

        public static Task<string> DecryptSecret(byte[] cipherArray)
        {
            using Aes aes = Aes.Create();
            ICryptoTransform decryptor = aes.CreateDecryptor(_secretKey, _secretIV);
            
            using (MemoryStream memStream = new MemoryStream(cipherArray))
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
            
            using (MemoryStream memStream = new MemoryStream())
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

        public static byte[] HashMessage(string message, Guid id)
        {
            return new Rfc2898DeriveBytes(message, id.ToByteArray(), _hashCycleCount).GetBytes(_keyByteSize);
        }

        public static bool ArraysMatch(ReadOnlySpan<byte> array1, ReadOnlySpan<byte> array2)
        {
            return array1.SequenceEqual(array2);
        }
    }
}
