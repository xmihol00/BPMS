using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using AutoMapper;
using BPMS_Common.Enums;
using BPMS_Common.Helpers;
using BPMS_DAL.Entities;
using BPMS_DAL.Repositories;
using BPMS_DTOs.Header;
using BPMS_DTOs.Service;
using BPMS_DTOs.DataSchema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BPMS_BL.Helpers
{
    public class WebServiceHelper
    {
        private readonly StringBuilder _builder = new StringBuilder();
        private readonly IEnumerable<IDataSchemaData> _data;
        private readonly SerializationEnum _serialization;
        private readonly Uri _url;
        private readonly HttpMethodEnum _method;
        private readonly List<HeaderRequestDTO> _headers;

        public WebServiceHelper(ServiceRequestDTO service)
        {
            _data = service.Nodes;
            _serialization = service.Serialization;
            _url = new Uri(service.URL);
            _method = service.HttpMethod;
            _headers = service.Headers;
        }

        public WebServiceHelper(IEnumerable<IDataSchemaData> data, SerializationEnum serialization, Uri url, HttpMethodEnum method)
        {
            _data = data;
            _serialization = serialization;
            _url = url;
            _method = method;
            _headers = new List<HeaderRequestDTO>();
        }

        public string GenerateRequest()
        {
            switch (_method)
            {
                case HttpMethodEnum.GET:
                    return CreateGetRequest();
                
                case HttpMethodEnum.POST:
                    return CreatePostRequest();

                default:
                    return "";
            }
        }

        public async Task<ServiceCallResultDTO> SendRequest()
        {
            switch (_method)
            {
                case HttpMethodEnum.GET:
                    return await SendGetRequest();

                case HttpMethodEnum.POST:
                    return await SendPostRequest();

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
            _builder.Append("\r\nContent-Type: ");
            _builder.Append(_serialization.ToMIME());
            GenerateHeaders();
            _builder.Append("\r\nAccept: application/json, text/xml, application/xml\r\n");
            return _builder.ToString();
        }

        private string CreatePostRequest()
        {
            _builder.Append("POST ");
            _builder.Append(_url.AbsolutePath);
            _builder.Append(" HTTP/1.1\r\nHost: ");
            _builder.Append(_url.DnsSafeHost);
            _builder.Append("\r\nContent-Type: ");
            _builder.Append(_serialization.ToMIME());
            GenerateHeaders();
            _builder.Append("\r\nAccept: application/json, text/xml, application/xml\r\n\r\n");
            string header = _builder.ToString();
            _builder.Clear();
            SerilizeData();

            return header + JToken.Parse(_builder.ToString()).ToString();
        }

        private void GenerateHeaders()
        {
            foreach(HeaderRequestDTO header in _headers)
            {
                _builder.Append("\r\n");
                _builder.Append(header.Name);
                _builder.Append(": ");
                _builder.Append(header.Value);
            }
        }

        private void GenerateHeaders(HttpRequestHeaders headers)
        {
            headers.Add("Accept", "application/json, text/xml, application/xml");
            foreach(HeaderRequestDTO header in _headers)
            {
                headers.Add(header.Name, header.Value);
            }
        }

        private async Task<ServiceCallResultDTO> SendGetRequest()
        {
            SerilizeData();
            using HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_url.ToString() + _builder.ToString()), 
            };
            GenerateHeaders(request.Headers);

            return await CreateResult(await client.SendAsync(request));
        }

        private async Task<ServiceCallResultDTO> SendPostRequest()
        {
            SerilizeData();
            using HttpClient client = new HttpClient();
            using HttpRequestMessage request = new HttpRequestMessage // TODO
            {
                Method = HttpMethod.Post,
                RequestUri = _url, 
                Content = new StringContent(_builder.ToString())
                {
                    Headers = { 
                        ContentType = new MediaTypeHeaderValue(_serialization.ToMIME())
                    }
                }
            };

            GenerateHeaders(request.Headers);

            return await CreateResult(await client.SendAsync(request));
        }


        private async Task<ServiceCallResultDTO> CreateResult(HttpResponseMessage response)
        {
            string? mediaType = response.Content.Headers?.ContentType?.MediaType;
            ServiceCallResultDTO result = new ServiceCallResultDTO();
            result.RecievedData = await response.Content.ReadAsStringAsync();

            if (mediaType == "text/xml" || mediaType == "application/xml")
            {
                result.Serialization = SerializationEnum.XML;
            }
            else if (mediaType == "application/json")
            {
                result.Serialization = SerializationEnum.JSON;
            }
            else
            {
                result.Serialization = null;
            }

            response.Dispose();
            return result;
        }

        private void SerilizeData()
        {
            switch (_serialization)
            {
                case SerializationEnum.JSON:
                    _builder.Append("{");
                    SerilizeJSON(_data);
                    _builder.Append("}");
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

        private void SerilizeJSON(IEnumerable<IDataSchemaData> data)
        {
            foreach (DataSchemaDataDTO schema in data)
            {
                _builder.Append("\"");
                _builder.Append(String.IsNullOrEmpty(schema.Alias) ? schema.Name : schema.Alias);
                _builder.Append("\":");

                switch (schema.Type)
                {
                    case DataTypeEnum.Object:
                        _builder.Append("{");
                        SerilizeJSON(schema.Children as IEnumerable<IDataSchemaData>);
                        _builder.Append("},");
                        break;

                    case DataTypeEnum.String:
                        _builder.Append("\"");
                        _builder.Append(schema.Data);
                        _builder.Append("\",");
                        break;
                    
                    case DataTypeEnum.Number:
                    case DataTypeEnum.Bool:
                        _builder.Append(schema.Data);
                        _builder.Append(",");
                        break;
                    
                    case DataTypeEnum.Array:
                        throw new NotImplementedException();
                }
            }

            if (data.Count() > 0)
            {
                _builder.Length = _builder.Length - 1;
            }
        }
    }
}
