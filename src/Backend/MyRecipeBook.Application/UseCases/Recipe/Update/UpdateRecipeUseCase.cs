using AutoMapper;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Update;
public class UpdateRecipeUseCase : IUpdateRecipeUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRecipeUpdateOnlyRepository _repository;
    private readonly ILoggedUser _loggedUser;
    private readonly IMapper _mapper;
    public UpdateRecipeUseCase(IUnitOfWork unitOfWork,
        IRecipeUpdateOnlyRepository repository,
        ILoggedUser loggedUser,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _loggedUser = loggedUser;
        _mapper = mapper;
    }
    public async Task Execute(long recipeId, RequestRecipeJson request)
    {
        Validate(request);

        var loggedUser = await _loggedUser.User();

        var recipe = await _repository.GetById(loggedUser, recipeId);

        if (recipe is null)
        {
            throw new NotFoundException(ResourceMessagesException.RECIPE_NOT_FOUND);
        }

        recipe.Ingredients.Clear();
        recipe.Instructions.Clear();
        recipe.DishTypes.Clear();

        _mapper.Map(request, recipe);

        var instructions = request.Instructions.OrderBy(i => i.Step).ToList();

        for (var index = 0; index < instructions.Count; index++) 
        {
            instructions[index].Step = index + 1;
        }

        recipe.Instructions = _mapper.Map<IList<Domain.Entities.Instruction>>(instructions);

        _repository.Update(recipe);

        await _unitOfWork.CommitAsync();   
    }
    private static void Validate(RequestRecipeJson request) 
    {
        var result = new RecipeValidator().Validate(request);

        if (result.IsValid.IsFalse()) 
        {
            throw new ErrorOnValidationException(result.Errors.Select(e => e.ErrorMessage).Distinct().ToList());
        }
    }
}
