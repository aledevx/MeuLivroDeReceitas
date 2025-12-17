using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.ServiceBus;

namespace MyRecipeBook.Application.UseCases.User.Delete.Request;
public class RequestDeleteUserUseCase : IRequestDeleteUserUseCase
{
    private readonly IDeleteUserQueue _queue;
    private readonly ILoggedUser _loggedUser;
    private readonly IUserUpdateOnlyRepository _userUpdateOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    public RequestDeleteUserUseCase(IDeleteUserQueue queue,
        ILoggedUser loggedUser,
        IUserUpdateOnlyRepository userUpdateOnlyRepository,
        IUnitOfWork unitOfWork)
    {
        _queue = queue;
        _loggedUser = loggedUser;
        _userUpdateOnlyRepository = userUpdateOnlyRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task Execute()
    {
        var loggedUser = await _loggedUser.User();

        var user = await _userUpdateOnlyRepository.GetById(loggedUser.Id);

        user.Active = false;
        _userUpdateOnlyRepository.Update(user);

        await _unitOfWork.CommitAsync();

        await _queue.SendMessage(loggedUser);
    }
}
