using AzureClient.BlobStorageConsumer.Domain.Interfaces.Services;
using AzureClient.BlobStorageConsumer.Infrastructure.Configurations;
using AzureClient.BlobStorageConsumer.Infrastructure.HttpClients;
using AzureClient.BlobStorageConsumer.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var services = builder.Services;

services.AddScoped<IFileStorageService, FileStorageService>();
services.AddScoped<IFileStreamService, FileStreamService>();

var blobStorageConsumerApiHttpClientConfiguration = builder.Configuration.GetSection("Services:BlobStorageConsumerApi").Get<BlobStorageConsumerApiConfiguration>();
services.AddHttpClient<IBlobStorageConsumerApiHttpClient, BlobStorageConsumerApiHttpClient>(httpClient => httpClient.BaseAddress = new Uri(blobStorageConsumerApiHttpClientConfiguration.BaseAddress));

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("https://storagepricelist.blob.core.windows.net");
                      });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();