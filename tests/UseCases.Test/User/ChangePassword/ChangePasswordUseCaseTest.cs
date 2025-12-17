using CommomTestUtilities.Cryptography;
using CommomTestUtilities.Entities;
using CommomTestUtilities.LoggedUser;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Requests;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.User.ChangePassword;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;

namespace UseCases.Test.User.ChangePassword;
public class ChangePasswordUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        //Arrange
        (var user, var password) = UserBuilder.Build();
        var request = RequestChangePasswordJsonBuilder.Build();
        request.Password = password;

        var useCase = CreateUseCase(user);

        //Act
        Func<Task> act = async () => await useCase.Execute(request);

        //Assert
        await act.Should().NotThrowAsync();
    }
    [Fact]
    public async Task Error_NewPassword_Empty()
    {
        //Arrange
        (var user, var password) = UserBuilder.Build();

        var request = new RequestChangePasswordJson() 
        {
            Password = password,
            NewPassword = string.Empty
        };

        var useCase = CreateUseCase(user);

        //Act
        Func<Task> act = async () => { await useCase.Execute(request); };

        //Assert
        (await act.Should().ThrowAsync<ErrorOnValidationException>())
          .Where(e => e.GetErrorMessages().Count == 1 &&
                      e.GetErrorMessages().Contains(ResourceMessagesException.PASSWORD_EMPTY));
    }
    [Fact]
    public async Task Error_CurrentPassword_Different()
    {
        //Arrange
        (var user, var password) = UserBuilder.Build();

        var request = RequestChangePasswordJsonBuilder.Build();

        var useCase = CreateUseCase(user);

        //Act
        Func<Task> act = async () => { await useCase.Execute(request); };

        //Assert
        (await act.Should().ThrowAsync<ErrorOnValidationException>())
          .Where(e => e.GetErrorMessages().Count == 1 &&
                      e.GetErrorMessages().Contains(ResourceMessagesException.PASSWORD_DIFFERENT_CURRENT_PASSWORD));
    }
    private static ChangePasswordUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
    {
        var loggedUser = LoggedUserBuilder.Build(user);
        var repository = new UserUpdateOnlyRepositoryBuilder().GetById(user).Build();
        var passwordEncrypter = PasswordEncripterBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();

        return new ChangePasswordUseCase(
            loggedUser,
            repository,
            unitOfWork,
            passwordEncrypter);
    }
}

