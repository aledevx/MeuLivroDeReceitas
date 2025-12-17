using CommomTestUtilities.Entities;
using CommomTestUtilities.LoggedUser;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.ServiceBus;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.User.Delete.Request;

namespace UseCases.Test.User.Delete.Request;
public class RequestDeleteUserUseCaseTest
{
    [Fact]
    public async Task Success() 
    {
        (var user, _) = UserBuilder.Build();

        var useCase = CreateUseCase(user);

        var act = async () => await useCase.Execute();

        await act.Should().NotThrowAsync();

        user.Active.Should().BeFalse();
    }
    private static RequestDeleteUserUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user) 
    {
        var queue = DeleteUserQueueBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var repository = new UserUpdateOnlyRepositoryBuilder().GetById(user).Build();
        var unitOfWork = UnitOfWorkBuilder.Build();

        return new RequestDeleteUserUseCase(queue, loggedUser, repository, unitOfWork);
    }

}

