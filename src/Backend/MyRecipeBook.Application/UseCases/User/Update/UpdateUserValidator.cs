using FluentValidation;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Exception;

namespace MyRecipeBook.Application.UseCases.User.Update;
public class UpdateUserValidator : AbstractValidator<RequestUpdateUserJson>
{
    public UpdateUserValidator() 
    {
        RuleFor(user => user.Name).NotEmpty().WithMessage(ResourceMessagesException.NAME_EMPTY);
        RuleFor(user => user.Email).NotEmpty().WithMessage(ResourceMessagesException.EMAIL_EMPTY);
        When(user => user.Email.NotEmpty(), () =>
        {
            RuleFor(user => user.Email).EmailAddress().WithMessage(ResourceMessagesException.EMAIL_INVALID);
        });
    }
}

