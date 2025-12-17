using MyRecipeBook.Domain.Repositories;

namespace MyRecipeBook.Infrastructure.DataAccess;

public class UnitOfWork : IUnitOfWork
{
    private readonly MyRecipeBookDbContext _dbContext;

    public UnitOfWork(MyRecipeBookDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task CommitAsync() => await _dbContext.SaveChangesAsync();
}