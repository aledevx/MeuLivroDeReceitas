using Bogus;
using Moq;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Services.Storage;

namespace CommomTestUtilities.BlobStorage;
public class BlobStorageServiceBuilder 
{
    private readonly Mock<IBlobStorageService> _mock;
    public BlobStorageServiceBuilder()
    {
        _mock = new Mock<IBlobStorageService>();
    }
    public BlobStorageServiceBuilder GetFileUrl(User user, string? fileName) 
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return this;
        }

        var faker = new Faker();

        var imageUrl = faker.Image.PlaceImgUrl();

        _mock.Setup(blobstorage => blobstorage.GetFileUrl(user, fileName)).ReturnsAsync(imageUrl);

        return this;
    }
    public BlobStorageServiceBuilder GetFileUrl(User user, IList<Recipe> recipes)
    {
        foreach (var recipe in recipes) 
        {
            GetFileUrl(user, recipe.ImageIdentifier);
        }

        return this;
    }
    public IBlobStorageService Build() 
    {
        return _mock.Object;
    }
}
