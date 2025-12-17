using System.Net;

namespace MyRecipeBook.Exception.ExceptionsBase;
public class InvalidLoginException : MyRecipeBookException
{
    public InvalidLoginException() : base(ResourceMessagesException.EMAIL_OR_PASSWORD_INVALID)
    {
    }

    public override IList<string> GetErrorMessages()
    {
        return [Message];
    }

    public override HttpStatusCode GetStatusCode()
    {
        return HttpStatusCode.Unauthorized;
    }
}
