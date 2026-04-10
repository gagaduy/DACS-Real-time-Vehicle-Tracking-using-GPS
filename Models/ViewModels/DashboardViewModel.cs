using System.Collections.Generic;
using DACS.Models;

namespace DACS.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalVehicles { get; set; }
        public int OnlineVehicles { get; set; }
        public int TotalCustomers { get; set; }
        public int PendingAlerts { get; set; }
        public List<Rental> LatestRentals { get; set; } = new List<Rental>();
        public List<VehicleLocationDto> VehicleLocations { get; set; } = new List<VehicleLocationDto>();
    }

    public class VehicleLocationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LicensePlate { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Status { get; set; }
    }
}
