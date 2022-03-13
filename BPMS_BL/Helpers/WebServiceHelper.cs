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
using System.Net;

namespace BPMS_BL.Helpers
{
    public class WebServiceHelper
    {
        private readonly StringBuilder _builder = new StringBuilder();
        private readonly Uri _url;
        private readonly ServiceRequestDTO _service;

        public WebServiceHelper(ServiceRequestDTO service)
        {
            _service = service;
            _url = new Uri(service.URL);
        }

        public string GenerateRequest()
        {
            switch (_service.HttpMethod)
            {
                case HttpMethodEnum.GET:
                    return CreateGetRequest();
                
                case HttpMethodEnum.POST:
                    return CreateBodyRequest("POST ");
                
                case HttpMethodEnum.PATCH:
                    return CreateBodyRequest("PATCH ");
                
                case HttpMethodEnum.PUT:
                    return CreateBodyRequest("PUT ");
                
                case HttpMethodEnum.DELETE:
                    return CreateBodyRequest("DELETE ");

                default:
                    throw new NotImplementedException();
            }
        }

        public async Task<ServiceCallResultDTO> SendRequest()
        {
            switch (_service.HttpMethod)
            {
                case HttpMethodEnum.GET:
                    return await SendGetRequest();

                case HttpMethodEnum.POST:
                    return await SendBodyRequest(HttpMethod.Post);
                
                case HttpMethodEnum.PUT:
                    return await SendBodyRequest(HttpMethod.Put);
                
                case HttpMethodEnum.PATCH:
                    return await SendBodyRequest(HttpMethod.Patch);
                
                case HttpMethodEnum.DELETE:
                    return await SendBodyRequest(HttpMethod.Delete);

                default:
                    throw new NotImplementedException();
            }
        }

        private string CreateGetRequest()
        {
            if (_service.Serialization == SerializationEnum.Replace)
            {
                _builder.Append("GET ");
                SerilizeData();
                Uri url = new Uri(_service.URL);
                _builder.Append(url.PathAndQuery);
                _builder.Append(" HTTP/1.1\r\nHost: ");
                _builder.Append(url.DnsSafeHost);
                GenerateHeaders();
                _builder.Append("\r\nAccept: application/json, text/xml, application/xml\r\n");
                return _builder.ToString();
            }
            else
            {
                string result = "GET " + _url.PathAndQuery;
                result += SerilizeData(true);
                _builder.Clear();
                _builder.Append(" HTTP/1.1\r\nHost: ");
                _builder.Append(_url.DnsSafeHost);
                _builder.Append("\r\nContent-Type: ");
                _builder.Append(_service.Serialization.ToMIME());
                return result + _builder.ToString();
            }
        }

        private string CreateBodyRequest(string method)
        {
            _builder.Append(method);
            if (_service.Serialization == SerializationEnum.Replace)
            {
                SerilizeData();
                Uri url = new Uri(_service.URL);
                _builder.Append(url.PathAndQuery);
                _builder.Append(" HTTP/1.1\r\nHost: ");
                _builder.Append(url.DnsSafeHost);
                GenerateHeaders();
                _builder.Append("\r\nAccept: application/json, text/xml, application/xml\r\n\r\n");
                return _builder.ToString();
            }
            else
            {
                _builder.Append(_url.AbsolutePath);
                _builder.Append(" HTTP/1.1\r\nHost: ");
                _builder.Append(_url.DnsSafeHost);
                _builder.Append("\r\nContent-Type: ");
                _builder.Append(_service.Serialization.ToMIME());
                GenerateHeaders();
                _builder.Append("\r\nAccept: application/json, text/xml, application/xml\r\n\r\n");
                string header = _builder.ToString();

                _builder.Clear();
                return header + SerilizeData(true);
            }
        }

        private void GenerateHeaders()
        {
            foreach(HeaderRequestDTO header in _service.Headers)
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
            if (_service.AuthType == ServiceAuthEnum.Basic)
            {
                headers.Add("Authorization", $"Basic {_service.AppId}:{_service.AppSecret}");
            }
            else
            {
                headers.Add("Authorization", $"Bearer {_service.AppSecret}");
            }

            foreach(HeaderRequestDTO header in _service.Headers)
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
            };
            if (_service.Serialization == SerializationEnum.Replace)
            {
                request.RequestUri = new Uri(_service.URL);
            }
            else
            {
                request.RequestUri = new Uri(_url.ToString() + _builder.ToString());
            }
            GenerateHeaders(request.Headers);

