using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MyRecipeBook.Domain.Dtos;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories.Recipe;

namespace MyRecipeBook.Infrastructure.DataAccess.Repositories;
public class RecipeRepository : IRecipeWriteOnlyRepository, IRecipeReadOnlyRepository, IRecipeUpdateOnlyRepository
{
    private readonly MyRecipeBookDbContext _dbContext;
    public RecipeRepository(MyRecipeBookDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task Add(Recipe recipe)
    {
        await _dbContext.Recipes.AddAsync(recipe);
    }

    public async Task Delete(long recipeId)
    {
       var recipe = await _dbContext.Recipes.FindAsync(recipeId);

        _dbContext.Recipes.Remove(recipe!);
    }

    public async Task<IList<Recipe>> Filter(User user, FilterRecipesDto filters)
    {
        var query = _dbContext
            .Recipes
            .AsNoTracking()
            .Include(r => r.Ingredients)
            .Where(r => r.Active && r.UserId == user.Id);

        if (filters.Difficulties.Any()) 
        {
            query = query.Where(r => r.Difficulty.HasValue && filters.Difficulties.Contains(r.Difficulty.Value));
        }
        if (filters.CookingTimes.Any())
        {
            query = query.Where(r => r.CoockingTime.HasValue && filters.CookingTimes.Contains(r.CoockingTime.Value));
        }
        if (filters.DishTypes.Any())
        {
            query = query.Where(r => r.DishTypes.Any(dishType => filters.DishTypes.Contains(dishType.Type)));
        }
        if (filters.RecipeTitle_Ingredient.NotEmpty()) 
        {
            query = query.Where(r => r.Title.Contains(filters.RecipeTitle_Ingredient) ||
                r.Ingredients.Any(ingredient => ingredient.Item.Contains(filters.RecipeTitle_Ingredient)));
        }

        return await query.ToListAsync();
    }

    async Task<Recipe?> IRecipeReadOnlyRepository.GetById(User user, long recipeId)
    {
       return await GetFullRecipe()
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == recipeId && r.UserId == user.Id && r.Active);
    }

    async Task<Recipe?> IRecipeUpdateOnlyRepository.GetById(User user, long recipeId)
    {
       return await GetFullRecipe().FirstOrDefaultAsync(r => r.Id == recipeId && r.UserId == user.Id && r.Active);

    }
    public void Update(Recipe recipe)
    {
        _dbContext.Recipes.Update(recipe);
    }
    private IIncludableQueryable<Recipe, IList<DishType>> GetFullRecipe() 
    {
        return _dbContext.Recipes
          .Include(r => r.Ingredients)
          .Include(r => r.Instructions)
          .Include(r => r.DishTypes);
    }

    public async Task<IList<Recipe>> GetForDashboard(User user)
    {
        return await _dbContext
            .Recipes
            .AsNoTracking()
            .Include(r => r.Ingredients)
            .Where(r => r.Active && r.UserId == user.Id)
            .OrderByDescending(r => r.CreatedOn)
            .Take(5)
            .ToListAsync();
    }
}
