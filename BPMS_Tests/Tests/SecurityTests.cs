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
    }
}