using AzureClient.BlobStorageConsumer.Domain.Entities;
using AzureClient.BlobStorageConsumer.Domain.Enums;

namespace AzureClient.BlobStorageConsumer.Infrastructure.HttpClients
{
    public interface IBlobStorageConsumerApiHttpClient
    {
        Task<List<FileStorage>> GetAllBlobFilesAsync();
        Task<string> GetHtmlContentAsync(int year, Website website, Association association);
    }
}