using AzureClient.BlobStorageConsumer.Domain.Entities;
using AzureClient.BlobStorageConsumer.Domain.Enums;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using System.Text;

namespace AzureClient.BlobStorageConsumer.Infrastructure.HttpClients
{
    public class BlobStorageConsumerApiHttpClient : IBlobStorageConsumerApiHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BlobStorageConsumerApiHttpClient> _logger;

        public BlobStorageConsumerApiHttpClient(HttpClient httpClient,
            ILogger<BlobStorageConsumerApiHttpClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<FileStorage>> GetAllBlobFilesAsync()
        {
            List<FileStorage> result = null;

            var rateLimit = Policy.RateLimitAsync(100, TimeSpan.FromMinutes(1));

            try
            {
                using var httpResponseMessage = await rateLimit.ExecuteAsync(() =>
                {
                    return _httpClient.GetAsync("api/v1");
                });

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    result = JsonConvert.DeserializeObject<List<FileStorage>>(httpResponseMessage.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting content.");
                throw;
            }

            return result;
        }

        public async Task<string> GetHtmlContentAsync(int year, Website website, Association association)
        {
            string result = null;

            var rateLimit = Policy.RateLimitAsync(100, TimeSpan.FromMinutes(1));

            try
            {
                using var httpResponseMessage = await rateLimit.ExecuteAsync(() =>
                {
                    var requestUri = new StringBuilder();

                    requestUri.Append("api/v1/HtmlContent");
                    requestUri.Append($"/{year}");

                    if (website == Website.Housing)
                    {
                        requestUri.Append($"/1");
                    }
                    else
                    {
                        requestUri.Append($"/2");
                    }

                    if (association == Association.NVM)
                    {
                        requestUri.Append($"/1");
                    }
                    else
                    {
                        requestUri.Append($"/2");
                    }

                    return _httpClient.GetAsync(requestUri.ToString());
                });

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    result = JsonConvert.DeserializeObject<Content>(httpResponseMessage.Content.ReadAsStringAsync().Result).HTMLContent;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting content.");
                throw;
            }

            return result;
        }
    }
}