            return await CreateResult(await client.SendAsync(request));
        }

        private async Task<ServiceCallResultDTO> SendBodyRequest(HttpMethod method)
        {
            SerilizeData();
            using HttpClient client = new HttpClient();
            using HttpRequestMessage request = new HttpRequestMessage
            {
                Method = method,
                RequestUri = _url, 
                Content = new StringContent(_builder.ToString())
                {
                    Headers = { 
                        ContentType = new MediaTypeHeaderValue(_service.Serialization.ToMIME())
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
            result.StatusCode = HttpStatusCode.OK;
            result.RecievedData = await response.Content.ReadAsStringAsync();

            if (mediaType == "text/xml" || mediaType == "application/xml")
            {
                result.Serialization = SerializationEnum.XMLMarks;
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

        private string SerilizeData(bool indented = false)
        {
            switch (_service.Serialization)
            {
                case SerializationEnum.JSON:
                    SerilizeJson();
                    if (indented)
                    {
                        return JObject.Parse(_builder.ToString()).ToString(Newtonsoft.Json.Formatting.Indented);
                    }
                    break;

                case SerializationEnum.URL:
                    SerilizeURL();
                    break;
                
                case SerializationEnum.XMLMarks:
                    SerilizeXMLMarks();
                    if (indented)
                    {
                        return XDocument.Parse(_builder.ToString()).ToString();
                    }
                    break;
                
                case SerializationEnum.XMLAttributes:
                    SerilizeXMLAttributes();
                    if (indented)
                    {
                        return XDocument.Parse(_builder.ToString()).ToString();
                    }
                    break;
                
                case SerializationEnum.Replace:
                    SerilizeUrlReplace(_service.Nodes);
                    break;
            }

            return "";
        }

        private void SerilizeUrlReplace(IEnumerable<IDataSchemaData> data)
        {
            foreach (DataSchemaDataDTO schema in data)
            {
                string search = "{{" + (String.IsNullOrEmpty(schema.Alias) ? schema.Name : schema.Alias) + "}}";

                if (schema.Type == DataTypeEnum.Object)
                {
                    SerilizeUrlReplace(schema.Children as IEnumerable<IDataSchemaData>);
                }
                else
                {
                    _service.URL = _service.URL.Replace(search, schema.Data);
                }
            }
        }

        private void SerilizeXMLAttributes()
        {
            if (_service.Nodes.Count() == 1 && _service.Nodes.First().Type == DataTypeEnum.Object)
            {
                SerilizeXMLAttributes(_service.Nodes);
            }
            else
            {
                _builder.Append("<root>");
                SerilizeXMLAttributes(_service.Nodes);
                _builder.Append("</root>");
            }
        }

        private void SerilizeXMLAttributes(IEnumerable<IDataSchemaData> data, bool array = false)
        {
            foreach (DataSchemaDataDTO schema in data)
            {
                string name = "value";
                if (!array)
                {
                    name = String.IsNullOrEmpty(schema.Alias) ? schema.Name : schema.Alias;
                }
                else if (schema.Data == null)
                {
                    continue;
                }

                switch (schema.Type)
                {
                    case DataTypeEnum.ArrayString:
                    case DataTypeEnum.ArrayNumber:
                    case DataTypeEnum.ArrayBool:
                    case DataTypeEnum.Object:
                        _builder.Append($"<{name}>");
                        SerilizeXMLAttributes(schema.Children as IEnumerable<IDataSchemaData>, schema.Type != DataTypeEnum.Object);
                        _builder.Append($"</{name}>");            
                        break;

                    case DataTypeEnum.Number:
                    case DataTypeEnum.Bool:
                    case DataTypeEnum.String:
                        _builder.Append($"<{name}>{schema.Data}</{name}>");
                        break;
                    
                    //case DataTypeEnum.ArrayObject:
                    //case DataTypeEnum.ArrayArray:
                        // TODO
                        //break;
                }
            }
        }

        private void SerilizeXMLMarks()
        {
            if (_service.Nodes.Count() == 1 && _service.Nodes.First().Type == DataTypeEnum.Object)
            {
                SerilizeXMLMarks(_service.Nodes, false);
            }
            else
            {
                _builder.Append("<root ");
                SerilizeXMLMarks(_service.Nodes, false);
                _builder.Append("</root>");
            }
        }

        private void SerilizeXMLMarks(IEnumerable<IDataSchemaData> data, bool array = false)
        {
            List<(string, DataSchemaDataDTO)> objects = new List<(string, DataSchemaDataDTO)>();
            foreach (DataSchemaDataDTO schema in data)
            {
                string name = "value";
                if (!array)
                {
                    name = String.IsNullOrEmpty(schema.Alias) ? schema.Name : schema.Alias;
                }
                else if (schema.Data == null)
                {
                    continue;
                }

                switch (schema.Type)
                {
                    case DataTypeEnum.Object:
                        objects.Add((name, schema));
                        break;

                    case DataTypeEnum.Number:
                    case DataTypeEnum.Bool:
                    case DataTypeEnum.String:
                        if (array)
                        {
                            _builder.Append($"<{name}>{schema.Data}</{name}>");
                        }
                        else
                        {
                            _builder.Append($"{name}=\"{schema.Data}\" ");
                        }
                        break;
                    
                    case DataTypeEnum.ArrayString:
                    case DataTypeEnum.ArrayNumber:
                    case DataTypeEnum.ArrayBool:
                    //case DataTypeEnum.ArrayObject:
                    //case DataTypeEnum.ArrayArray:
                        objects.Add((name, schema));
                        break;
                }
            }

            if (!array)
            {
                _builder.Append(">");
            }

            foreach ((string name, DataSchemaDataDTO schema) obj in objects)
            {
                if (obj.schema.Type != DataTypeEnum.Object)
                {
                    _builder.Append($"<{obj.name}>");
                    SerilizeXMLMarks(obj.schema.Children as IEnumerable<IDataSchemaData>, true);
                }
                else
                {
                    _builder.Append($"<{obj.name} ");
                    SerilizeXMLMarks(obj.schema.Children as IEnumerable<IDataSchemaData>, false);
                }
                _builder.Append($"</{obj.name}>");
            }
        }

        private void SerilizeURL()
        {
            _builder.Append("?");
            foreach (DataSchemaDataDTO schema in _service.Nodes)
            {
                if (schema.Type < DataTypeEnum.Object)
                {
                    _builder.Append(HttpUtility.UrlEncode(String.IsNullOrEmpty(schema.Alias) ? schema.Name : schema.Alias));
                    _builder.Append("=");
                    _builder.Append(HttpUtility.UrlEncode(schema.Data));
                    _builder.Append("&");
                }
            }
            _builder.Length = _builder.Length - 1;
        }

        private void SerilizeJson()
        {
            _builder.Append("{");
            SerilizeJSON(_service.Nodes);
            _builder.Append("}");
        }

        private void SerilizeJSON(IEnumerable<IDataSchemaData> data, bool array = false)
        {
            foreach (DataSchemaDataDTO schema in data)
            {
                if (!array)
                {
                    _builder.Append("\"");
                    _builder.Append(String.IsNullOrEmpty(schema.Alias) ? schema.Name : schema.Alias);
                    _builder.Append("\":");
                }
                else if (schema.Data == null)
                {
                    continue;
                }

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
                    
                    case DataTypeEnum.ArrayString:
                    case DataTypeEnum.ArrayNumber:
                    case DataTypeEnum.ArrayBool:
                    //case DataTypeEnum.ArrayObject:
                    //case DataTypeEnum.ArrayArray:
                        _builder.Append("[");
                        SerilizeJSON(schema.Children as IEnumerable<IDataSchemaData>, true);
                        _builder.Append("],");
                        break;
                }
            }

            if (data.Count() > 0)
            {
                _builder.Length = _builder.Length - 1;
            }
        }
    }
}
