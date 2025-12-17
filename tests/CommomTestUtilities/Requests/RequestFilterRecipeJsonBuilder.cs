using Bogus;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requests;

namespace CommomTestUtilities.Requests;
public class RequestFilterRecipeJsonBuilder
{
    public static RequestFilterRecipeJson Build() 
    {
        return new Faker<RequestFilterRecipeJson>()
            .RuleFor(recipe => recipe.RecipeTitle_Ingredient, faker => faker.Lorem.Word())
            .RuleFor(recipe => recipe.Difficulties, faker => faker.Make(3, faker.PickRandom<Difficulty>))
            .RuleFor(recipe => recipe.CookingTimes, faker => faker.Make(3, faker.PickRandom<CookingTime>))
            .RuleFor(recipe => recipe.DishTypes, faker => faker.Make(3, faker.PickRandom<DishType>));
    }
}
