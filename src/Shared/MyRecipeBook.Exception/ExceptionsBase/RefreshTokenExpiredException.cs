using System.Net;

namespace MyRecipeBook.Exception.ExceptionsBase;
public class RefreshTokenExpiredException : MyRecipeBookException
{
    public RefreshTokenExpiredException() : base(ResourceMessagesException.EXPIRED_SESSION)
    {
    }

    public override IList<string> GetErrorMessages()
    {
        return [Message];
    }

    public override HttpStatusCode GetStatusCode()
    {
        return HttpStatusCode.Forbidden;
    }
}
