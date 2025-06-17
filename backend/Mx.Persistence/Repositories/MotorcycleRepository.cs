using Mx.Persistence.Model;
using Microsoft.EntityFrameworkCore;

namespace Mx.Persistence.Repositories;

public interface IMotorcycleRepository
{
    public Motorcycle AddMotorcycle(string model, string number, int horsepower, int? trackId);
    public ValueTask<IReadOnlyCollection<Motorcycle>> GetAllMotorcyclesAsync(bool tracking);
    public ValueTask<Motorcycle?> GetMotorcycleByIdAsync(int id, bool tracking);
    public ValueTask<IReadOnlyCollection<Motorcycle>> GetAvailableMotorcyclesAsync(bool tracking);
    public void RemoveMotorcycle(Motorcycle motorcycle);
}

public sealed class MotorcycleRepository(DbSet<Motorcycle> motorcycleSet) : IMotorcycleRepository
{
    private IQueryable<Motorcycle> Motorcycles => motorcycleSet;
    private IQueryable<Motorcycle> MotorcyclesNoTracking => Motorcycles.AsNoTracking();

    public Motorcycle AddMotorcycle(string model, string number, int horsepower, int? trackId)
    {
        var motorcycle = new Motorcycle
        {
            Model = model,
            Number = number,
            Horsepower = horsepower,
            IsRented = false,
            TrackId = trackId
        };

        motorcycleSet.Add(motorcycle);

        return motorcycle;
    }

    public async ValueTask<IReadOnlyCollection<Motorcycle>> GetAllMotorcyclesAsync(bool tracking)
    {
        IQueryable<Motorcycle> source = tracking ? Motorcycles : MotorcyclesNoTracking;

        List<Motorcycle> motorcycles = await source
            .Include(m => m.Track)
            .Include(m => m.RentedBy)
            .ToListAsync();

        return motorcycles;
    }

    public async ValueTask<Motorcycle?> GetMotorcycleByIdAsync(int id, bool tracking)
    {
        IQueryable<Motorcycle> source = tracking ? Motorcycles : MotorcyclesNoTracking;

        var motorcycle = await source
            .Include(m => m.Track)
            .Include(m => m.RentedBy)
            .FirstOrDefaultAsync(m => m.Id == id);

        return motorcycle;
    }

    public async ValueTask<IReadOnlyCollection<Motorcycle>> GetAvailableMotorcyclesAsync(bool tracking)
    {
        IQueryable<Motorcycle> source = tracking ? Motorcycles : MotorcyclesNoTracking;

        List<Motorcycle> motorcycles = await source
            .Include(m => m.Track)
            .Where(m => !m.IsRented)
            .ToListAsync();

        return motorcycles;
    }

    public void RemoveMotorcycle(Motorcycle motorcycle)
    {
        motorcycleSet.Remove(motorcycle);
    }
} 