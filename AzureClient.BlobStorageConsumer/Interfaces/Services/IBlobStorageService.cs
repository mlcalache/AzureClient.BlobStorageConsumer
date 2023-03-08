using AzureClient.BlobStorageConsumer.Domain.Entities;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AzureClient.BlobStorageConsumer.Domain.Interfaces.Services
{
    public interface IBlobStorageService
    {
        Task<List<BlobStorage>> GetAllBlobFilesAsync(string storageConnectionString, string storageContainerName);

        Task UploadBlobFileAsync(MemoryStream file, string fileName, string storageConnectionString, string storageContainerName);

        Task DeleteDocumentAsync(string blobName, string storageConnectionString, string storageContainerName);

        Task<Stream> GetFileAsync(string blobName, string storageConnectionString, string storageContainerName);
    }
}