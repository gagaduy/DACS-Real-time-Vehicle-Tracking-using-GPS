using NetTopologySuite.Geometries;

namespace DACS.Models;

public class Geofence
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Polygon Area { get; set; } = null!;
    public bool IsActive { get; set; }
}
