using Moq;
using MyRecipeBook.Domain.Services.ServiceBus;

namespace CommomTestUtilities.ServiceBus;
public class DeleteUserQueueBuilder
{
    public static IDeleteUserQueue Build()
    {
        var mock = new Mock<IDeleteUserQueue>();

        return mock.Object;
    }
}
