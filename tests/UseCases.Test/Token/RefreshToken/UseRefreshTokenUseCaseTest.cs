using Azure.Core;
using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Tokens;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Token.RefreshToken;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.ValueObjects;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;

namespace UseCases.Test.Token.RefreshToken;
public class UseRefreshTokenUseCaseTest
{
    [Fact]
    public async Task Success() 
    {
        (var user, _) = UserBuilder.Build();
        var refreshToken = RefreshTokenBuilder.Build(user);

        var useCase = CreateUseCase(refreshToken);

        var result = await useCase.Execute(new RequestNewToken() 
        {
            RefreshToken = refreshToken.Value
        });

        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
    }
    [Fact]
    public async Task Error_RefreshToken_NotFound()
    {
        (var user, _) = UserBuilder.Build();
        var refreshToken = RefreshTokenBuilder.Build(user);

        var useCase = CreateUseCase(refreshToken);

        refreshToken.Value = RefreshTokenGeneratorBuilder.Build().Generate();

        Func<Task> act = async () => await useCase.Execute(new RequestNewToken() { RefreshToken = refreshToken.Value });

        (await act.Should().ThrowAsync<RefreshTokenNotFoundException>())
            .Where(e => e.GetErrorMessages().Count == 1 &&
                        e.GetErrorMessages().Contains(ResourceMessagesException.INVALID_SESSION));
    }
    [Fact]
    public async Task Error_RefreshToken_Expired()
    {
        (var user, _) = UserBuilder.Build();
        var refreshToken = RefreshTokenBuilder.Build(user);

        refreshToken.CreatedOn = DateTime.UtcNow.AddDays(-MyRecipeBookRuleConstants.REFRESH_TOKEN_EXPIRATION_DAYS - 1);

        var useCase = CreateUseCase(refreshToken);


        Func<Task> act = async () => await useCase.Execute(new RequestNewToken() { RefreshToken = refreshToken.Value });

        (await act.Should().ThrowAsync<RefreshTokenExpiredException>())
            .Where(e => e.GetErrorMessages().Count == 1 &&
                        e.GetErrorMessages().Contains(ResourceMessagesException.EXPIRED_SESSION));
    }
    private static UseRefreshTokenUseCase CreateUseCase(MyRecipeBook.Domain.Entities.RefreshToken refreshToken) 
    {
        var tokenRepository = new TokenRepositoryBuilder().Get(refreshToken).Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var accessTokenGenerator = JwtTokenGeneratorBuilder.Build();
        var refreshTokenGenerator = RefreshTokenGeneratorBuilder.Build();

        return new UseRefreshTokenUseCase(tokenRepository, unitOfWork, accessTokenGenerator, refreshTokenGenerator);
    }
}
