namespace AzureClient.BlobStorageConsumer.API.Models
{
    public class UploadContentRequest
    {
        public MemoryStream File { get; set; }

        public string FileName { get; set; }
    }
}