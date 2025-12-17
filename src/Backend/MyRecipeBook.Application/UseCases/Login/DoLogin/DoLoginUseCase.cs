using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Token;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Exception.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Login.DoLogin;
public class DoLoginUseCase : IDoLoginUseCase
{
    private readonly IUserReadOnlyRepository _repository;
    private readonly IPasswordEncripter _passwordEncripter;
    private readonly IAccessTokenGenerator _acessTokenGenerator;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;
    private readonly ITokenRepository _tokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    public DoLoginUseCase(IUserReadOnlyRepository repository,
        IPasswordEncripter passwordEncripter,
        IAccessTokenGenerator acessTokenGenerator,
        IRefreshTokenGenerator refreshTokenGenerator,
        ITokenRepository tokenRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _passwordEncripter = passwordEncripter;
        _acessTokenGenerator = acessTokenGenerator;
        _refreshTokenGenerator = refreshTokenGenerator;
        _tokenRepository = tokenRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<ResponseRegisteredUserJson> Execute(RequestLoginJson request)
    {
        var user = await _repository.GetByEmail(request.Email);

        if (user is null || _passwordEncripter.IsValid(request.Password, user.Password).IsFalse()) 
        {
            throw new InvalidLoginException();
        }


        var result = new ResponseRegisteredUserJson
        {
            Name = user.Name,
            Tokens = new ResponseTokensJson 
            { 
                AccessToken = _acessTokenGenerator.Generate(user.UserIdentifier),
                RefreshToken = await CreateAndSaveRefreshToken(user)
            }
        };

        return result;
    }
    private async Task<string> CreateAndSaveRefreshToken(Domain.Entities.User user)
    {
        var refreshToken = new Domain.Entities.RefreshToken
        {
            UserId = user.Id,
            Value = _refreshTokenGenerator.Generate()
        };

        await _tokenRepository.SaveNewRefreshToken(refreshToken);
        await _unitOfWork.CommitAsync();

        return refreshToken.Value;
    }
}
