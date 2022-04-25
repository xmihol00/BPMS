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
            ServiceRequestDTO service = ServiceRequestDTOFactory.CreateEmptyTree(SerializationEnum.JSON, HttpMethodEnum.POST, ServiceAuthEnum.None, "http://192.168.10.5/");
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("POST / HTTP/1.1", result);
            Assert.Contains("Host: 192.168.10.5", result);
            Assert.Contains("Content-Type: application/json", result);
        }

        [Fact]
        public async Task EmptySerilization2()
        {
            ServiceRequestDTO service = ServiceRequestDTOFactory.CreateEmptyTree(SerializationEnum.URL, HttpMethodEnum.GET, ServiceAuthEnum.None, "http://192.168.10.15/Data/Test");
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
            ServiceRequestDTO service = ServiceRequestDTOFactory.CreateEmptyTree(SerializationEnum.XMLMarks, HttpMethodEnum.POST, ServiceAuthEnum.Basic, "http://192.168.10.15:12345/Path",
                                                                        await SymetricCryptoHelper.EncryptSecret(appId), await SymetricCryptoHelper.EncryptSecret(appSecret));
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("POST /Path HTTP/1.1", result);
            Assert.Contains("Host: 192.168.10.15:12345", result);
            Assert.Contains("Content-Type: text/xml", result);
            Assert.Contains("Authorization: Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(appId + ":" + appSecret)), result);
        }

        public async Task EmptySerilization4()
        {
            string appSecret = "API KEY 784";
            ServiceRequestDTO service = ServiceRequestDTOFactory.CreateEmptyTree(SerializationEnum.XMLMarks, HttpMethodEnum.POST, ServiceAuthEnum.Bearer, "http://192.168.10.15:12345/Path",
                                                                                 null, await SymetricCryptoHelper.EncryptSecret(appSecret));
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("POST /Path HTTP/1.1", result);
            Assert.Contains("Host: 192.168.10.15:12345", result);
            Assert.Contains("Content-Type: text/xml", result);
            Assert.Contains("Authorization: Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(appSecret)), result);
        }

        [Fact]
        public async Task Serilization1()
        {
            ServiceRequestDTO service = ServiceRequestDTOFactory.CreateTree(SerializationEnum.JSON, HttpMethodEnum.POST, ServiceAuthEnum.None, "http://192.168.10.15:12345/Path");
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("\"alias1\": \"Hello\"", result);
        }

        [Fact]
        public async Task Serilization2()
        {
            ServiceRequestDTO service = ServiceRequestDTOFactory.CreateTree(SerializationEnum.JSON, HttpMethodEnum.POST, ServiceAuthEnum.None, "http://192.168.10.15:12345/Path");
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("\"Object1\": {},", result);
        }

        [Fact]
        public async Task Serilization3()
        {
            ServiceRequestDTO service = ServiceRequestDTOFactory.CreateTree(SerializationEnum.JSON, HttpMethodEnum.POST, ServiceAuthEnum.None, "http://192.168.10.15:12345/Path");
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("\n  \"object2\": {\n    \"integer\": 42\n  }", result);
        }

        [Fact]
        public async Task Serilization4()
        {
            ServiceRequestDTO service = ServiceRequestDTOFactory.CreateTree(SerializationEnum.JSON, HttpMethodEnum.POST, ServiceAuthEnum.None, "http://192.168.10.15:12345/Path");
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("\n  \"obj3\": {\n    \"boolean\": true,\n    \"double\": 85.36\n  }", result);
        }

        [Fact]
        public async Task Serilization5()
        {
            ServiceRequestDTO service = ServiceRequestDTOFactory.CreateTree(SerializationEnum.JSON, HttpMethodEnum.POST, ServiceAuthEnum.None, "http://192.168.10.15/");
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("\n  \"emptyArray\": [],\n", result);
        }

        [Fact]
        public async Task Serilization6()
        {
            ServiceRequestDTO service = ServiceRequestDTOFactory.CreateTree(SerializationEnum.JSON, HttpMethodEnum.POST, ServiceAuthEnum.None, "http://192.168.10.15/");
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("\n  \"stringArray\": [\n    \"str1\",\n    \"str2\",\n    \"str3\"\n  ],\n", result);
        }

        [Fact]
        public async Task Serilization7()
        {
            ServiceRequestDTO service = ServiceRequestDTOFactory.CreateTree(SerializationEnum.JSON, HttpMethodEnum.POST, ServiceAuthEnum.None, "http://192.168.10.15/");
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("\n  \"intArray\": [\n    10,\n    22.22,\n    0\n  ],\n", result);
        }

        [Fact]
        public async Task Serilization8()
        {
            ServiceRequestDTO service = ServiceRequestDTOFactory.CreateTree(SerializationEnum.JSON, HttpMethodEnum.POST, ServiceAuthEnum.None, "http://192.168.10.15/");
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("\n  \"boolArray\": [\n    false,\n    true\n  ]", result);
        }

        [Fact]
        public async Task Serilization9()
        {
            ServiceRequestDTO service = ServiceRequestDTOFactory.CreateTree(SerializationEnum.JSON, HttpMethodEnum.POST, ServiceAuthEnum.None, "http://192.168.10.15:12345/Path");
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("\n  \"objLevel1\": {\n    \"objLevel2\": {\n      \"num\": 255\n    }\n  }", result);
        }

        [Fact]
        public async Task Serilization11()
        {
            ServiceRequestDTO service = ServiceRequestDTOFactory.CreateTree(SerializationEnum.XMLMarks, HttpMethodEnum.POST, ServiceAuthEnum.None, "http://192.168.10.15:12345/Path");
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("<alias1>Hello</alias1>", result);
        }

        [Fact]
        public async Task Serilization12()
        {
            ServiceRequestDTO service = ServiceRequestDTOFactory.CreateTree(SerializationEnum.XMLMarks, HttpMethodEnum.POST, ServiceAuthEnum.None, "http://192.168.10.15:12345/Path");
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("<Object1></Object1>", result);
        }
        
        [Fact]
        public async Task Serilization13()
        {
            ServiceRequestDTO service = ServiceRequestDTOFactory.CreateTree(SerializationEnum.XMLMarks, HttpMethodEnum.POST, ServiceAuthEnum.None, "http://192.168.10.15:12345/Path");
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("\n  <object2>\n    <integer>42</integer>\n  </object2>", result);
        }
        
        [Fact]
        public async Task Serilization14()
        {
            ServiceRequestDTO service = ServiceRequestDTOFactory.CreateTree(SerializationEnum.XMLMarks, HttpMethodEnum.POST, ServiceAuthEnum.None, "http://192.168.10.15:12345/Path");
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("\n  <obj3>\n    <boolean>true</boolean>\n    <double>85.36</double>\n  </obj3>", result);
        }
        
        [Fact]
        public async Task Serilization15()
        {
            ServiceRequestDTO service = ServiceRequestDTOFactory.CreateTree(SerializationEnum.XMLMarks, HttpMethodEnum.POST, ServiceAuthEnum.None, "http://192.168.10.15/");
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("\n  <emptyArray></emptyArray>\n", result);
        }
         
        [Fact]
        public async Task Serilization16()
        {
            ServiceRequestDTO service = ServiceRequestDTOFactory.CreateTree(SerializationEnum.XMLMarks, HttpMethodEnum.POST, ServiceAuthEnum.None, "http://192.168.10.15/");
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("\n  <stringArray>\n    <value>str1</value>\n    <value>str2</value>\n    <value>str3</value>\n  </stringArray>\n", result);
        }
        
        [Fact]
        public async Task Serilization17()
        {
            ServiceRequestDTO service = ServiceRequestDTOFactory.CreateTree(SerializationEnum.XMLMarks, HttpMethodEnum.POST, ServiceAuthEnum.None, "http://192.168.10.15/");
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("\n  <intArray>\n    <value>10</value>\n    <value>22.22</value>\n    <value>0</value>\n  </intArray>\n", result);
        }
        
        [Fact]
        public async Task Serilization18()
        {
            ServiceRequestDTO service = ServiceRequestDTOFactory.CreateTree(SerializationEnum.XMLMarks, HttpMethodEnum.POST, ServiceAuthEnum.None, "http://192.168.10.15/");
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("\n  <boolArray>\n    <value>false</value>\n    <value>true</value>\n  </boolArray>", result);
        }
        
        [Fact]
        public async Task Serilization19()
        {
            ServiceRequestDTO service = ServiceRequestDTOFactory.CreateTree(SerializationEnum.XMLMarks, HttpMethodEnum.POST, ServiceAuthEnum.None, "http://192.168.10.15:12345/Path");
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("\n  <objLevel1>\n    <objLevel2>\n      <num>255</num>\n    </objLevel2>\n  </objLevel1>", result);
        }

        [Fact]
        public async Task Serilization21()
        {
            ServiceRequestDTO service = ServiceRequestDTOFactory.CreateTree(SerializationEnum.XMLMarks, HttpMethodEnum.POST, ServiceAuthEnum.None, "http://192.168.10.15:12345/Path");
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("<alias1>Hello</alias1>", result);
        }

        [Fact]
        public async Task Serilization22()
        {
            ServiceRequestDTO service = ServiceRequestDTOFactory.CreateTree(SerializationEnum.XMLMarks, HttpMethodEnum.POST, ServiceAuthEnum.None, "http://192.168.10.15:12345/Path");
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("<Object1></Object1>", result);
        }
        
        [Fact]
        public async Task Serilization23()
        {
            ServiceRequestDTO service = ServiceRequestDTOFactory.CreateTree(SerializationEnum.XMLMarks, HttpMethodEnum.POST, ServiceAuthEnum.None, "http://192.168.10.15:12345/Path");
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("\n  <object2>\n    <integer>42</integer>\n  </object2>", result);
        }
        
        [Fact]
        public async Task Serilization24()
        {
            ServiceRequestDTO service = ServiceRequestDTOFactory.CreateTree(SerializationEnum.XMLMarks, HttpMethodEnum.POST, ServiceAuthEnum.None, "http://192.168.10.15:12345/Path");
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("\n  <obj3>\n    <boolean>true</boolean>\n    <double>85.36</double>\n  </obj3>", result);
        }
        
        [Fact]
        public async Task Serilization25()
        {
            ServiceRequestDTO service = ServiceRequestDTOFactory.CreateTree(SerializationEnum.XMLMarks, HttpMethodEnum.POST, ServiceAuthEnum.None, "http://192.168.10.15/");
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("\n  <emptyArray></emptyArray>\n", result);
        }
         
        [Fact]
        public async Task Serilization26()
        {
            ServiceRequestDTO service = ServiceRequestDTOFactory.CreateTree(SerializationEnum.XMLMarks, HttpMethodEnum.POST, ServiceAuthEnum.None, "http://192.168.10.15/");
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("\n  <stringArray>\n    <value>str1</value>\n    <value>str2</value>\n    <value>str3</value>\n  </stringArray>\n", result);
        }
        
        [Fact]
        public async Task Serilization27()
        {
            ServiceRequestDTO service = ServiceRequestDTOFactory.CreateTree(SerializationEnum.XMLMarks, HttpMethodEnum.POST, ServiceAuthEnum.None, "http://192.168.10.15/");
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("\n  <intArray>\n    <value>10</value>\n    <value>22.22</value>\n    <value>0</value>\n  </intArray>\n", result);
        }
        
        [Fact]
        public async Task Serilization28()
        {
            ServiceRequestDTO service = ServiceRequestDTOFactory.CreateTree(SerializationEnum.XMLMarks, HttpMethodEnum.POST, ServiceAuthEnum.None, "http://192.168.10.15/");
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("\n  <boolArray>\n    <value>false</value>\n    <value>true</value>\n  </boolArray>", result);
        }
        
        [Fact]
        public async Task Serilization29()
        {
            ServiceRequestDTO service = ServiceRequestDTOFactory.CreateTree(SerializationEnum.XMLMarks, HttpMethodEnum.POST, ServiceAuthEnum.None, "http://192.168.10.15:12345/Path");
            WebServiceHelper serviceHelper = new WebServiceHelper(service);
            string result = await serviceHelper.GenerateRequest();

            Assert.Contains("\n  <objLevel1>\n    <objLevel2>\n      <num>255</num>\n    </objLevel2>\n  </objLevel1>", result);
        }
    }
}
