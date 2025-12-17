using CommomTestUtilities.Entities;
using CommomTestUtilities.LoggedUser;
using CommomTestUtilities.Mapper;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Requests;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Recipe.Update;
using MyRecipeBook.Exception.ExceptionsBase;
using MyRecipeBook.Exception;

namespace UseCases.Test.Recipe.Update;
public class UpdateRecipeUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        (var user, _) = UserBuilder.Build();

        var recipe = RecipeBuilder.Build(user);

        var useCase = CreateUseCase(user, recipe);

        var request = RequestRecipeJsonBuilder.Build();

        Func<Task> act = async () => await useCase.Execute(recipe.Id, request);

        await act.Should().NotThrowAsync();
    }
    [Fact]
    public async Task Error_Recipe_Not_Found()
    {
        (var user, _) = UserBuilder.Build();

        var useCase = CreateUseCase(user);

        var request = RequestRecipeJsonBuilder.Build();

        Func<Task> act = async () => await useCase.Execute(1, request);

        (await act.Should().ThrowAsync<NotFoundException>())
           .Where(e => e.GetErrorMessages().Count == 1 &&
                       e.GetErrorMessages().Contains(ResourceMessagesException.RECIPE_NOT_FOUND));

    }
    [Fact]
    public async Task Error_Title_Empty()
    {
        (var user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);

        var useCase = CreateUseCase(user, recipe);

        var request = RequestRecipeJsonBuilder.Build();
        request.Title = string.Empty;

        Func<Task> act = async () => await useCase.Execute(1, request);

        (await act.Should().ThrowAsync<ErrorOnValidationException>())
           .Where(e => e.GetErrorMessages().Count == 1 &&
                       e.GetErrorMessages().Contains(ResourceMessagesException.RECIPE_TITLE_EMPTY));

    }
    private static UpdateRecipeUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user, 
        MyRecipeBook.Domain.Entities.Recipe? recipe = null) 
    {
        var repository = new RecipeUpdateOnlyRepositoryBuilder().GetById(user, recipe).Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var mapper = MapperBuilder.Build();

        return new UpdateRecipeUseCase(unitOfWork, repository, loggedUser, mapper);
    }
}
