using Mx.Persistence.Model;

namespace Mx.Responses;

public class MotorcycleResponse
{
    public int Id { get; set; }
    public string Model { get; set; } = default!;
    public string Number { get; set; } = default!;
    public int Horsepower { get; set; }
    public int? TrackId { get; set; }
    public bool IsRented { get; set; }

    public static MotorcycleResponse FromMotorcycle(Motorcycle m) => new MotorcycleResponse
    {
        Id = m.Id,
        Model = m.Model,
        Number = m.Number,
        Horsepower = m.Horsepower,
        TrackId = m.TrackId,
        IsRented = m.IsRented
    };
}
