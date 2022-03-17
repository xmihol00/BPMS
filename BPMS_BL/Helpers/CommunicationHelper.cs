
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using BPMS_Common;
using BPMS_Common.Enums;
using BPMS_Common.Interfaces;
using BPMS_DAL.Entities;
using BPMS_DAL.Sharing;
using BPMS_DTOs.Agenda;
using BPMS_DTOs.BlockModel;
using BPMS_DTOs.Model;
using BPMS_DTOs.Pool;
using Newtonsoft.Json;

namespace BPMS_BL.Helpers
{
    public static class CommunicationHelper
    {
        public static async Task<bool> ShareModel(IAddressAuth addressAuth, object payload)
        {
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/ShareModel");
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static async Task<bool> IsModelRunable(IAddressAuth addressAuth, object payload)
        {
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/IsModelRunable");
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static async Task<bool> RunModel(IAddressAuth addressAuth, object payload)
        {
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/RunModel");
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static async Task<bool> CreateRecieverAttribute(IAddressAuth addressAuth, object payload)
        {
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/CreateRecieverAttribute");
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static async Task<bool> CreateForeignRecieverAttribute(IAddressAuth addressAuth, object payload)
        {
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/CreateForeignRecieverAttribute");
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static async Task<bool> RemoveRecieverAttribute(IAddressAuth addressAuth, Guid id)
        {
            object payload = id;
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, $"Communication/RemoveRecieverAttribute/");
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static async Task<bool> RemoveForeignRecieverAttribute(IAddressAuth addressAuth, Guid id)
        {
            object payload = id;
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, $"Communication/RemoveForeignRecieverAttribute/");
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static async Task<bool> Message(IAddressAuth addressAuth, object payload)
        {
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/Message");
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static async Task<bool> ForeignMessage(IAddressAuth addressAuth, object payload)
        {
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/ForeignMessage");
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static async Task<bool> BlockActivity(IAddressAuth addressAuth, object payload)
        {
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/BlockActivity");
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static async Task<bool> CreateSystem(IAddressAuth addressAuth, object payload)
        {
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/CreateSystem");
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static async Task<bool> ActivateSystem(IAddressAuth addressAuth)
        {
            object payload = "";
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/ActivateSystem");
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static async Task<bool> ReactivateSystem(IAddressAuth addressAuth, object payload)
        {
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/ReactivateSystem");
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static async Task<bool> DeactivateSystem(IAddressAuth addressAuth)
        {
            object payload = "";
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/DeactivateSystem");
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static async Task<SenderRecieverConfigDTO> SenderInfo(IAddressAuth addressAuth, object payload)
        {
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/SenderInfo/", HttpMethod.Put);

            SenderRecieverConfigDTO dto = JsonConvert.DeserializeObject<SenderRecieverConfigDTO>(await response.Content.ReadAsStringAsync());
            if (dto != null)
            {
                return dto;
            }
            else
            {
                throw new Exception(); // TODO
            }
        }

        public static async Task<SenderRecieverConfigDTO> ForeignRecieverInfo(IAddressAuth addressAuth, Guid blockId)
        {
            object payload = blockId;
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/ForeignRecieverInfo", HttpMethod.Put);
            SenderRecieverConfigDTO dto = JsonConvert.DeserializeObject<SenderRecieverConfigDTO>(await response.Content.ReadAsStringAsync());
            if (dto != null)
            {
                return dto;
            }
            else
            {
                throw new Exception(); // TODO
            }
        }

        internal static async Task<List<AgendaIdNameDTO>> Agendas(IAddressAuth addressAuth)
        {
            object payload = "";
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/Agendas", HttpMethod.Put);
            List<AgendaIdNameDTO> dto = JsonConvert.DeserializeObject<List<AgendaIdNameDTO>>(await response.Content.ReadAsStringAsync());
            if (dto != null)
            {
                return dto;
            }
            else
            {
                throw new Exception(); // TODO
            }
        }

        public static async Task<List<ModelIdNameDTO>> Models(IAddressAuth addressAuth, Guid agendaId)
        {
            object payload = agendaId;
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/Models", HttpMethod.Put);
            List<ModelIdNameDTO> dto = JsonConvert.DeserializeObject<List<ModelIdNameDTO>>(await response.Content.ReadAsStringAsync());
            if (dto != null)
            {
                return dto;
            }
            else
            {
                throw new Exception(); // TODO
            }
        }

        public static async Task<List<PoolIdNameDTO>> Pools(IAddressAuth addressAuth, Guid modelId)
        {
            object payload = modelId;
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/Pools", HttpMethod.Put);
            List<PoolIdNameDTO> dto = JsonConvert.DeserializeObject<List<PoolIdNameDTO>>(await response.Content.ReadAsStringAsync());
            if (dto != null)
            {
                return dto;
            }
            else
            {
                throw new Exception(); // TODO
            }
        }

        public static async Task<List<BlockIdNameDTO>> SenderBlocks(IAddressAuth addressAuth, Guid poolId)
        {
            object payload = poolId;
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/SenderBlocks", HttpMethod.Put);
            List<BlockIdNameDTO> dto = JsonConvert.DeserializeObject<List<BlockIdNameDTO>>(await response.Content.ReadAsStringAsync());
            if (dto != null)
            {
                return dto;
            }
            else
            {
                throw new Exception(); // TODO
            }
        }

        public static async Task<bool> RemoveReciever(IAddressAuth addressAuth, Guid blockId, Guid senderId)
        {
            object payload = new BlockIdSenderIdDTO
            {
                BlockId = blockId,
                SenderId = senderId
            };
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/RemoveReciever");
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static async Task<List<AttributeEntity>> AddReciever(IAddressAuth addressAuth, Guid blockId, Guid senderId)
        {
            object payload = new BlockIdSenderIdDTO
            {
                BlockId = blockId,
                SenderId = senderId
            };
            using HttpResponseMessage response = await SendMessage(addressAuth, payload, "Communication/AddReciever");
            List<AttributeEntity> dto = JsonConvert.DeserializeObject<List<AttributeEntity>>(await response.Content.ReadAsStringAsync());
            if (dto != null)
            {
                return dto;
            }
            else
            {
                throw new Exception(); // TODO
            }
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
                addressAuth.PayloadHash = new Rfc2898DeriveBytes(body, addressAuth.MessageId.ToByteArray(), 1000).GetBytes(32);
            }

            if (addressAuth.Encryption == EncryptionLevelEnum.Encrypted)
            {
                body = await SymetricCipherHelper.Encrypt(body, addressAuth);
            }

            using HttpClient client = new HttpClient(httpClientHandler);
            using HttpRequestMessage request = new HttpRequestMessage
            {
                Method = method ?? HttpMethod.Post,
                RequestUri = new Uri(addressAuth.DestinationURL + path), 
                Content = new StringContent(body)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", SymetricCipherHelper.AuthEncrypt(addressAuth));

            return await client.SendAsync(request);
        }
    }
}
