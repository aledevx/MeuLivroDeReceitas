using AutoMapper;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.GetById;
public class GetByIdRecipeUseCase : IGetByIdRecipeUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IMapper _mapper;
    private readonly IRecipeReadOnlyRepository _repository;
    private readonly IBlobStorageService _blobStorageService;
    public GetByIdRecipeUseCase(ILoggedUser loggedUser,
        IMapper mapper,
        IRecipeReadOnlyRepository repository,
        IBlobStorageService blobStorageService)
    {
        _loggedUser = loggedUser;
        _mapper = mapper;
        _repository = repository;
        _blobStorageService = blobStorageService;
    }
    public async Task<ResponseRecipeJson> Execute(long recipeId)
    {
        var loggedUser = await _loggedUser.User();

        var recipe = await _repository.GetById(loggedUser, recipeId);

        if (recipe is null) 
        {
            throw new NotFoundException(ResourceMessagesException.RECIPE_NOT_FOUND);
        }

        var response = _mapper.Map<ResponseRecipeJson>(recipe);

        if (recipe.ImageIdentifier.NotEmpty()) 
        {
            var url = await _blobStorageService.GetFileUrl(loggedUser, recipe.ImageIdentifier);

            response.ImageUrl = url;
        }

        return response;
    }
}
