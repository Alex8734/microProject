using Mx.Persistence.Model;

namespace Mx.Dtos;

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public int Age { get; set; }
    public double Weight { get; set; }

    public static UserDto FromUser(User u) => new UserDto
    {
        Id = u.Ssn,
        Name = u.Name,
        Age = u.Age,
        Weight = u.Weight
    };
}
