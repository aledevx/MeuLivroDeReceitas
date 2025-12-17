
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Services.Storage;

namespace MyRecipeBook.Application.UseCases.User.Delete.Delete;
public class DeleteUserAccountUserCase : IDeleteUserAccountUserCase
{
    private readonly IUserDeleteOnlyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobStorageService _blobStorageService;
    public DeleteUserAccountUserCase(IUserDeleteOnlyRepository repository,
        IUnitOfWork unitOfWork, 
        IBlobStorageService blobStorageService)
    {
       _repository = repository;
        _unitOfWork = unitOfWork;
        _blobStorageService = blobStorageService;
    }
    public async Task Execute(Guid userIdentifier)
    {
        await _blobStorageService.DeleteContainer(userIdentifier);
        await _repository.DeleteAccount(userIdentifier);
        await _unitOfWork.CommitAsync();
    }
}
