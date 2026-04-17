using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACS.Models;

public class Contract
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Số Hợp đồng")]
    public string ContractNumber { get; set; } = string.Empty;

    [Display(Name = "Ngày lập")]
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    [Required]
    [Display(Name = "Ngày bắt đầu")]
    public DateTime StartDate { get; set; }
    
    [Required]
    [Display(Name = "Ngày kết thúc")]
    public DateTime EndDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Giá thuê")]
    public decimal RentalPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Tiền đặt cọc")]
    public decimal Deposit { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Tổng tiền")]
    public decimal TotalAmount { get; set; }

    [Required]
    [StringLength(20)]
    [Display(Name = "Trạng thái")]
    public string Status { get; set; } = "Draft"; // Draft, Active, Completed, Cancelled

    [Display(Name = "Điều khoản")]
    public string? TermsAndConditions { get; set; }

    // Relationships
    [Display(Name = "Khách hàng")]
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    [Display(Name = "Xe")]
    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;
}
