
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using BPMS_Common;
using BPMS_DAL.Entities;
using BPMS_DTOs.Agenda;
using BPMS_DTOs.BlockModel;
using BPMS_DTOs.Model;
using BPMS_DTOs.Pool;
using Newtonsoft.Json;

namespace BPMS_BL.Helpers
{
    public static class CommunicationHelper
    {
        public static async Task<bool> ShareModel(string systemURL, string auth, string payload)
        {
            using HttpResponseMessage response = await SendMessage(systemURL, "Communication/ShareModel", auth, payload);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static async Task<bool> IsModelRunable(string systemURL, string auth, string payload)
        {
            using HttpResponseMessage response = await SendMessage(systemURL, "Communication/IsModelRunable", auth, payload);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static async Task<bool> RunModel(string systemURL, string auth, string payload)
        {
            using HttpResponseMessage response = await SendMessage(systemURL, "Communication/RunModel", auth, payload);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static async Task<bool> ToggleRecieverAttribute(string systemURL, string auth, string payload)
        {
            using HttpResponseMessage response = await SendMessage(systemURL, "Communication/ToggleRecieverAttribute", auth, payload);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static async Task<bool> ToggleForeignRecieverAttribute(string systemURL, string auth, string payload)
        {
            using HttpResponseMessage response = await SendMessage(systemURL, "Communication/ToggleForeignRecieverAttribute", auth, payload);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static async Task<bool> RemoveRecieverAttribute(string systemURL, string auth, Guid id)
        {
            using HttpResponseMessage response = await SendMessage(systemURL, $"Communication/RemoveRecieverAttribute/{id}", auth, "");
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static async Task<bool> RemoveForeignRecieverAttribute(string systemURL, string auth, Guid id)
        {
            using HttpResponseMessage response = await SendMessage(systemURL, $"Communication/RemoveForeignRecieverAttribute/{id}", auth, "");
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static async Task<bool> Message(string systemURL, string auth, string payload)
        {
            using HttpResponseMessage response = await SendMessage(systemURL, "Communication/Message", auth, payload);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static async Task<bool> ForeignMessage(string systemURL, string auth, string payload)
        {
            using HttpResponseMessage response = await SendMessage(systemURL, "Communication/ForeignMessage", auth, payload);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static async Task<bool> BlockActivity(string systemURL, string auth, string payload)
        {
            using HttpResponseMessage response = await SendMessage(systemURL, "Communication/BlockActivity", auth, payload);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static async Task<bool> CreateSystem(string systemURL, string auth, string payload)
        {
            using HttpResponseMessage response = await SendMessage(systemURL, "Communication/CreateSystem", auth, payload);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static async Task<bool> ActivateSystem(string systemURL, string auth, string payload)
        {
            using HttpResponseMessage response = await SendMessage(systemURL, "Communication/ActivateSystem", auth, payload);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static async Task<SenderRecieverConfigDTO> SenderInfo(string systemURL, string auth, string payload)
        {
            using HttpResponseMessage response = await SendMessage(systemURL, $"Communication/SenderInfo/{payload}", auth, "");

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

        public static async Task<List<SenderRecieverConfigDTO>> RecieversInfo(string systemURL, string auth, Guid blockId)
        {
            using HttpResponseMessage response = await SendMessage(systemURL, $"Communication/RecieversInfo/{blockId}", auth, "");
            List<SenderRecieverConfigDTO> dto = JsonConvert.DeserializeObject<List<SenderRecieverConfigDTO>>(await response.Content.ReadAsStringAsync());
            if (dto != null)
            {
                return dto;
            }
            else
            {
                throw new Exception(); // TODO
            }
        }

        internal static async Task<List<AgendaIdNameDTO>> Agendas(string systemURL, string auth)
        {
            using HttpResponseMessage response = await SendMessage(systemURL, "Communication/Agendas", auth, "");
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

        public static async Task<List<ModelIdNameDTO>> Models(string systemURL, string auth, Guid agendaId)
        {
            using HttpResponseMessage response = await SendMessage(systemURL, $"Communication/Models/{agendaId}", auth, "");
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

        public static async Task<List<PoolIdNameDTO>> Pools(string systemURL, string auth, Guid modelId)
        {
            using HttpResponseMessage response = await SendMessage(systemURL, $"Communication/Pools/{modelId}", auth, "");
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

        public static async Task<List<BlockIdNameDTO>> SenderBlocks(string systemURL, string auth, Guid poolId)
        {
            using HttpResponseMessage response = await SendMessage(systemURL, $"Communication/SenderBlocks/{poolId}", auth, "");
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

        public static async Task<bool> RemoveReciever(string systemURL, string auth, Guid blockId, Guid senderId)
        {
            using HttpResponseMessage response = await SendMessage(systemURL, $"Communication/RemoveReciever/{blockId}/{senderId}", auth, "");
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static async Task<List<AttributeEntity>> AddReciever(string systemURL, string auth, Guid blockId, Guid senderId)
        {
            using HttpResponseMessage response = await SendMessage(systemURL, $"Communication/AddReciever/{blockId}/{senderId}", auth, "");
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

        private static async Task<HttpResponseMessage> SendMessage(string systemURL, string path, string auth, string payload)
        {
            using HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            using HttpClient client = new HttpClient(httpClientHandler);
            using HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(systemURL + path), 
                Content = new StringContent(payload)
                {
                    Headers = { 
                        ContentType = new MediaTypeHeaderValue("application/json")
                    }
                }
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", auth);

            return await client.SendAsync(request);
        }
    }
}
