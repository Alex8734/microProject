using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Mx.Requests;

namespace Mx.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SimulatorController : ControllerBase
{
    private readonly ILogger<SimulatorController> _logger;

    public SimulatorController(ILogger<SimulatorController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSimulator([FromBody] SimulatorRequest request)
    {
        try
        {
            var uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);
            var containerName = $"simulator-{uniqueId}";

            // Erstelle den Docker-Befehl
            var command = new StringBuilder();
            command.Append("docker run -d ");
            command.Append($"--name {containerName} ");
            command.Append("--network app-network ");
            command.Append($"-e ASPNETCORE_ENVIRONMENT={request.Environment} ");
            command.Append($"-e TRACK_ID={request.TrackId} ");
            command.Append($"-e BACKEND_URL={request.BackendUrl} ");
            command.Append($"-e GRPC_SERVER_URL={request.GrpcServerUrl} ");
            command.Append($"-e TRACKER_ID={request.TrackerId} ");
            command.Append($"-e USER_SSN={request.UserSsn} ");
            command.Append($"-e MOTORCYCLE_ID={request.MotorcycleId} ");
            command.Append($"-e UPDATE_INTERVAL_MS={request.UpdateIntervalMs} ");
            command.Append("--restart unless-stopped ");
            command.Append("micropproject-simulator:latest");

            _logger.LogInformation("Executing Docker command: {Command}", command.ToString());

            // Führe den Docker-Befehl aus - plattformunabhängig
            var process = new Process();
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {command}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
            }
            else // Linux oder macOS
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{command}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
            }

            process.Start();
            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                _logger.LogError("Error starting simulator container: {Error}", error);
                return BadRequest(new { Success = false, Message = "Fehler beim Starten des Simulators", Error = error });
            }

            // Extrahiere die Container-ID aus der Ausgabe
            var containerId = output.Trim();

            return Ok(new { 
                Success = true, 
                Message = "Simulator erfolgreich gestartet", 
                ContainerId = containerId,
                ContainerName = containerName
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating simulator container");
            return StatusCode(500, new { Success = false, Message = "Interner Serverfehler", Error = ex.Message });
        }
    }

    [HttpDelete("{containerId}")]
    public async Task<IActionResult> StopSimulator(string containerId)
    {
        try
        {
            // Führe den Docker-Befehl aus, um den Container zu stoppen und zu entfernen
            var process = new Process();
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c docker rm -f {containerId}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
            }
            else // Linux oder macOS
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"docker rm -f {containerId}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
            }

            process.Start();
            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                _logger.LogError("Error stopping simulator container: {Error}", error);
                return BadRequest(new { Success = false, Message = "Fehler beim Stoppen des Simulators", Error = error });
            }

            return Ok(new { Success = true, Message = "Simulator erfolgreich gestoppt" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping simulator container");
            return StatusCode(500, new { Success = false, Message = "Interner Serverfehler", Error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetSimulators()
    {
        try
        {
            // Führe den Docker-Befehl aus, um alle laufenden Simulator-Container abzufragen
            var process = new Process();
            
            var command = "docker ps --filter name=simulator --format \"{{.ID}}|{{.Names}}|{{.Status}}\"";
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {command}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
            }
            else // Linux oder macOS
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{command}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
            }

            process.Start();
            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                _logger.LogError("Error getting simulator containers: {Error}", error);
                return BadRequest(new { Success = false, Message = "Fehler beim Abrufen der Simulatoren", Error = error });
            }

            var simulators = new List<object>();
            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length >= 3)
                {
                    simulators.Add(new
                    {
                        ContainerId = parts[0],
                        ContainerName = parts[1],
                        Status = parts[2]
                    });
                }
            }

            return Ok(new { Success = true, Simulators = simulators });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting simulator containers");
            return StatusCode(500, new { Success = false, Message = "Interner Serverfehler", Error = ex.Message });
        }
    }
}
