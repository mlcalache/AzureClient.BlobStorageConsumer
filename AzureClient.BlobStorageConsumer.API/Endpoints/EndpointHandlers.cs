using AzureClient.BlobStorageConsumer.API.Models;
using AzureClient.BlobStorageConsumer.Domain.Enums;
using AzureClient.BlobStorageConsumer.Domain.Interfaces.Services;
using AzureClient.BlobStorageConsumer.Domain.Services;

namespace AzureClient.BlobStorageConsumer.API.Endpoints;

public static class EndpointHandlers
{
    public static async Task<IResult> UploadFile(UploadContentRequest request, CancellationToken token)
    {
        string response = "";

        return Results.Ok(response);
    }

    public static async Task<IResult> GetContentDetails(string id, CancellationToken token)
    {
        ContentResponse response = new ContentResponse();

        return Results.Ok(response);
    }

    public static async Task<IResult> GetHTMLContent(int year, Website website, Association association, CancellationToken token)
    {
        string htmlContent = null;

        //Website website = Website.Housing;
        //Association association = Association.NVM;

        string fileUri = FileUrlService.BuildUri(year, website, association);

        if (website == Website.Business)
        {
            htmlContent = $@"<p></p>
                    <table class=""layout"" cellspacing=""0"" border=""0"">
                        <tbody>
                            <tr>
                                <td class=""col-m"">
                                    <h1><span>Title A</span></h1>
                                    <iframe width=""827px"" height=""1200px"" src=""https://storagepricelist.blob.core.windows.net/blobpricelist/{fileUri}""></iframe>
                                </td>
                            </tr>
                        </tbody>
                    </table>";
        }
        else
        {
            htmlContent = $@"<p></p>
                    <table class=""layout"" cellspacing=""0"" border=""0"">
                        <tbody>
                            <tr>
                                <td class=""col-m"">
                                    <h1><span>Title B</span></h1>
                                    <iframe width=""827px"" height=""1200px"" src=""https://storagepricelist.blob.core.windows.net/blobpricelist/{fileUri}""></iframe>
                                </td>
                            </tr>
                        </tbody>
                    </table>";
        }
        return Results.Ok(new ContentResponse { HTMLContent = htmlContent, Website = website, Year = year, Association = association });
    }

    public static async Task<IResult> GetAllContent(IConfiguration configuration, IFileStorageService blobStorage, CancellationToken token)
    {
        var storageConnectionString = configuration.GetValue<string>("BlobConnectionString");
        var storageContainerName = configuration.GetValue<string>("BlobContainerName");

        var response = await blobStorage.GetAllBlobFilesAsync(storageConnectionString, storageContainerName);

        return Results.Ok(response);
    }
}