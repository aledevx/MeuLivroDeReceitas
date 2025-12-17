using System.Net;

namespace MyRecipeBook.Exception.ExceptionsBase;
public class NoPermissionException : MyRecipeBookException
{
    public NoPermissionException(string message) : base(message)
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
