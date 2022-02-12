using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using AutoMapper;
using BPMS_Common.Enums;
using BPMS_Common.Helpers;
using BPMS_DAL.Entities;
using BPMS_DAL.Repositories;
using BPMS_DTOs.Service;
using BPMS_DTOs.ServiceDataSchema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BPMS_BL.Helpers
{
    public class HttpRequestHelper
    {
        private readonly StringBuilder _builder = new StringBuilder();
        private readonly IEnumerable<DataSchemaDataDTO> _data;
        private readonly SerializationEnum _serialization;
        private readonly Uri _url;
        private readonly HttpMethodEnum _method;

        public HttpRequestHelper(ServiceRequestDTO service)
        {
            _data = service.Nodes;
            _serialization = service.Serialization;
            _url = new Uri(service.URL);
            _method = service.HttpMethod;
        }

        public HttpRequestHelper(IEnumerable<DataSchemaDataDTO> data, SerializationEnum serialization, Uri url, HttpMethodEnum method)
        {
            _data = data;
            _serialization = serialization;
            _url = url;
            _method = method;
        }

        public string GenerateRequest()
        {
            switch (_method)
            {
                case HttpMethodEnum.GET:
                    return CreateGetRequest();

                default:
                    return "";
            }
        }

        public async Task<ServiceTestResultDTO> SendRequest()
        {
            switch (_method)
            {
                case HttpMethodEnum.GET:
                    return await SendGetRequest();

                default:
                    throw new NotImplementedException();
            }
        }

        private string CreateGetRequest()
        {
            _builder.Append("GET ");
            _builder.Append(_url.AbsolutePath);
            SerilizeData();
            _builder.Append(" HTTP/1.1\r\nHost: ");
            _builder.Append(_url.DnsSafeHost);
            _builder.Append("\r\nAccept: application/json, text/xml, application/xml\r\n");
            return _builder.ToString();
        }

        private async Task<ServiceTestResultDTO> SendGetRequest()
        {
            SerilizeData();
            using HttpClient client = new HttpClient();
            string url = _url.ToString() + _builder.ToString();
            HttpResponseMessage response = await client.GetAsync(url);
            string? mediaType = response.Content.Headers?.ContentType?.MediaType;
            ServiceTestResultDTO result = new ServiceTestResultDTO();
            result.RecievedData = JObject.Parse(await response.Content.ReadAsStringAsync()).ToString(Formatting.Indented);

            if (mediaType == "application/json")
            {
                result.Serialization = SerializationEnum.JSON;
            }
            else if (mediaType == "text/xml" || mediaType == "application/xml")
            {
                result.Serialization = SerializationEnum.XML;
            }
            else
            {
                throw new FormatException();
            }
            
            return result;
        }

        private void SerilizeData()
        {
            switch (_serialization)
            {
                case SerializationEnum.JSON:
                    SerilizeJSON();
                    break;
                
                case SerializationEnum.URL:
                    SerilizeURL();
                    break;
                
                case SerializationEnum.XML:
                    SerilizeXML();
                    break;
                
                default:
                    return;
            }
        }

        private void SerilizeXML()
        {
            return;
        }

        private void SerilizeURL()
        {
            _builder.Append("?");
            foreach (DataSchemaDataDTO schema in _data)
            {
                if (schema.Type != DataTypeEnum.Object && schema.Type != DataTypeEnum.Array)
                {
                    _builder.Append(HttpUtility.UrlEncode(String.IsNullOrEmpty(schema.Alias) ? schema.Name : schema.Alias));
                    _builder.Append("=");
                    _builder.Append(HttpUtility.UrlEncode(schema.Data));
                    _builder.Append("&");
                }
            }
            _builder.Length = _builder.Length - 1;
        }

        private void SerilizeJSON()
        {
            return;
        }
    }
}
