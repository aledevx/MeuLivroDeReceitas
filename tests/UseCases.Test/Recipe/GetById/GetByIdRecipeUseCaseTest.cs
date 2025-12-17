using CommomTestUtilities.BlobStorage;
using CommomTestUtilities.Entities;
using CommomTestUtilities.LoggedUser;
using CommomTestUtilities.Mapper;
using CommomTestUtilities.Repositories;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Recipe.GetById;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;

namespace UseCases.Test.Recipe.GetById;
public class GetByIdRecipeUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        (var user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);

        var useCase = CreateUseCase(user, recipe);

        var result = await useCase.Execute(recipe.Id);

        result.Should().NotBeNull();
        result.Id.Should().NotBeNullOrWhiteSpace();
        result.Title.Should().Be(recipe.Title);
    }
    [Fact]
    public async Task Error_Title_Empty()
    {
        (var user, _) = UserBuilder.Build();

        var useCase = CreateUseCase(user);

        Func<Task> act = async () => { await useCase.Execute(recipeId: 1000); };

        (await act.Should().ThrowAsync<NotFoundException>())
            .Where(e => e.GetErrorMessages().Count == 1 &&
                        e.GetErrorMessages().Contains(ResourceMessagesException.RECIPE_NOT_FOUND));

    }
    private static GetByIdRecipeUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user,
        MyRecipeBook.Domain.Entities.Recipe? recipe = null)
    {
        var repository = new RecipeReadOnlyRepositoryBuilder().GetById(user, recipe).Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var mapper = MapperBuilder.Build();
        var blobStorage = new BlobStorageServiceBuilder().GetFileUrl(user, recipe?.ImageIdentifier).Build();

        return new GetByIdRecipeUseCase(loggedUser, mapper, repository, blobStorage);
    }
}
