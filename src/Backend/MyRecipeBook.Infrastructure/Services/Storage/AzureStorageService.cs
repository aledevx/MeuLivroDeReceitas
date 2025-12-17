using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Domain.ValueObjects;
using System.ComponentModel;

namespace MyRecipeBook.Infrastructure.Services.Storage;
public class AzureStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    public AzureStorageService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }
    public async Task Upload(User user, Stream file, string fileName)
    {
        var containerName = user.UserIdentifier.ToString();
        var container = _blobServiceClient.GetBlobContainerClient(containerName);
        await container.CreateIfNotExistsAsync();

        var blobClient = container.GetBlobClient(fileName);
        await blobClient.UploadAsync(file, overwrite: true);
    }
    public async Task<string> GetFileUrl(User user, string fileName) 
    {
        var containerName = user.UserIdentifier.ToString();
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        var exists = await containerClient.ExistsAsync();
        if (exists.Value.IsFalse())
        {
            return string.Empty; 
        }

        var blobClient = containerClient.GetBlobClient(fileName);
        exists = await blobClient.ExistsAsync();
        if (exists.Value) 
        {
            var sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = containerName,
                BlobName = fileName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(MyRecipeBookRuleConstants.MAXIMUM_IMAGE_URL_LIFETIME_IN_MINUTES)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            return blobClient.GenerateSasUri(sasBuilder).ToString();
        }

        return string.Empty;
    }

    public async Task Delete(User user, string fileName)
    {
        var containerName = user.UserIdentifier.ToString();
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var exists = await containerClient.ExistsAsync();
        if (exists.Value) 
        {
            await containerClient.DeleteBlobIfExistsAsync(fileName);
        }

    }

    public async Task DeleteContainer(Guid userIdentifier)
    {
        var container = _blobServiceClient.GetBlobContainerClient(userIdentifier.ToString());

        await container.DeleteIfExistsAsync();
    }
}
