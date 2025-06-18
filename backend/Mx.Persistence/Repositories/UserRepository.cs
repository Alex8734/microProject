using Mx.Persistence.Model;
using Microsoft.EntityFrameworkCore;

namespace Mx.Persistence.Repositories;

public interface IUserRepository
{
    public User AddUser(string name, int age, double weight);
    public ValueTask<IReadOnlyCollection<User>> GetAllUsersAsync(bool tracking);
    public ValueTask<User?> GetUserByIdAsync(int id, bool tracking);
    public void RemoveUser(User user);
}

public sealed class UserRepository(DbSet<User> userSet) : IUserRepository
{
    private IQueryable<User> Users => userSet;
    private IQueryable<User> UsersNoTracking => Users.AsNoTracking();

    public User AddUser(string name, int age, double weight)
    {
        var user = new User
        {
            Name = name,
            Age = age,
            Weight = weight
        };

        userSet.Add(user);

        return user;
    }

    public async ValueTask<IReadOnlyCollection<User>> GetAllUsersAsync(bool tracking)
    {
        IQueryable<User> source = tracking ? Users : UsersNoTracking;

        List<User> users = await source
            .Include(u => u.RentedMotorcycle)
            .ToListAsync();

        return users;
    }

    public async ValueTask<User?> GetUserByIdAsync(int id, bool tracking)
    {
        IQueryable<User> source = tracking ? Users : UsersNoTracking;

        var user = await source
            .Include(u => u.RentedMotorcycle)
            .FirstOrDefaultAsync(u => u.Ssn == id);

        return user;
    }

    public void RemoveUser(User user)
    {
        userSet.Remove(user);
    }
} 