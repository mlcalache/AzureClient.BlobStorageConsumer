using System.ComponentModel.DataAnnotations;

namespace AzureClient.BlobStorageConsumer.Domain.Entities
{
    public class FileStorage
    {
        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [Display(Name = "File Size")]
        public string FileSize { get; set; }

        public string Modified { get; set; }

        public string Uri { get; set; }
    }
}