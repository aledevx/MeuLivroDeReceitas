using MyRecipeBook.Communication.Responses;

namespace MyRecipeBook.Application.UseCases.Dashboard.Get;
public interface IGetDashboardUseCase
{
    Task<ResponseRecipesJson> Execute();
}
