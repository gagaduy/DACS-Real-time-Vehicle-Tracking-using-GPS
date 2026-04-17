using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DACS.Models.ViewModels;

public class ContractViewModel
{
    public int? Id { get; set; }

    [Required]
    [Display(Name = "Số Hợp đồng")]
    public string ContractNumber { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày bắt đầu")]
    public DateTime StartDate { get; set; } = DateTime.Now;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày kết thúc")]
    public DateTime EndDate { get; set; } = DateTime.Now.AddDays(1);

    [Required]
    [Display(Name = "Giá thuê (VNĐ/Ngày)")]
    public decimal RentalPrice { get; set; }

    [Display(Name = "Tiền đặt cọc (VNĐ)")]
    public decimal Deposit { get; set; }

    [Required]
    [Display(Name = "Khách hàng")]
    public int CustomerId { get; set; }

    [Required]
    [Display(Name = "Xe thuê")]
    public int VehicleId { get; set; }

    [Display(Name = "Điều khoản")]
    public string? TermsAndConditions { get; set; }

    public string? Status { get; set; }

    // SelectLists for the view
    public SelectList? CustomerList { get; set; }
    public SelectList? VehicleList { get; set; }
}
