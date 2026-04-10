using NetTopologySuite.Geometries;

namespace DACS.Models;

public class GPSHistory
{
    public int Id { get; set; }
    
    public int DeviceId { get; set; }
    public Device Device { get; set; } = null!;

    public DateTime Timestamp { get; set; }
    public Point Location { get; set; } = null!;
    public double Speed { get; set; }
}
