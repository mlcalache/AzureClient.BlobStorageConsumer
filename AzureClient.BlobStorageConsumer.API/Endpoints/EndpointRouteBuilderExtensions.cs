using AzureClient.BlobStorageConsumer.API.Models;
using AzureClient.BlobStorageConsumer.Domain.Entities;

namespace AzureClient.BlobStorageConsumer.API.Endpoints;

public static class EndpointRouteBuilderExtensions
{
    private static string Prefix => "/api/v1/";

    private static string Route(string routeTemplate = "") => $"{Prefix}{routeTemplate}";

    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost(Route(), EndpointHandlers.UploadFile)
            .Accepts<UploadContentRequest>("application/json")
            .Produces<string>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem();

        app.MapGet(Route("{id}"), EndpointHandlers.GetContentDetails)
            .Accepts<string>("application/json")
            .Produces<ContentResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem();

        app.MapGet(Route(), EndpointHandlers.GetAllContent)
            //.Accepts<UploadContentRequest>("application/json")
            .Produces<List<FileStorage>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem();

        app.MapGet(Route("HtmlContent/{year}/{website}/{association}"), EndpointHandlers.GetHTMLContent)
            //.Accepts<UploadContentRequest>("application/json")
            .Produces<List<ContentResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem();

        return app;
    }
}