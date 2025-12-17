using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Infrastructure.Security.Cryptography;

namespace CommomTestUtilities.Cryptography;

public class PasswordEncripterBuilder
{
    public static IPasswordEncripter Build()
    {
        return new BCryptNet();
    }
}