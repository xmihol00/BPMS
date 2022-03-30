
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using BPMS_Common;
using BPMS_Common.Enums;
using BPMS_Common.Helpers;
using BPMS_Common.Interfaces;
using BPMS_DAL.Entities;
using BPMS_DAL.Sharing;
using BPMS_DTOs.Agenda;
using BPMS_DTOs.BlockModel;
using BPMS_DTOs.BlockWorkflow;
using BPMS_DTOs.Model;
using BPMS_DTOs.Pool;
using BPMS_DTOs.System;
using Newtonsoft.Json;

namespace BPMS_BL.Helpers
{
    public static class CommunicationHelper
    {
        public static async Task<bool> ShareModel(IAddressAuth addressAuth, string payload)
        {
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/ShareModel");
            return await CheckResponse(addressAuth, response);
        }

        public static async Task<bool> IsModelRunable(IAddressAuth addressAuth, string payload)
        {
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/IsModelRunable");
            return await CheckResponse(addressAuth, response);
        }

        public static async Task<bool> RunModel(IAddressAuth addressAuth, string payload)
        {
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/RunModel");
            return await CheckResponse(addressAuth, response);
        }

        public static async Task<bool> CreateRecieverAttribute(IAddressAuth addressAuth, AttributeEntity payload)
        {
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/CreateRecieverAttribute");
            return await CheckResponse(addressAuth, response);
        }

        public static async Task<bool> CreateForeignRecieverAttribute(IAddressAuth addressAuth, AttributeEntity payload)
        {
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/CreateForeignRecieverAttribute");
            return await CheckResponse(addressAuth, response);
        }

        public static async Task<bool> UpdateRecieverAttribute(IAddressAuth addressAuth, AttributeEntity payload)
        {
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/UpdateRecieverAttribute");
            return await CheckResponse(addressAuth, response);
        }

        public static async Task<bool> UpdateForeignRecieverAttribute(IAddressAuth addressAuth, AttributeEntity payload)
        {
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/UpdateForeignRecieverAttribute");
            return await CheckResponse(addressAuth, response);
        }

        public static async Task<bool> RemoveRecieverAttribute(IAddressAuth addressAuth, Guid id)
        {
            object payload = id;
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, $"Communication/RemoveRecieverAttribute/");
            return await CheckResponse(addressAuth, response);
        }

