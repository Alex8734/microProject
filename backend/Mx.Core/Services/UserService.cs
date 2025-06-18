using Mx.Persistence;
using Mx.Persistence.Model;
using Mx.Persistence.Util;
using OneOf;
using OneOf.Types;

namespace Mx.Core.Services;

public interface IUserService
{
    public ValueTask<OneOf<User, ValidationError>> AddUserAsync(string ssn, string name, int age, double weight);
    public ValueTask<OneOf<User, NotFound>> GetUserBySsnAsync(string ssn, bool tracking);
    public ValueTask<IReadOnlyCollection<User>> GetAllUsersAsync();
    public ValueTask<OneOf<Success, NotFound>> DeleteUserAsync(string id);
}

public sealed class UserService(IUnitOfWork uow) : IUserService
{
    private const int MinAge = 18;
    private const int MaxAge = 80;
    private const double MinWeight = 40.0;
    private const double MaxWeight = 150.0;

    public async ValueTask<OneOf<User, ValidationError>> AddUserAsync(string ssn, string name, int age, double weight)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return new ValidationError { Message = "Name cannot be empty" };
        }

        if (age < MinAge || age > MaxAge)
        {
            return new ValidationError { Message = $"Age must be between {MinAge} and {MaxAge}" };
        }

        if (weight < MinWeight || weight > MaxWeight)
        {
            return new ValidationError { Message = $"Weight must be between {MinWeight} and {MaxWeight} kg" };
        }

        var user = uow.UserRepository.AddUser(ssn,name, age, weight);
        await uow.SaveChangesAsync();
        return user;
    }

    public async ValueTask<OneOf<User, NotFound>> GetUserBySsnAsync(string id, bool tracking)
    {
        var user = await uow.UserRepository.GetUserBySsnAsync(id, tracking);
        return user is null ? new NotFound() : user;
    }

    public async ValueTask<IReadOnlyCollection<User>> GetAllUsersAsync()
    {
        IReadOnlyCollection<User> users = await uow.UserRepository.GetAllUsersAsync(false);
        return users;
    }

    public async ValueTask<OneOf<Success, NotFound>> DeleteUserAsync(string ssn)
    {
        var user = await uow.UserRepository.GetUserBySsnAsync(ssn, true);

        if (user is null)
        {
            return new NotFound();
        }

        if (user.RentedMotorcycleId.HasValue)
        {
            return new NotFound(); // Cannot delete user with active rental
        }

        uow.UserRepository.RemoveUser(user);
        await uow.SaveChangesAsync();
        return new Success();
    }
} 