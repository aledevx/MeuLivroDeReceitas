using Bogus;
using MyRecipeBook.Communication.Requests;

namespace CommomTestUtilities.Requests;
public class RequestUpdateUserJsonBuilder
{
    public static RequestUpdateUserJson Build()
    {
        var faker = new Faker<RequestUpdateUserJson>()
            .RuleFor(user => user.Name, (f) => f.Name.FirstName())
            .RuleFor(user => user.Email, (f, user) => f.Internet.Email(user.Name));

        return faker;
    }
}

