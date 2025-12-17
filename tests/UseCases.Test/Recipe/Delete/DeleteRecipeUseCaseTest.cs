using CommomTestUtilities.BlobStorage;
using CommomTestUtilities.Entities;
using CommomTestUtilities.LoggedUser;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Requests;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Recipe.Delete;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;

namespace UseCases.Test.Recipe.Delete;
public class DeleteRecipeUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        (var user, _) = UserBuilder.Build();

        var recipe = RecipeBuilder.Build(user);

        var useCase = CreateUseCase(user, recipe);

        Func<Task> act = async () => { await useCase.Execute(recipe.Id); };

        await act.Should().NotThrowAsync();
    }
    [Fact]
    public async Task Error_Recipe_Not_Found()
    {
        (var user, _) = UserBuilder.Build();

        var useCase = CreateUseCase(user);

        Func<Task> act = async () => { await useCase.Execute(1); };

        (await act.Should().ThrowAsync<NotFoundException>())
            .Where(e => e.GetErrorMessages().Contains(ResourceMessagesException.RECIPE_NOT_FOUND));

    }
    private static DeleteRecipeUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user, 
        MyRecipeBook.Domain.Entities.Recipe? recipe = null)
    {
        var readOnlyRepository = new RecipeReadOnlyRepositoryBuilder().GetById(user, recipe).Build();
        var writeOnlyRepository = RecipeWriteOnlyRepositoryBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var blobStorage = new BlobStorageServiceBuilder().GetFileUrl(user, recipe?.ImageIdentifier).Build();

        return new DeleteRecipeUseCase(readOnlyRepository, writeOnlyRepository, unitOfWork, loggedUser, blobStorage);
    }
}
