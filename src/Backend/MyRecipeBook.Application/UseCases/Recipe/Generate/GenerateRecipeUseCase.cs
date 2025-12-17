using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Services.OpenAI;
using MyRecipeBook.Exception.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Generate;
public class GenerateRecipeUseCase : IGenerateRecipeUseCase
{
    private readonly IGenerateRecipeAI _generatedRecipeAI;
    public GenerateRecipeUseCase(IGenerateRecipeAI generatedRecipeAI)
    {
        _generatedRecipeAI = generatedRecipeAI;

    }
    public async Task<ResponseGeneratedRecipeJson> Execute(RequestGenerateRecipeJson request)
    {
        Validate(request);

        var response = await _generatedRecipeAI.Generate(request.Ingredients);

        return new ResponseGeneratedRecipeJson
        {
            Title = response.Title,
            Ingredients = response.Ingredients,
            CookingTime = (Communication.Enums.CookingTime)response.CookingTime,
            Instructions = response.Instructions.Select(i => new ResponseGeneratedInstructionJson
            {
                Step = i.Step,
                Text = i.Text
            }).ToList(),
            Difficulty = Communication.Enums.Difficulty.Low
        };

    }
    private static void Validate(RequestGenerateRecipeJson request) 
    {
        var result = new GenerateRecipeValidator().Validate(request);

        if (result.IsValid.IsFalse())
            throw new ErrorOnValidationException(result.Errors.Select(e => e.ErrorMessage).ToList());
    }
}
