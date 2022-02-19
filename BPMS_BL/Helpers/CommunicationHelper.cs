
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using BPMS_Common;

namespace BPMS_BL.Helpers
{
    public static class CommunicationHelper
    {
        public static async Task<bool> ShareModel(string systemURL, string auth, string payload)
        {
            using HttpResponseMessage response = await SendMessage(systemURL, "Communication/ShareImport", auth, payload);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static async Task<bool> AskForModelRun(string systemURL, string auth, string payload)
        {
            using HttpResponseMessage response = await SendMessage(systemURL, "Communication/IsModelRunable", auth, payload);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static async Task<bool> RunModel(string systemURL, string auth, string payload)
        {
            using HttpResponseMessage response = await SendMessage(systemURL, "Communication/RunModel", auth, payload);
            return response.StatusCode == HttpStatusCode.OK;
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
