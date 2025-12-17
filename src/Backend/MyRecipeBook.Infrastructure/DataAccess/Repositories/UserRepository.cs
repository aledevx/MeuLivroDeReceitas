using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.User;

namespace MyRecipeBook.Infrastructure.DataAccess.Repositories;

public class UserRepository : IUserWriteOnlyRepository, IUserReadOnlyRepository, IUserUpdateOnlyRepository, IUserDeleteOnlyRepository
{
    private readonly MyRecipeBookDbContext _dbContext;

    public UserRepository(MyRecipeBookDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(User user)
    {
        await _dbContext.Users.AddAsync(user);
    }

    public async Task<bool> ExistActiveUserWithEmail(string email)
    {
        var exists = await _dbContext.Users.AnyAsync(u => u.Email.Equals(email) && u.Active);
        return exists;
    }

    public async Task<User?> GetByEmailAndPassword(string email, string password)
    {
        return await _dbContext
            .Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email.Equals(email)
            && u.Password.Equals(password) 
            && u.Active);
    }
    public async Task<bool> ExistActiveUserWithIdentifier(Guid userIdentifier)
    {
        var exists = await _dbContext.Users.AnyAsync(u => u.UserIdentifier.Equals(userIdentifier) && u.Active);
        return exists;
    }

    public async Task<User> GetByUserIdentifier(Guid userIdentifier)
    {
        var user = await _dbContext.Users.AsNoTracking()
            .FirstAsync(u => u.UserIdentifier.Equals(userIdentifier) && u.Active);
        return user;
    }

    public async Task<User> GetById(long id)
    {
        return await _dbContext.Users.FirstAsync(u => u.Id == id);
    }

    public void Update(User user)
    {
        _dbContext.Users.Update(user);
    }
    public async Task DeleteAccount(Guid userIdentifier)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserIdentifier == userIdentifier);
        if (user is null)
            return;

        var recipes = _dbContext.Recipes.Where(r => r.UserId == user.Id);

        _dbContext.Recipes.RemoveRange(recipes);

        _dbContext.Users.Remove(user);
    }

    public async Task<User?> GetByEmail(string email)
    {
        return await _dbContext.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email.Equals(email) && u.Active);
    }
}