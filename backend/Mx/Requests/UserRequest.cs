namespace Mx.Requests;

public class UserRequest
{
    public required string Ssn { get; set; }
    public string Name { get; set; } = default!;
    public int Age { get; set; }
    public double Weight { get; set; }
}
