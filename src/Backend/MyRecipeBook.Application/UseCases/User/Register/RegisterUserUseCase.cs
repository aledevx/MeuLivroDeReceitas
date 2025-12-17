using AutoMapper;
using Microsoft.AspNetCore.Mvc.Formatters;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Token;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.User.Register;

public class RegisterUserUseCase : IRegisterUserUseCase
{
    private readonly IUserWriteOnlyRepository _writeOnlyRepository;
    private readonly IUserReadOnlyRepository _readOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPasswordEncripter _passwordEncripter;
    private readonly IAccessTokenGenerator _acessTokenGenerator;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;
    private readonly ITokenRepository _tokenRepository;

    public RegisterUserUseCase(
        IUserWriteOnlyRepository writeOnlyRepository,
        IUserReadOnlyRepository readOnlyRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IPasswordEncripter passwordEncripter,
        IAccessTokenGenerator acessTokenGenerator,
        IRefreshTokenGenerator refreshTokenGenerator,
        ITokenRepository tokenRepository)
    {
        _writeOnlyRepository = writeOnlyRepository;
        _readOnlyRepository = readOnlyRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _passwordEncripter = passwordEncripter;
        _acessTokenGenerator = acessTokenGenerator;
        _refreshTokenGenerator = refreshTokenGenerator;
        _tokenRepository = tokenRepository;
    }
    public async Task<ResponseRegisteredUserJson> Execute(RequestRegisterUserJson request)
    {
        await Validate(request);
        
        var user = _mapper.Map<Domain.Entities.User>(request);
        user.Password = _passwordEncripter.Encrypt(request.Password);

        await _writeOnlyRepository.Add(user);
        await _unitOfWork.CommitAsync();

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

    private async Task Validate(RequestRegisterUserJson request)
    {
        var validator = new RegisterUserValidator();
        var result = validator.Validate(request);

        var emailExist = await _readOnlyRepository.ExistActiveUserWithEmail(request.Email);

        if (emailExist)
        {
            result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, ResourceMessagesException.EMAIL_ALREADY_REGISTERED));
        }

        if (result.IsValid.IsFalse())
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();
            
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}