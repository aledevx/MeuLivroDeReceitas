using AutoMapper;
using MyRecipeBook.Application.Extensions;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.Storage;

namespace MyRecipeBook.Application.UseCases.Dashboard.Get;
public class GetDashboardUseCase : IGetDashboardUseCase
{
    private readonly IRecipeReadOnlyRepository _repository;
    private readonly ILoggedUser _loggedUser;
    private readonly IMapper _mapper;
    private readonly IBlobStorageService _blobStorageService;
    public GetDashboardUseCase(IRecipeReadOnlyRepository repository, ILoggedUser loggedUser, IMapper mapper, IBlobStorageService blobStorageService)
    {
        _repository = repository;
        _loggedUser = loggedUser;
        _mapper = mapper;
        _blobStorageService = blobStorageService;
    }
    public async Task<ResponseRecipesJson> Execute()
    {
        var loggedUser = await _loggedUser.User();

        var recipes = await _repository.GetForDashboard(loggedUser);

        return new ResponseRecipesJson
        {
            Recipes = await recipes.MapToShortRecipeJson(loggedUser, blobStorageService: _blobStorageService, mapper: _mapper)
        };
    }
}
