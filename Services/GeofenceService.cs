using NetTopologySuite.Geometries;
using DACS.Data;

namespace DACS.Services;

public class GeofenceService : IGeofenceService
{
    private readonly ApplicationDbContext _context;

    public GeofenceService(ApplicationDbContext context)
    {
        _context = context;
    }

    public bool CheckViolation(Point vehicleLocation, int geofenceId)
    {
        var geofence = _context.Geofences.Find(geofenceId);
        if (geofence == null || !geofence.IsActive)
        {
            return false;
        }

        // Returns true if the location is OUTSIDE the geofence area (violation)
        // If Area.Contains(vehicleLocation) is true, the vehicle is inside.
        return !geofence.Area.Contains(vehicleLocation);
    }
}
