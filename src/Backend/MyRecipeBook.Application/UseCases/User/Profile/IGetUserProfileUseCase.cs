using MyRecipeBook.Communication.Responses;

namespace MyRecipeBook.Application.UseCases.User.GetProfile;
public interface IGetUserProfileUseCase
{
    public Task<ResponseUserProfileJson> Execute();
}
