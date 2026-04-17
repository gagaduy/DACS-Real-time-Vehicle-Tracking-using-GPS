namespace DACS.Models;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string? CitizenID { get; set; } // Số CCCD
    public string? DriverLicense { get; set; } // Số bằng lái xe
    public DateTime? Birthday { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}
