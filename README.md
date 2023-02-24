# AzureClient.BlobStorageConsumer

You need to change (replace) the necessary information in the appsettings.json file.

1. BlobConnectionString with your connection string from your Azure blob storage:

"BlobConnectionString": "DefaultEndpointsProtocol=https;AccountName={ACCOUNT_NAME};AccountKey={ACCOUNT_KEY};EndpointSuffix=core.windows.net",

2. BlobContainerName with your container name from your Azure blob storage:

"BlobContainerName": "{CONTAINER_NAME}"
