namespace Mx.Persistence.Model;

public class User
{
    public required string Ssn { get; set; }
    public required string Name { get; set; }
    public int Age { get; set; }
    public double Weight { get; set; }
    public Motorcycle? RentedMotorcycle { get; set; }
    public int? RentedMotorcycleId { get; set; }
} 