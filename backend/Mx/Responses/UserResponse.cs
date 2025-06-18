using Mx.Core.Services;
using Mx.Persistence.Model;

namespace Mx.Responses;

public class UserResponse
{
    
    public required string Ssn { get; set; }
    public required string Name { get; set; }
    public int Age { get; set; }
    public double Weight { get; set; }
    public int? RentedMotorcycleId { get; set; }
    public string? RentedMotorcycleModel { get; set; }
    public string? RentedMotorcycleNumber { get; set; }
    public List<ValidationError> Errors { get; set; } = new();

    public static UserResponse FromUser(User u) =>
        new()
        {
            Ssn = u.Ssn,
            Name = u.Name,
            Age = u.Age,
            Weight = u.Weight,
            RentedMotorcycleId = u.RentedMotorcycleId,
            RentedMotorcycleModel = u.RentedMotorcycle?.Model,
            RentedMotorcycleNumber = u.RentedMotorcycle?.Number
        };
}
