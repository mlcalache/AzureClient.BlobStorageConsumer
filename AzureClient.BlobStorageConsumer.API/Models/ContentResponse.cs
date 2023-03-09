using AzureClient.BlobStorageConsumer.Domain.Enums;

namespace AzureClient.BlobStorageConsumer.API.Models
{
    public class ContentResponse
    {
        public Website Website { get; set; }
        public string ContentTypeDescription => Website.ToString();
        public Association Association { get; set; }
        public string AssociationDescription => Association.ToString();
        public int Year { get; set; }
        public string HTMLContent { get; set; }
    }
}