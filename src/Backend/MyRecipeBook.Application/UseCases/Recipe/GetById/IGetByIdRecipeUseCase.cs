using MyRecipeBook.Communication.Responses;

namespace MyRecipeBook.Application.UseCases.Recipe.GetById;
public interface IGetByIdRecipeUseCase
{
    Task<ResponseRecipeJson> Execute(long recipeId);
}
