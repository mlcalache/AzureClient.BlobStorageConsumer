using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureClient.BlobStorageConsumer.Web.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureClient.BlobStorageConsumer.Web.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly string _storageConnectionString;
        private readonly string _storageContainerName;

        public BlobStorageService(IConfiguration configuration)
        {
            _storageConnectionString = configuration.GetValue<string>("BlobConnectionString");
            _storageContainerName = configuration.GetValue<string>("BlobContainerName");
        }

        public static async Task DownloadToStream(BlobClient blobClient, string localFilePath)
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

        public async Task<Stream> GetFileAsync(string blobName)
        {
            BlobContainerClient container = new BlobContainerClient(_storageConnectionString, _storageContainerName);
            container.CreateIfNotExists(PublicAccessType.Blob);

            var blockBlob = container.GetBlobClient(blobName);

            var streamingResult = await blockBlob.DownloadStreamingAsync();

            var streaming = streamingResult.Value;

            return streaming.Content;
        }

        public async Task<List<BlobStorage>> GetAllBlobFiles()
        {
            try
            {
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_storageConnectionString);
                // Create the blob client.
                CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(_storageContainerName);
                CloudBlobDirectory dirb = container.GetDirectoryReference(_storageContainerName);

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
                        Modified = DateTime.Parse(blob.Properties.LastModified.ToString()).ToLocalTime().ToString()
                    });
                }
                return fileList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UploadBlobFileAsync(IFormFile file)
        {
            try
            {
                byte[] dataFiles;
                // Retrieve storage account from connection string.
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_storageConnectionString);
                // Create the blob client.
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                // Retrieve a reference to a container.
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(_storageContainerName);

                BlobContainerPermissions permissions = new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                };
                string systemFileName = file.FileName;
                await cloudBlobContainer.SetPermissionsAsync(permissions);
                await using (var target = new MemoryStream())
                {
                    file.CopyTo(target);
                    dataFiles = target.ToArray();
                }
                // This also does not make a service call; it only creates a local object.
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(systemFileName);
                await cloudBlockBlob.UploadFromByteArrayAsync(dataFiles, 0, dataFiles.Length);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteDocumentAsync(string blobName)
        {
            try
            {
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_storageConnectionString);
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(_storageContainerName);
                var blob = cloudBlobContainer.GetBlobReference(blobName);
                await blob.DeleteIfExistsAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}