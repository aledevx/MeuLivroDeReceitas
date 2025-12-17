using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.User.ChangePassword;
public class ChangePasswordUseCase : IChangePasswordUseCase
{
    private readonly ILoggedUser _loggerUser;
    private readonly IUserUpdateOnlyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordEncripter _passwordEncripter;
    public ChangePasswordUseCase(ILoggedUser loggerUser,
        IUserUpdateOnlyRepository repository,
        IUnitOfWork unitOfWork,
        IPasswordEncripter passwordEncripter)
    {
        _loggerUser = loggerUser;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _passwordEncripter = passwordEncripter;
    }
    public async Task Execute(RequestChangePasswordJson request)
    {
        var loggedUser = await _loggerUser.User();

        Validate(request, loggedUser);

        var user = await _repository.GetById(loggedUser.Id);

        user.Password = _passwordEncripter.Encrypt(request.NewPassword);

        _repository.Update(user);

        await _unitOfWork.CommitAsync();


    }
    private void Validate(RequestChangePasswordJson request, Domain.Entities.User loggedUser)
    {
        var result = new ChangePasswordValidator().Validate(request);

        if (_passwordEncripter.IsValid(request.Password, loggedUser.Password).IsFalse()) 
        {
            result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, ResourceMessagesException.PASSWORD_DIFFERENT_CURRENT_PASSWORD));
        }

        if (result.IsValid.IsFalse())
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }

    }
}

