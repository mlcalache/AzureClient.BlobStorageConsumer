using AzureClient.BlobStorageConsumer.Domain.Interfaces.Services;
using AzureClient.BlobStorageConsumer.Infrastructure.HttpClients;
using AzureClient.BlobStorageConsumer.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AzureClient.BlobStorageConsumer.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IFileStorageService _blobStorage;
        private readonly IFileStreamService _fileService;
        private readonly IBlobStorageConsumerApiHttpClient _blobStorageConsumerApiHttpClient;

        private readonly string _storageConnectionString;
        private readonly string _storageContainerName;

        public HomeController(ILogger<HomeController> logger, IFileStorageService blobStorage, IFileStreamService fileService, IConfiguration configuration, IBlobStorageConsumerApiHttpClient blobStorageConsumerApiHttpClient)
        {
            _logger = logger;
            _blobStorage = blobStorage;
            _fileService = fileService;
            _blobStorageConsumerApiHttpClient = blobStorageConsumerApiHttpClient;

            _storageConnectionString = configuration.GetValue<string>("BlobConnectionString");
            _storageContainerName = configuration.GetValue<string>("BlobContainerName");
        }

        public async Task<IActionResult> Index()
        {
            var result = await _blobStorageConsumerApiHttpClient.GetAllBlobFilesAsync();

            return View(result);

            //return View(await _blobStorage.GetAllBlobFilesAsync(_storageConnectionString, _storageContainerName));
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(List<IFormFile> files)
        {
            var streams = new List<MemoryStream>();

            foreach (var file in files)
            {
                using (var target = new MemoryStream())
                {
                    file.CopyTo(target);
                    streams.Add(target);

                    await _blobStorage.UploadBlobFileAsync(target, file.FileName, _storageConnectionString, _storageContainerName);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string blobName)
        {
            await _blobStorage.DeleteDocumentAsync(blobName, _storageConnectionString, _storageContainerName);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ViewPDF(string blobName)
        {
            return View("ViewPDF", @"https://storagepricelist.blob.core.windows.net/blobpricelist/" + blobName);
        }

        public async Task<IActionResult> DownloadFile(string blobName)
        {
            var str = await _blobStorage.GetBlobFileAsync(blobName, _storageConnectionString, _storageContainerName);

            var data = _fileService.GetBytesFromStream(str);

            return File(data, System.Net.Mime.MediaTypeNames.Application.Octet, blobName);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}