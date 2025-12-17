using Moq;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.Token;

namespace CommomTestUtilities.Repositories;
public class TokenRepositoryBuilder
{
    private readonly Mock<ITokenRepository> _repository;
    public TokenRepositoryBuilder()
    {
        _repository = new Mock<ITokenRepository>();
    }
    public TokenRepositoryBuilder Get(RefreshToken refreshToken)
    {
        _repository.Setup(repository => repository.Get(refreshToken.Value)).ReturnsAsync(refreshToken);

        return this;
    }
    public void SaveNewRefreshToken(RefreshToken refreshToken)
    {
        _repository.Setup(repository => repository.SaveNewRefreshToken(refreshToken)).Returns(Task.CompletedTask);
    }
    public ITokenRepository Build()
    {
        return _repository.Object;
    }
}
