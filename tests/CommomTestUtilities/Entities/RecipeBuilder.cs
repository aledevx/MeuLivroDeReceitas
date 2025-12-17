using Bogus;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Enums;

namespace CommomTestUtilities.Entities;
public class RecipeBuilder
{
    public static IList<Recipe> Collection(User user, uint count = 2) 
    {
        var list = new List<Recipe>();

        if(count == 0)
            count = 1;

        var recipeId = 1;

        for (int x = 0; x < count; x++) 
        {
            var fakeRecipe = Build(user);
            fakeRecipe.Id = recipeId++;

            list.Add(fakeRecipe);
        }

        return list;
    }
    public static Recipe Build(User user)
    {
        return new Faker<Recipe>()
            .RuleFor(recipe => recipe.Id, () => 1)
            .RuleFor(recipe => recipe.Title, (f) => f.Lorem.Word())
            .RuleFor(recipe => recipe.CoockingTime, (f) => f.PickRandom<CookingTime>())
            .RuleFor(recipe => recipe.Difficulty, (f) => f.PickRandom<Difficulty>())
            .RuleFor(recipe => recipe.ImageIdentifier, _ => $"{Guid.NewGuid()}.png")
            .RuleFor(recipe => recipe.Ingredients, (f) => f.Make(1, () => new Ingredient
            {
                Id = 1,
                Item = f.Commerce.ProductName(),
            }))
            .RuleFor(recipe => recipe.Instructions, (f) => f.Make(1, () => new Instruction
            {
                Id = 1,
                Step = 1,
                Text = f.Lorem.Paragraph(),
            }))
            .RuleFor(recipe => recipe.DishTypes, (f) => f.Make(1, () => new MyRecipeBook.Domain.Entities.DishType
            {
                Id = 1,
                Type = f.PickRandom<MyRecipeBook.Domain.Enums.DishType>(),
            }))
            .RuleFor(recipe => recipe.UserId, _ => user.Id);
    }
}
