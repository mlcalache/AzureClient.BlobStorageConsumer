using AzureClient.BlobStorageConsumer.Domain.Enums;
using System.Text;

namespace AzureClient.BlobStorageConsumer.Domain.Services
{
    public static class FileUrlService
    {
        public static string BuildUri(int year, Website website, Association association)
        {
            //Possible list of existing files:
            //fib-prijslijst-2023-non_nvm.pdf
            //fib-prijslijst-2023-nvm.pdf
            //wonen-prijslijst-2023-non_nvm.pdf
            //wonen-prijslijst-2023-nvm.pdf

            var fileUri = new StringBuilder();

            if (website == Website.Business)
            {
                fileUri.Append("fib-");
            }
            else
            {
                fileUri.Append("wonen-");
            }

            fileUri.Append("prijslijst-");
            fileUri.Append($"{year}-");

            if (association == Association.NVM)
            {
                fileUri.Append("nvm.pdf");
            }
            else
            {
                fileUri.Append("non_nvm.pdf");
            }

            return fileUri.ToString();
        }
    }
}