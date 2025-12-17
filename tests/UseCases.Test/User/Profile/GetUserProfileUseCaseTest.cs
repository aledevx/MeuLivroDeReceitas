using CommomTestUtilities.Entities;
using CommomTestUtilities.LoggedUser;
using CommomTestUtilities.Mapper;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.User.GetProfile;
using MyRecipeBook.Communication.Responses;

namespace UseCases.Test.User.Profile;
public class GetUserProfileUseCaseTest
{
    [Fact]
    public async Task Success() 
    {
        (var user, var _) = UserBuilder.Build();

        var useCase = CreateUseCase(user);

        var result = await useCase.Execute();

        result.Should().NotBeNull();
        result.Email.Should().Be(user.Email);
        result.Name.Should().Be(user.Name);
    }
    private static GetUserProfileUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
    {
        var loggedUser = LoggedUserBuilder.Build(user);
        var mapper = MapperBuilder.Build();
        return new GetUserProfileUseCase(loggedUser, mapper);
    }
}

