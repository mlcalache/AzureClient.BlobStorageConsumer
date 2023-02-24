using AzureClient.BlobStorageConsumer.Web.Models;
using AzureClient.BlobStorageConsumer.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AzureClient.BlobStorageConsumer.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IBlobStorageService _blobStorage;
        private readonly IFileService _fileService;

        public HomeController(ILogger<HomeController> logger, IBlobStorageService blobStorage, IFileService fileService)
        {
            _logger = logger;
            _blobStorage = blobStorage;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _blobStorage.GetAllBlobFiles());
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(List<IFormFile> files)
        {
            foreach (var file in files)
            {
                await _blobStorage.UploadBlobFileAsync(file);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string blobName)
        {
            await _blobStorage.DeleteDocumentAsync(blobName);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ViewPDF(string blobName)
        {
            return View("ViewPDF", @"https://storagepricelist.blob.core.windows.net/blobpricelist/" + blobName);
        }

        public async Task<IActionResult> DownloadFile(string blobName)
        {
            var str = await _blobStorage.GetFileAsync(blobName);

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