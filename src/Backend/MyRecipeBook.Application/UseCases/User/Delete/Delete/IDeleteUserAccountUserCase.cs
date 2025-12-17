namespace MyRecipeBook.Application.UseCases.User.Delete.Delete;
public interface IDeleteUserAccountUserCase
{
    public Task Execute(Guid userIdentifier);
}
