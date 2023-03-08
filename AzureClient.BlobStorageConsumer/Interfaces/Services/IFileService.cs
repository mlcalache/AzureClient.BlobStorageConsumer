using System.IO;

namespace AzureClient.BlobStorageConsumer.Domain.Interfaces.Services
{
    public interface IFileService
    {
        byte[] GetBytesFromStream(Stream input);
    }
}