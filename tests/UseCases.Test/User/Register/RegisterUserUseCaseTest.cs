using CommomTestUtilities.Cryptography;
using CommomTestUtilities.Mapper;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Requests;
using CommomTestUtilities.Tokens;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;

namespace UseCases.Test.User.Register;

public class RegisterUserUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        //Arrange
        var request = RequestRegisterUserJsonBuilder.Build();
        var useCase = CreateUseCase();
        
        //Act
        var result = await useCase.Execute(request);

        //Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(request.Name);
        result.Tokens.AccessToken.Should().NotBeNullOrEmpty();
        result.Tokens.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Error_Email_Already_Exists()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        
        var useCase = CreateUseCase(request.Email);
        
        Func<Task> act = async () => await useCase.Execute(request);

        (await act.Should().ThrowAsync<ErrorOnValidationException>())
            .Where(e => e.GetErrorMessages().Count == 1 &&
                        e.GetErrorMessages().Contains(ResourceMessagesException.EMAIL_ALREADY_REGISTERED));
    }
    
    [Fact]
    public async Task Error_Name_Empty()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        request.Name = string.Empty;
        
        var useCase = CreateUseCase();
        
        Func<Task> act = async () => await useCase.Execute(request);

        (await act.Should().ThrowAsync<ErrorOnValidationException>())
            .Where(e => e.GetErrorMessages().Count == 1 &&
                        e.GetErrorMessages().Contains(ResourceMessagesException.NAME_EMPTY));
    }

    private static RegisterUserUseCase CreateUseCase(string? email = null)
    {
        
        var mapper = MapperBuilder.Build();
        var passwordEncripter = PasswordEncripterBuilder.Build();
        var writeRepository = UserWriteOnlyRepositoryBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var readRepositoryBuilder = new UserReadOnlyRepositoryBuilder();
        var jwtTokenGenerator = JwtTokenGeneratorBuilder.Build();
        var refreshTokenGenerator = RefreshTokenGeneratorBuilder.Build();
        var tokenRepository = new TokenRepositoryBuilder();

        if (string.IsNullOrEmpty(email) == false)
        {
            readRepositoryBuilder.ExistActiveUserWithEmail(email);
        }

        return new RegisterUserUseCase(writeRepository, 
            readRepositoryBuilder.Build(),
            unitOfWork,
            mapper,
            passwordEncripter,
            jwtTokenGenerator,
            refreshTokenGenerator,
            tokenRepository.Build()
            );
    }
}