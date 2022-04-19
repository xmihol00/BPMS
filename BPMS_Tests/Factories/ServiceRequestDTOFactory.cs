using Microsoft.EntityFrameworkCore;
using BPMS_DAL;
using BPMS_DTOs.Model;
using System.IO;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.Mime;
using BPMS_DTOs.Service;
using BPMS_Common.Enums;

namespace BPMS_Tests.Factories
{
    public static class ServiceRequestDTOFactory
    {
        public static ServiceRequestDTO Create(SerializationEnum serialization, HttpMethodEnum httpMethod, ServiceAuthEnum authType, string URL, 
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

        public static ServiceRequestDTO CreateEmptyTree()
        {
            ServiceRequestDTO dto = new ServiceRequestDTO();

            return dto;
        }
    }
}