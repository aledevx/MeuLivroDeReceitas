using System.Net;

namespace MyRecipeBook.Exception.ExceptionsBase;

public abstract class MyRecipeBookException : SystemException
{
    protected MyRecipeBookException(string message) : base(message) 
    { }
    public abstract IList<string> GetErrorMessages();
    public abstract HttpStatusCode GetStatusCode();

}