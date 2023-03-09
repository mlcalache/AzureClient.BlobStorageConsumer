using AzureClient.BlobStorageConsumer.Domain.Entities;

namespace AzureClient.BlobStorageConsumer.Domain.Interfaces.Services
{
    public interface IFileStorageService
    {
        Task<List<FileStorage>> GetAllBlobFilesAsync(string storageConnectionString, string storageContainerName);

        Task UploadBlobFileAsync(MemoryStream file, string fileName, string storageConnectionString, string storageContainerName);

        Task DeleteDocumentAsync(string blobName, string storageConnectionString, string storageContainerName);

        Task<Stream> GetBlobFileAsync(string blobName, string storageConnectionString, string storageContainerName);
    }
}