
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using BPMS_Common;

namespace BPMS_BL.Helpers
{
    public static class CommunicationHelper
    {
        public static async Task<bool> ShareModel(string url, string auth, string payload)
        {
            using HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            using HttpClient client = new HttpClient(httpClientHandler);
            using HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url + "Communication/ShareImport"), 
                Content = new StringContent(payload)
                {
                    Headers = { 
                        ContentType = new MediaTypeHeaderValue("application/json")
                    }
                }
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", auth);

            HttpResponseMessage response = await client.SendAsync(request);
            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
