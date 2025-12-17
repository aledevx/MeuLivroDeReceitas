using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;

namespace MyRecipeBook.API.Filters;

public class AuthenticadedUserFilter : IAsyncAuthorizationFilter
{
    private readonly IAccessTokenValidator _accessTokenValidator;
    private readonly IUserReadOnlyRepository _repository;
    public AuthenticadedUserFilter(IAccessTokenValidator accessTokenValidator, IUserReadOnlyRepository repository)
    {
        _accessTokenValidator = accessTokenValidator;
        _repository = repository;
    }
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        try
        {
            var token = TokenOnRequest(context);
            var userIdentifier = _accessTokenValidator.ValidateAndGetUserIdentifier(token);
            var exist = await _repository.ExistActiveUserWithIdentifier(userIdentifier);

            if (exist.IsFalse())
                throw new NoPermissionException(ResourceMessagesException.USER_WITHOUT_PERMISSION_ACCESS_RESOURCE);
        }
        catch (SecurityTokenExpiredException)
        {
            context.Result = new UnauthorizedObjectResult(new ResponseErrorJson("")
            {
                TokenIsExpired = true
            });
        }
        catch (MyRecipeBookException ex)
        {
            context.Result = new UnauthorizedObjectResult(new ResponseErrorJson(ex.Message));
        }
        catch
        {
            context.Result = new UnauthorizedObjectResult(new ResponseErrorJson(ResourceMessagesException.USER_WITHOUT_PERMISSION_ACCESS_RESOURCE));
        }
    }
    private static string TokenOnRequest(AuthorizationFilterContext context)
    {
        var authentication = context.HttpContext.Request.Headers.Authorization.ToString();

        if (string.IsNullOrWhiteSpace(authentication))
        {
            throw new NoPermissionException(ResourceMessagesException.NO_TOKEN);
        }

        return authentication["Bearer ".Length..].Trim();
    }
}
