using Bogus;
using CommomTestUtilities.Cryptography;
using MyRecipeBook.Domain.Entities;

namespace CommomTestUtilities.Entities;
public class UserBuilder
{
    public static (User user, string password) Build() 
    {
        var passwordEncripter = PasswordEncripterBuilder.Build();

        var password = new Faker().Internet.Password();

        var user = new Faker<User>()
            .RuleFor(u => u.Id, () => 1)
            .RuleFor(u => u.Name, (f) => f.Person.FirstName)
            .RuleFor(u => u.Email, (f, user) => f.Internet.Email(user.Name))
            .RuleFor(u => u.UserIdentifier, _ => Guid.NewGuid())
            .RuleFor(u => u.Password, (f) => passwordEncripter.Encrypt(password));

        return (user, password);
    }
}

