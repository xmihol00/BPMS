using Xunit;
using BPMS_DAL;
using BPMS_Tests.Factories;
using System;
using BPMS_BL.Facades;
using BPMS_DAL.Repositories;
using BPMS_DTOs.Model;
using System.Threading.Tasks;
using System.IO;
using BPMS_DAL.Entities;
using BPMS_BL.Helpers;
using BPMS_DTOs.Service;
using BPMS_Common.Enums;
using BPMS_Common.Helpers;
using System.Text;
using BPMS_DTOs.Account;

namespace BPMS_Tests.Tests
{
    public class SecurityTests
    {
        [Fact]
        public async Task NewKey()
        {
            byte[] key = await SymetricCryptoHelper.NewKey();
            Assert.NotNull(key);
            Assert.True(key.Length == 48);
        }

        [Fact]
        public async Task Encrypt()
        {
            byte[] key = await SymetricCryptoHelper.EncryptKey(Encoding.UTF8.GetBytes("12345678901234561234567890123456"));
            Assert.NotNull(key);
            Assert.True(key.Length == 48);
        }

        [Fact]
        public async Task Decrypt()
        {
            byte[] key = await SymetricCryptoHelper.DecryptKey(await SymetricCryptoHelper.EncryptKey());
            Assert.NotNull(key);
            Assert.True(key.Length == 32);
        }

        [Fact]
        public async Task EncryptDecrypt()
        {
            byte[] originalKey = Encoding.UTF8.GetBytes("12345678901234561234567890123456");
            byte[] key = await SymetricCryptoHelper.DecryptKey(await SymetricCryptoHelper.EncryptKey(originalKey));
            Assert.NotNull(key);
            Assert.Equal(originalKey, key);
        }

        [Fact]
        public async Task EncryptMessage()
        {
            string message = await SymetricCryptoHelper.EncryptMessage("my message", new AddressDTO { PayloadKey = await SymetricCryptoHelper.NewKey(), PayloadIV = new byte[16]});
            Assert.NotEmpty(message);
        }

        [Fact]
        public async Task DecryptMessage()
        {
            AddressDTO dto = new AddressDTO();
            string message = "my message";
            string encrypted = await SymetricCryptoHelper.EncryptMessage(message, dto);
            string result = await SymetricCryptoHelper.DecryptMessage(encrypted, dto.PayloadKey ?? new byte[0], dto.PayloadIV ?? new byte[0]);
            Assert.Equal(message, result);
        }

        [Fact]
        public async Task EncryptSecret()
        {
            byte[] secret = await SymetricCryptoHelper.EncryptSecret("my secret");
            Assert.NotNull(secret);
        }

        [Fact]
        public async Task DecryptSecret()
        {
            AddressDTO dto = new AddressDTO();
            string secret = "my secret";
            byte[] encrypted = await SymetricCryptoHelper.EncryptSecret(secret);
            string result = await SymetricCryptoHelper.DecryptSecret(encrypted);
            Assert.Equal(secret, result);
        }

        [Fact]
        public async Task EncryptAuth()
        {
            AddressDTO dto = new AddressDTO()
            {
                Encryption = EncryptionLevelEnum.Audit,
                MessageId = Guid.NewGuid(),
                Key = await SymetricCryptoHelper.NewKey(),
                SystemId = Guid.NewGuid()
            };
            string auth = await SymetricCryptoHelper.AuthEncrypt(dto);
            Assert.NotEmpty(auth);
        }

        [Fact]
        public async Task DecryptAuth()
        {
            AddressDTO dto = new AddressDTO()
            {
                Encryption = EncryptionLevelEnum.Audit,
                MessageId = Guid.NewGuid(),
                Key = await SymetricCryptoHelper.NewKey(),
                SystemId = Guid.NewGuid(),
            };
            
            
            string auth = await SymetricCryptoHelper.AuthEncrypt(dto);
            AddressDTO? result = await SymetricCryptoHelper.AuthDecrypt<AddressDTO>(SymetricCryptoHelper.ExtractGuid(auth).Item2, dto.Key);
            Assert.Equal(dto.MessageId, result?.MessageId);
        }

        [Fact]
        public void HashMessage()
        {
            byte[] hash = SymetricCryptoHelper.HashMessage("test", Guid.NewGuid());
            Assert.NotEmpty(hash);
        }

        [Fact]
        public void HashCompareSame()
        {
            Guid id = Guid.NewGuid();
            byte[] hash1 = SymetricCryptoHelper.HashMessage("test message", id);
            byte[] hash2 = SymetricCryptoHelper.HashMessage("test message", id);
            Assert.True(SymetricCryptoHelper.ArraysMatch(hash1, hash2));
        }

        [Fact]
        public void HashCompareDifferent()
        {
            Guid id = Guid.NewGuid();
            byte[] hash1 = SymetricCryptoHelper.HashMessage("test message1", id);
            byte[] hash2 = SymetricCryptoHelper.HashMessage("test message2", id);
            Assert.False(SymetricCryptoHelper.ArraysMatch(hash1, hash2));
        }
    }
}