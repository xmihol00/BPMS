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

namespace BPMS_Tests.Tests
{
    public class SerilizationDeserilizationTests
    {
        [Fact]
        public async Task EmptySerilization1()
        {
            ServiceRequestDTO service = ServiceRequestDTOFactory.Create(SerializationEnum.JSON, HttpMethodEnum.POST, ServiceAuthEnum.None, "http://192.168.10.5/");
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("POST / HTTP/1.1", result);
            Assert.Contains("Host: 192.168.10.5", result);
            Assert.Contains("Content-Type: application/json", result);
        }

        [Fact]
        public async Task EmptySerilization2()
        {
            ServiceRequestDTO service = ServiceRequestDTOFactory.Create(SerializationEnum.URL, HttpMethodEnum.GET, ServiceAuthEnum.None, "http://192.168.10.15/Data/Test");
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("GET /Data/Test HTTP/1.1", result);
            Assert.Contains("Host: 192.168.10.15", result);
            Assert.Contains("Content-Type: application/x-www-form-urlencoded", result);
        }

        [Fact]
        public async Task EmptySerilization3()
        {
            string appId = "username";
            string appSecret = "secret";
            ServiceRequestDTO service = ServiceRequestDTOFactory.Create(SerializationEnum.XMLMarks, HttpMethodEnum.POST, ServiceAuthEnum.Basic, "http://192.168.10.15:12345/Path",
                                                                        await SymetricCryptoHelper.EncryptSecret(appId), await SymetricCryptoHelper.EncryptSecret(appSecret));
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("POST /Path HTTP/1.1", result);
            Assert.Contains("Host: 192.168.10.15:12345", result);
            Assert.Contains("Content-Type: text/xml", result);
            Assert.Contains("Authorization: Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(appId + ":" + appSecret)), result);
        }
    }
}
