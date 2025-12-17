using System.Net;

namespace MyRecipeBook.Exception.ExceptionsBase;
public class NotFoundException : MyRecipeBookException
{
    public NotFoundException(string message) : base(message)
    {
    }

    public override IList<string> GetErrorMessages()
    {
        return [Message];
    }

    public override HttpStatusCode GetStatusCode()
    {
        return HttpStatusCode.NotFound;
    }
}
