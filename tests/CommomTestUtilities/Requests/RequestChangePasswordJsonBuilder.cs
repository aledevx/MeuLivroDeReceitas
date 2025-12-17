using Bogus;
using MyRecipeBook.Communication.Requests;

namespace CommomTestUtilities.Requests;
public class RequestChangePasswordJsonBuilder
{
    public static RequestChangePasswordJson Build(int passwordLength = 10)
    {
        return new Faker<RequestChangePasswordJson>()
            .RuleFor(request => request.Password, (f) => f.Internet.Password())
            .RuleFor(request => request.NewPassword, (f) => f.Internet.Password(passwordLength));
    }
}

