using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Infrastructure.Security.Tokens.Refresh;

namespace CommomTestUtilities.Tokens;
public class RefreshTokenGeneratorBuilder
{
    public static IRefreshTokenGenerator Build() => new RefreshTokenGenerator();
}
