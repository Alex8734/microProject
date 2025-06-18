using Mx.Persistence;
using Mx.Persistence.Model;
using Mx.Persistence.Util;
using OneOf;
using OneOf.Types;

namespace Mx.Core.Services;

public interface IMotorcycleService
{
    public ValueTask<OneOf<Motorcycle, ValidationError>> AddMotorcycleAsync(string model, string number, int horsepower, int? trackId);
    public ValueTask<OneOf<Motorcycle, NotFound>> GetMotorcycleByIdAsync(int id, bool tracking);
    public ValueTask<IReadOnlyCollection<Motorcycle>> GetAllMotorcyclesAsync();
    public ValueTask<IReadOnlyCollection<Motorcycle>> GetAvailableMotorcyclesAsync();
    public ValueTask<OneOf<Success, NotFound>> DeleteMotorcycleAsync(int id);
    public ValueTask<OneOf<Success, ValidationError>> RentMotorcycleAsync(int motorcycleId,string userSsn);
    public ValueTask<OneOf<Success, ValidationError>> ReturnMotorcycleAsync(int motorcycleId);
}

public sealed class MotorcycleService(IUnitOfWork uow) : IMotorcycleService
{
    private const int MinHorsepower = 50;
    private const int MaxHorsepower = 200;

    public async ValueTask<OneOf<Motorcycle, ValidationError>> AddMotorcycleAsync(string model, string number, int horsepower, int? trackId)
    {
        if (string.IsNullOrWhiteSpace(model))
        {
            return new ValidationError { Message = "Model cannot be empty" };
        }

        if (string.IsNullOrWhiteSpace(number))
        {
            return new ValidationError { Message = "Number cannot be empty" };
        }

        if (horsepower < MinHorsepower || horsepower > MaxHorsepower)
        {
            return new ValidationError { Message = $"Horsepower must be between {MinHorsepower} and {MaxHorsepower}" };
        }

        if (trackId.HasValue)
        {
            var track = await uow.TrackRepository.GetTrackByIdAsync(trackId.Value, false);
            if (track is null)
            {
                return new ValidationError { Message = "Specified track does not exist" };
            }
        }

        var motorcycle = uow.MotorcycleRepository.AddMotorcycle(model, number, horsepower, trackId);
        await uow.SaveChangesAsync();
        return motorcycle;
    }

    public async ValueTask<OneOf<Motorcycle, NotFound>> GetMotorcycleByIdAsync(int id, bool tracking)
    {
        var motorcycle = await uow.MotorcycleRepository.GetMotorcycleByIdAsync(id, tracking);
        return motorcycle is null ? new NotFound() : motorcycle;
    }

    public async ValueTask<IReadOnlyCollection<Motorcycle>> GetAllMotorcyclesAsync()
    {
        IReadOnlyCollection<Motorcycle> motorcycles = await uow.MotorcycleRepository.GetAllMotorcyclesAsync(false);
        return motorcycles;
    }

    public async ValueTask<IReadOnlyCollection<Motorcycle>> GetAvailableMotorcyclesAsync()
    {
        IReadOnlyCollection<Motorcycle> motorcycles = await uow.MotorcycleRepository.GetAvailableMotorcyclesAsync(false);
        return motorcycles;
    }

    public async ValueTask<OneOf<Success, NotFound>> DeleteMotorcycleAsync(int id)
    {
        var motorcycle = await uow.MotorcycleRepository.GetMotorcycleByIdAsync(id, true);

        if (motorcycle is null)
        {
            return new NotFound();
        }

        if (motorcycle.IsRented)
        {
            return new NotFound(); // Cannot delete rented motorcycle
        }

        uow.MotorcycleRepository.RemoveMotorcycle(motorcycle);
        await uow.SaveChangesAsync();
        return new Success();
    }

    public async ValueTask<OneOf<Success, ValidationError>> RentMotorcycleAsync(int motorcycleId, string userSsn)
    {
        var motorcycle = await uow.MotorcycleRepository.GetMotorcycleByIdAsync(motorcycleId, true);
        if (motorcycle is null)
        {
            return new ValidationError { Message = "Motorcycle not found" };
        }

        var user = await uow.UserRepository.GetUserBySsnAsync(userSsn, true);
        if (user is null)
        {
            return new ValidationError { Message = "User not found" };
        }

        if (motorcycle.IsRented)
        {
            return new ValidationError { Message = "Motorcycle is already rented" };
        }

        if (user.RentedMotorcycleId.HasValue)
        {
            return new ValidationError { Message = "User already has a rented motorcycle" };
        }

        motorcycle.IsRented = true;
        motorcycle.RentedBySsn = userSsn;
        motorcycle.RentalStartTime = DateTime.UtcNow;
        user.RentedMotorcycleId = motorcycleId;

        await uow.SaveChangesAsync();
        return new Success();
    }

    public async ValueTask<OneOf<Success, ValidationError>> ReturnMotorcycleAsync(int motorcycleId)
    {
        var motorcycle = await uow.MotorcycleRepository.GetMotorcycleByIdAsync(motorcycleId, true);
        if (motorcycle is null)
        {
            return new ValidationError { Message = "Motorcycle not found" };
        }

        if (!motorcycle.IsRented)
        {
            return new ValidationError { Message = "Motorcycle is not rented" };
        }

        var user = await uow.UserRepository.GetUserBySsnAsync(motorcycle.RentedBySsn!, true);
        if (user is not null)
        {
            user.RentedMotorcycleId = null;
        }

        motorcycle.IsRented = false;
        motorcycle.RentedBySsn = null;
        motorcycle.RentalStartTime = null;

        await uow.SaveChangesAsync();
        return new Success();
    }
} 