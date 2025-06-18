namespace Mx.Requests;

public class SimulatorRequest
{
    public int TrackId { get; set; } = 1;
    public string TrackerId { get; set; } = "1";
    public string UserSsn { get; set; } = "123-45-6789";
    public int MotorcycleId { get; set; } = 1;
    public int UpdateIntervalMs { get; set; } = 100;
    public string BackendUrl { get; set; } = "http://backend:5000";
    public string GrpcServerUrl { get; set; } = "grpc://backend:5000";
    public string Environment { get; set; } = "Production";
}
