using Microsoft.EntityFrameworkCore;
using BPMS_DAL;
using BPMS_DTOs.Model;
using System.IO;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.Mime;
using BPMS_DTOs.Service;
using BPMS_Common.Enums;
using BPMS_DTOs.DataSchema;
using System.Collections.Generic;

namespace BPMS_Tests.Factories
{
    public static class ServiceRequestDTOFactory
    {
        public static ServiceRequestDTO CreateEmptyTree(SerializationEnum serialization, HttpMethodEnum httpMethod, ServiceAuthEnum authType, string URL, 
                                               byte[]? appId = null, byte[]? appSecret = null)
        {
            ServiceRequestDTO dto = new ServiceRequestDTO();
            dto.Type = ServiceTypeEnum.REST;
            dto.Serialization = serialization;
            dto.HttpMethod = httpMethod;
            dto.AuthType = authType;
            dto.AppId = appId;
            dto.AppSecret = appSecret;
            dto.URL = URL;

            return dto;
        }

        public static ServiceRequestDTO CreateTree(SerializationEnum serialization, HttpMethodEnum httpMethod, ServiceAuthEnum authType, string URL, 
                                                        byte[]? appId = null, byte[]? appSecret = null)
        {
            ServiceRequestDTO dto = CreateEmptyTree(serialization, httpMethod, authType, URL, appId, appSecret);
            dto.Nodes = new List<DataSchemaDataDTO>
            {
                new DataSchemaDataDTO
                {
                    Alias = "alias1",
                    Compulsory = true,
                    Data = "Hello",
                    Description = "empty",
                    Name = "name",
                    Type = DataTypeEnum.String
                },
                new DataSchemaDataDTO
                {
                    Name = "Object1",
                    Compulsory = true,
                    Type = DataTypeEnum.Object,
                    Children = new List<DataSchemaDataDTO>()
                },
                new DataSchemaDataDTO
                {
                    Name = "object2",
                    Compulsory = true,
                    Type = DataTypeEnum.Object,
                    Children = new List<DataSchemaDataDTO>
                    {
                        new DataSchemaDataDTO
                        {
                            Alias = "integer",
                            Data = "42",
                            Name = "..",
                            Type = DataTypeEnum.Number,
                        }
                    }
                },
                new DataSchemaDataDTO
                {
                    Name = "not",
                    Alias = "obj3",
                    Compulsory = true,
                    Type = DataTypeEnum.Object,
                    Children = new List<DataSchemaDataDTO>
                    {
                        new DataSchemaDataDTO
                        {
                            Alias = "boolean",
                            Data = "true",
                            Name = "bool",
                            Type = DataTypeEnum.Bool,
                        },
                        new DataSchemaDataDTO
                        {
                            Data = "85.36",
                            Name = "double",
                            Type = DataTypeEnum.Number,
                        },
                    }
                },
                new DataSchemaDataDTO
                {
                    Alias = "emptyArray",
                    Children = new List<DataSchemaDataDTO>(),
                    Type = DataTypeEnum.ArrayString,
                },
                new DataSchemaDataDTO
                {
                    Alias = "stringArray",
                    Children = new List<DataSchemaDataDTO>
                    {
                        new DataSchemaDataDTO
                        {
                            Type = DataTypeEnum.String,
                            Data = "str1",
                        },
                        new DataSchemaDataDTO
                        {
                            Type = DataTypeEnum.String,
                            Data = "str2",
                        },
                        new DataSchemaDataDTO
                        {
                            Type = DataTypeEnum.String,
                            Data = "str3",
                        },
                    },
                    Type = DataTypeEnum.ArrayString,
                },
                new DataSchemaDataDTO
                {
                    Alias = "intArray",
                    Children = new List<DataSchemaDataDTO>
                    {
                        new DataSchemaDataDTO
                        {
                            Type = DataTypeEnum.Number,
                            Data = "10",
                        },
                        new DataSchemaDataDTO
                        {
                            Type = DataTypeEnum.Number,
                            Data = "22.22",
                        },
                        new DataSchemaDataDTO
                        {
                            Type = DataTypeEnum.Number,
                            Data = "0",
                        },
                    },
                    Type = DataTypeEnum.ArrayNumber,
                },
                new DataSchemaDataDTO
                {
                    Alias = "boolArray",
                    Children = new List<DataSchemaDataDTO>
                    {
                        new DataSchemaDataDTO
                        {
                            Type = DataTypeEnum.Bool,
                            Data = "false",
                        },
                        new DataSchemaDataDTO
                        {
                            Type = DataTypeEnum.Bool,
                            Data = "true",
                        }
                    },
                    Type = DataTypeEnum.ArrayBool,
                },
                new DataSchemaDataDTO
                {
                    Alias = "objLevel1",
                    Children = new List<DataSchemaDataDTO>
                    {
                        new DataSchemaDataDTO
                        {
                            Type = DataTypeEnum.Object,
                            Name = "objLevel2",
                            Children = new List<DataSchemaDataDTO>
                            {
                                new DataSchemaDataDTO
                                {
                                    Alias = "num",
                                    Type = DataTypeEnum.Number,
                                    Data = "255",
                                    Compulsory = true
                                }
                            }
                        },
                    },
                    Type = DataTypeEnum.Object,
                }
            };

            return dto;
        }
    }
}