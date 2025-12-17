using CommomTestUtilities.Requests;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Recipe.Filter;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Exception;

namespace Validators.Test.Recipe.Filter;
public class FilterRecipeValidatorTest
{
    [Fact]
    public void Success() 
    {
        var validator = new FilterRecipeValidator();

        var request = RequestFilterRecipeJsonBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }
    [Fact]
    public void Error_Invalid_Cooking_Time()
    {
        var validator = new FilterRecipeValidator();

        var request = RequestFilterRecipeJsonBuilder.Build();
        request.CookingTimes.Add((CookingTime)1000);

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(error => error.ErrorMessage.Equals(ResourceMessagesException.COOKING_TIME_NOT_SUPPORTED));
    }
    [Fact]
    public void Error_Invalid_Diffculty()
    {
        var validator = new FilterRecipeValidator();

        var request = RequestFilterRecipeJsonBuilder.Build();
        request.Difficulties.Add((Difficulty)1000);

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(error => error.ErrorMessage.Equals(ResourceMessagesException.DIFFICULTY_LEVEL_NOT_SUPPORTED));
    }
    [Fact]
    public void Error_Invalid_DishTypes()
    {
        var validator = new FilterRecipeValidator();

        var request = RequestFilterRecipeJsonBuilder.Build();
        request.DishTypes.Add((DishType)1000);

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(error => error.ErrorMessage.Equals(ResourceMessagesException.DISH_TYPE_NOT_SUPPORTED));
    }
}
