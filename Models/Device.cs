namespace DACS.Models;

public class Device
{
    public int Id { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    public int? VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }

    public ICollection<GPSHistory> GPSHistories { get; set; } = new List<GPSHistory>();
}
