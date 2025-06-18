using Mx.Persistence.Model;

namespace Mx.Dtos;

public class MotorcycleDto
{
    public int Id { get; set; }
    public string Model { get; set; } = default!;
    public string Number { get; set; } = default!;
    public int Horsepower { get; set; }
    public int? TrackId { get; set; }
    public bool IsRented { get; set; }

    public static MotorcycleDto FromMotorcycle(Motorcycle m) => new MotorcycleDto
    {
        Id = m.Id,
        Model = m.Model,
        Number = m.Number,
        Horsepower = m.Horsepower,
        TrackId = m.TrackId,
        IsRented = m.IsRented
    };
}
