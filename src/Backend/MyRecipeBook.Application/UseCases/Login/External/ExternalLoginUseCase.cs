using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Tokens;

namespace MyRecipeBook.Application.UseCases.Login.External;
public class ExternalLoginUseCase : IExternalLoginUseCase
{
    private readonly IUserWriteOnlyRepository _userWriteOnlyRepository;
    private readonly IUserReadOnlyRepository _userReadOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccessTokenGenerator _accessTokenGenerator;
    public ExternalLoginUseCase(IUserWriteOnlyRepository userWriteOnlyRepository,
        IUserReadOnlyRepository userReadOnlyRepository,
        IUnitOfWork unitOfWork,
        IAccessTokenGenerator accessTokenGenerator)
    {
        _userWriteOnlyRepository = userWriteOnlyRepository;
        _userReadOnlyRepository = userReadOnlyRepository;
        _unitOfWork = unitOfWork;
        _accessTokenGenerator = accessTokenGenerator;
    }
    public async Task<string> Execute(string name, string email)
    {
        var user = await _userReadOnlyRepository.GetByEmail(email);

        if (user is null) 
        {
            user = new Domain.Entities.User
            {
                Name = name,
                Email = email,
                Password = "-"
            };

            await _userWriteOnlyRepository.Add(user);
            await _unitOfWork.CommitAsync();

        }

        return _accessTokenGenerator.Generate(user.UserIdentifier);
    }
}
