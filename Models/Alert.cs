using NetTopologySuite.Geometries;

namespace DACS.Models;

public class Alert
{
    public int Id { get; set; }

    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;

    public DateTime Timestamp { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public Point Location { get; set; } = null!;
    public bool IsProcessed { get; set; } = false;
}
