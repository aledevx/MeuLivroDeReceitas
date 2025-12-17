using System.Net;

namespace MyRecipeBook.Exception.ExceptionsBase;
public class RefreshTokenNotFoundException : MyRecipeBookException
{
    public RefreshTokenNotFoundException() : base(ResourceMessagesException.INVALID_SESSION)
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
