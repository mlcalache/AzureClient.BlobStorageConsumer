using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureClient.BlobStorageConsumer.Domain.Entities;
using AzureClient.BlobStorageConsumer.Domain.Interfaces;
using AzureClient.BlobStorageConsumer.Domain.Interfaces.Services;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureClient.BlobStorageConsumer.Infrastructure
{
    public class BlobStorageService : IBlobStorageService
    {
        public static async Task DownloadToStreamAsync(BlobClient blobClient, string localFilePath)
        {
            try
            {
                FileStream fileStream = File.OpenWrite(localFilePath);
                await blobClient.DownloadToAsync(fileStream);
                fileStream.Close();
            }
            catch (DirectoryNotFoundException ex)
            {
                // Let the user know that the directory does not exist
                Console.WriteLine($"Directory not found: {ex.Message}");
            }
        }

        public async Task<Stream> GetFileAsync(string blobName, string storageConnectionString, string storageContainerName)
        {
            BlobContainerClient container = new BlobContainerClient(storageConnectionString, storageContainerName);
            container.CreateIfNotExists(PublicAccessType.Blob);

            var blockBlob = container.GetBlobClient(blobName);

            var streamingResult = await blockBlob.DownloadStreamingAsync();

            var streaming = streamingResult.Value;

            return streaming.Content;
        }

        public async Task<List<BlobStorage>> GetAllBlobFilesAsync(string storageConnectionString, string storageContainerName)
        {
            try
            {
                CloudBlobContainer container = GetCloudBlobContainer(storageConnectionString, storageContainerName);
                CloudBlobDirectory dirb = container.GetDirectoryReference(storageContainerName);

                BlobResultSegment resultSegment = await container.ListBlobsSegmentedAsync(string.Empty,
                    true, BlobListingDetails.Metadata, 100, null, null, null);
                List<BlobStorage> fileList = new List<BlobStorage>();

                foreach (var blobItem in resultSegment.Results)
                {
                    // A flat listing operation returns only blobs, not virtual directories.
                    var blob = (CloudBlob)blobItem;
                    fileList.Add(new BlobStorage()
                    {
                        FileName = blob.Name,
                        FileSize = Math.Round((blob.Properties.Length / 1024f) / 1024f, 2).ToString(),
                        Modified = DateTime.Parse(blob.Properties.LastModified.ToString()).ToLocalTime().ToString(),
                        Uri = blob.Uri.ToString()
                    });
                }
                return fileList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UploadBlobFileAsync(MemoryStream file, string fileName, string storageConnectionString, string storageContainerName)
        {
            try
            {
                byte[] dataFiles;
                CloudBlobContainer cloudBlobContainer = GetCloudBlobContainer(storageConnectionString, storageContainerName);

                BlobContainerPermissions permissions = new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                };
                await cloudBlobContainer.SetPermissionsAsync(permissions);

                dataFiles = file.ToArray();
                // This also does not make a service call; it only creates a local object.
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                await cloudBlockBlob.UploadFromByteArrayAsync(dataFiles, 0, dataFiles.Length);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteDocumentAsync(string blobName, string storageConnectionString, string storageContainerName)
        {
            try
            {
                CloudBlobContainer cloudBlobContainer = GetCloudBlobContainer(storageConnectionString, storageContainerName);
                var blob = cloudBlobContainer.GetBlobReference(blobName);
                await blob.DeleteIfExistsAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private CloudBlobContainer GetCloudBlobContainer(string storageConnectionString, string storageContainerName)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);
            // Create the blob client.
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            // Retrieve a reference to a container.
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(storageContainerName);
            return cloudBlobContainer;
        }
    }
}