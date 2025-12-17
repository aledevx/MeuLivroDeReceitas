using Microsoft.AspNetCore.Http;

namespace MyRecipeBook.Application.UseCases.Recipe.UpdateImage;
public interface IAddUpdateImageCoverUseCase
{
    Task Execute(long recipeId, IFormFile file);
}
