using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Infrastructure.DataAccess;
using MyRecipeBook.Infrastructure.Security.Tokens.Access;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MyRecipeBook.Infrastructure.Services.LoggedUser;
public class LoggedUser : ILoggedUser
{
    private readonly MyRecipeBookDbContext _context;
    private readonly ITokenProvider _tokenProvider;
    public LoggedUser(MyRecipeBookDbContext context, ITokenProvider tokenProvider)
    {
        _context = context;
        _tokenProvider = tokenProvider;
    }
    public async Task<User> User()
    {
        var token = _tokenProvider.Value();
        var tokenHandler = new JwtSecurityTokenHandler();

        var jwtSecurityToken = tokenHandler.ReadJwtToken(token);

        var identifier = jwtSecurityToken.Claims.First(claim => claim.Type == ClaimTypes.Sid).Value;

        var userIdentifier = Guid.Parse(identifier);

        return await _context.Users
            .AsNoTracking()
            .FirstAsync(u => u.UserIdentifier.Equals(userIdentifier) && u.Active);

    }
}
