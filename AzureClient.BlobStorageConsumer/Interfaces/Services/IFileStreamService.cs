using System.IO;

namespace AzureClient.BlobStorageConsumer.Domain.Interfaces.Services
{
    public interface IFileStreamService
    {
        byte[] GetBytesFromStream(Stream input);
    }
}