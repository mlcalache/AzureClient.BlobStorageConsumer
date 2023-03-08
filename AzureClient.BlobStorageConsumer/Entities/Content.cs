﻿using AzureClient.BlobStorageConsumer.Domain.Enums;

namespace AzureClient.BlobStorageConsumer.Domain.Entities
{
    public class Content
    {
        public FundaContentType ContentType { get; set; }
        public string ContentTypeDescription { get; set; }
        public string HTMLContent { get; set; }
    }
}