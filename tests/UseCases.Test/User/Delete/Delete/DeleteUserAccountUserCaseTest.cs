using CommomTestUtilities.BlobStorage;
using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.User.Delete.Delete;

namespace UseCases.Test.User.Delete.Delete;
public class DeleteUserAccountUserCaseTest
{
    [Fact]
    public async Task Success()
    {
        (var user, _) = UserBuilder.Build();

        var useCase = CreateUseCase();

        var act = async () => await useCase.Execute(user.UserIdentifier);

        await act.Should().NotThrowAsync();
    }
    private static DeleteUserAccountUserCase CreateUseCase() 
    {
        var repository = UserDeleteOnlyRepositoryBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var blobStorageService = new BlobStorageServiceBuilder().Build();

        return new DeleteUserAccountUserCase(repository, unitOfWork, blobStorageService);
    }
}
