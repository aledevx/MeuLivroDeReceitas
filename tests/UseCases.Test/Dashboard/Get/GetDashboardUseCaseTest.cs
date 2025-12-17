using CommomTestUtilities.BlobStorage;
using CommomTestUtilities.Entities;
using CommomTestUtilities.LoggedUser;
using CommomTestUtilities.Mapper;
using CommomTestUtilities.Repositories;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Dashboard.Get;

namespace UseCases.Test.Dashboard.Get;
public class GetDashboardUseCaseTest
{
    [Fact]
    public async Task Success() 
    {
        (var user, _) = UserBuilder.Build();

        var recipes = RecipeBuilder.Collection(user);

        var useCase = CreateUseCase(user, recipes);

        var response = await useCase.Execute();

        response.Should().NotBeNull();
        response.Recipes.Should()
            .HaveCountGreaterThan(0)
            .And.OnlyHaveUniqueItems(r => r.Id)
            .And.AllSatisfy(recipe =>
            {
                recipe.Id.Should().NotBeNullOrWhiteSpace();
                recipe.Title.Should().NotBeNullOrWhiteSpace();
                recipe.AmountIngredients.Should().BeGreaterThan(0);
                recipe.ImageUrl.Should().NotBeNullOrWhiteSpace();
            });
    }
    public static GetDashboardUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user,
        IList<MyRecipeBook.Domain.Entities.Recipe> recipes) 
    {
        var loggedUser = LoggedUserBuilder.Build(user);
        var mapper = MapperBuilder.Build();
        var repository = new RecipeReadOnlyRepositoryBuilder().GetForDashboard(user, recipes).Build();
        var blobStorage = new BlobStorageServiceBuilder().GetFileUrl(user, recipes).Build();

        return new GetDashboardUseCase(repository, loggedUser, mapper, blobStorage);
    }
}
