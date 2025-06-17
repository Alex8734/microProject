namespace Mx.Persistence.Model;

public class Motorcycle
{
    public int Id { get; set; }
    public required string Model { get; set; }
    public required string Number { get; set; }
    public int Horsepower { get; set; }
    public bool IsRented { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public int? TrackId { get; set; }
    public Track? Track { get; set; }
    public int? RentedById { get; set; }
    public User? RentedBy { get; set; }
    public DateTime? RentalStartTime { get; set; }
} 