        public static async Task<bool> RemoveForeignRecieverAttribute(IAddressAuth addressAuth, Guid id)
        {
            object payload = id;
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, $"Communication/RemoveForeignRecieverAttribute/");
            return await CheckResponse(addressAuth, response);
        }

        public static async Task<bool> Message(IAddressAuth addressAuth, MessageShare payload)
        {
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/Message");
            return await CheckResponse(addressAuth, response);
        }

        public static async Task<bool> ForeignMessage(IAddressAuth addressAuth, MessageShare payload)
        {
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/ForeignMessage");
            return await CheckResponse(addressAuth, response);
        }

        public static async Task<bool> BlockActivity(IAddressAuth addressAuth, List<BlockWorkflowActivityDTO> payload)
        {
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/BlockActivity");
            return await CheckResponse(addressAuth, response);
        }

        public static async Task<bool> CreateSystem(IAddressAuth addressAuth, SystemEntity payload)
        {
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/CreateSystem");
            return await CheckResponse(addressAuth, response, payload.Key);
        }

        public static async Task<bool> ActivateSystem(IAddressAuth addressAuth)
        {
            object payload = addressAuth.MessageId.ToString();
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/ActivateSystem");
            return await CheckResponse(addressAuth, response);
        }

        public static async Task<bool> ReactivateSystem(IAddressAuth addressAuth, ConnectionRequestEntity payload)
        {
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/ReactivateSystem");
            return await CheckResponse(addressAuth, response);
        }

        public static async Task<bool> DeactivateSystem(IAddressAuth addressAuth)
        {
            object payload = addressAuth.MessageId.ToString();
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/DeactivateSystem");
            return await CheckResponse(addressAuth, response);
        }

        public static async Task<SenderRecieverConfigDTO> SenderInfo(IAddressAuth addressAuth, Guid payload)
        {
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/SenderInfo/", HttpMethod.Put);
            return await CheckResponse<SenderRecieverConfigDTO>(addressAuth, response);
        }

        public static async Task<SenderRecieverConfigDTO> ForeignRecieverInfo(IAddressAuth addressAuth, Guid blockId)
        {
            object payload = blockId;
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/ForeignRecieverInfo", HttpMethod.Put);
            return await CheckResponse<SenderRecieverConfigDTO>(addressAuth, response);
        }

        public static async Task<List<AgendaIdNameDTO>> Agendas(IAddressAuth addressAuth)
        {
            object payload = addressAuth.MessageId.ToString();
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/Agendas", HttpMethod.Put);
            return await CheckResponse<List<AgendaIdNameDTO>>(addressAuth, response);
        }

        public static async Task<List<ModelIdNameDTO>> Models(IAddressAuth addressAuth, Guid agendaId)
        {
            object payload = agendaId;
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/Models", HttpMethod.Put);
            return await CheckResponse<List<ModelIdNameDTO>>(addressAuth, response);
        }

        public static async Task<List<PoolIdNameDTO>> Pools(IAddressAuth addressAuth, Guid modelId)
        {
            object payload = modelId;
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/Pools", HttpMethod.Put);
            return await CheckResponse<List<PoolIdNameDTO>>(addressAuth, response);
        }

        public static async Task<List<BlockIdNameDTO>> SenderBlocks(IAddressAuth addressAuth, Guid poolId)
        {
            object payload = poolId;
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/SenderBlocks", HttpMethod.Put);
            return await CheckResponse<List<BlockIdNameDTO>>(addressAuth, response);
        }

        public static async Task<bool> RemoveReciever(IAddressAuth addressAuth, Guid blockId, Guid senderId)
        {
            object payload = new BlockIdSenderIdDTO
            {
                BlockId = blockId,
                SenderId = senderId
            };
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/RemoveReciever");
            return await CheckResponse(addressAuth, response);
        }

        public static async Task<List<AttributeEntity>> AddReciever(IAddressAuth addressAuth, Guid blockId, Guid senderId)
        {
            object payload = new BlockIdSenderIdDTO
            {
                BlockId = blockId,
                SenderId = senderId
            };
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/AddReciever");
            return await CheckResponse<List<AttributeEntity>>(addressAuth, response);
        }

        public static async Task<bool> ChangeEncryption(IAddressAuth addressAuth, EncryptionLevelEnum encryption)
        {
            object payload = encryption.ToString();
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/ChangeEncryption");
            
            return await CheckResponse(addressAuth, response);
        }

        private static async Task<HttpResponseMessage> SendMessage(IAddressAuth addressAuth, object payload, string path, HttpMethod? method = null)
        {
            using HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            string body;
            if (payload.GetType() == typeof(string))
            {
                body = payload as string;
            }
            else
            {
                body = JsonConvert.SerializeObject(payload);
            }

            if (addressAuth.Encryption >= EncryptionLevelEnum.Hash)
            {
                addressAuth.PayloadHash = SymetricCipherHelper.HashMessage(body, addressAuth.MessageId);
            }

            if (addressAuth.Encryption == EncryptionLevelEnum.Encrypted)
            {
                body = await SymetricCipherHelper.EncryptMessage(body, addressAuth);
            }

            using HttpClient client = new HttpClient(httpClientHandler);
            using HttpRequestMessage request = new HttpRequestMessage
            {
                Method = method ?? HttpMethod.Post,
                RequestUri = new Uri(addressAuth.DestinationURL + path), 
                Content = new StringContent(body)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await SymetricCipherHelper.AuthEncrypt(addressAuth));

            return await client.SendAsync(request);
        }

        private static async Task<bool> CheckResponse(IAddressAuth addressAuth, HttpResponseMessage response, byte[]? key = null)
        {
            try
            {
                await Authenticate(addressAuth, response, key);
            }
            catch
            {
                return false;
            }

            return response.StatusCode == HttpStatusCode.OK;
        }

        private static async Task<T> CheckResponse<T>(IAddressAuth addressAuth, HttpResponseMessage response, byte[]? key = null) where T : class
        {
            await Authenticate(addressAuth, response, key);

            T dto = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
            if (dto != null)
            {
                return dto;
            }
            else
            {
                throw new Exception();
            }
        }

        private static async Task<string> Authenticate(IAddressAuth addressAuth, HttpResponseMessage response, byte[]? key = null)
        {
            addressAuth.Key = key ?? addressAuth.Key;
            string auth = response.Headers.First(x => x.Key == "Authorization").Value.First()["Bearer ".Length..];
            string data = await response.Content.ReadAsStringAsync();

            (Guid _, byte[] byteAuth) = SymetricCipherHelper.ExtractGuid(auth);
            SystemAuthorizationDTO authSystem = await SymetricCipherHelper.AuthDecrypt<SystemAuthorizationDTO>(byteAuth, addressAuth.Key);

            if (authSystem.MessageId != addressAuth.MessageId)
            {
                throw new UnauthorizedAccessException();
            }

            if (addressAuth.Encryption == EncryptionLevelEnum.Encrypted)
            {
                data = await SymetricCipherHelper.DecryptMessage(data, addressAuth.PayloadKey, addressAuth.PayloadIV);
            }

            if (addressAuth.Encryption >= EncryptionLevelEnum.Hash)
            {
                if (!SymetricCipherHelper.ArraysMatch(authSystem.PayloadHash, SymetricCipherHelper.HashMessage(data, addressAuth.MessageId)))
                {
                    throw new UnauthorizedAccessException();
                }
            }

            return data;
        }
    }
}
