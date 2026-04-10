using NetTopologySuite.Geometries;

namespace DACS.Services;

public interface IGeofenceService
{
    bool CheckViolation(Point vehicleLocation, int geofenceId);
}
