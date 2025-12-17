using CommomTestUtilities.Entities;
using CommomTestUtilities.LoggedUser;
using CommomTestUtilities.Mapper;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Requests;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Recipe.Register;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Exception.ExceptionsBase;
using MyRecipeBook.Exception;
using CommomTestUtilities.BlobStorage;
using Microsoft.AspNetCore.Http;
using UseCases.Test.Recipe.InlineDatas;

namespace UseCases.Test.Recipe.Register;
public class RegisterRecipeUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        (var user, _) = UserBuilder.Build();

        var useCase = CreateUseCase(user);

        var request = RequestRegisterRecipeFormDataBuilder.Build();

        var result = await useCase.Execute(request);

        result.Should().NotBeNull();
        result.Id.Should().NotBeNullOrWhiteSpace();
        result.Title.Should().Be(request.Title);
    }
    [Theory]
    [ClassData(typeof(ImageTypesInlineData))]
    public async Task Success_With_Image(IFormFile file)
    {
        (var user, _) = UserBuilder.Build();

        var useCase = CreateUseCase(user);

        var request = RequestRegisterRecipeFormDataBuilder.Build(file);

        var result = await useCase.Execute(request);

        result.Should().NotBeNull();
        result.Id.Should().NotBeNullOrWhiteSpace();
        result.Title.Should().Be(request.Title);
    }
    [Fact]
    public async Task Error_Title_Empty()
    {
        (var user, _) = UserBuilder.Build();

        var useCase = CreateUseCase(user);

        var request = RequestRegisterRecipeFormDataBuilder.Build();
        request.Title = string.Empty;

        Func<Task> act = async () => await useCase.Execute(request);

        (await act.Should().ThrowAsync<ErrorOnValidationException>())
            .Where(e => e.GetErrorMessages().Count == 1 &&
                        e.GetErrorMessages().Contains(ResourceMessagesException.RECIPE_TITLE_EMPTY));

    }
    [Fact]
    public async Task Error_Invalid_File()
    {
        (var user, _) = UserBuilder.Build();

        var useCase = CreateUseCase(user);

        var textFile = FormFileBuilder.Txt();

        var request = RequestRegisterRecipeFormDataBuilder.Build(textFile);

        Func<Task> act = async () => await useCase.Execute(request);

        (await act.Should().ThrowAsync<ErrorOnValidationException>())
            .Where(e => e.GetErrorMessages().Count == 1 &&
                        e.GetErrorMessages().Contains(ResourceMessagesException.ONLY_IMAGES_ACCEPTED));

    }
    private static RegisterRecipeUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
    {
        var repository = RecipeWriteOnlyRepositoryBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var mapper = MapperBuilder.Build();
        var blobStorage = new BlobStorageServiceBuilder().Build();

        return new RegisterRecipeUseCase(repository, unitOfWork, loggedUser, mapper, blobStorage);
    }
}
