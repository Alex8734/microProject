namespace Mx.Requests;

public class MotorcycleRequest
{
    public string Model { get; set; } = default!;
    public string Number { get; set; } = default!;
    public int Horsepower { get; set; }
    public int? TrackId { get; set; }
}
