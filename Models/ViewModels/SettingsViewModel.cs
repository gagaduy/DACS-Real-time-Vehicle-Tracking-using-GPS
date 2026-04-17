using System.ComponentModel.DataAnnotations;

namespace DACS.Models.ViewModels
{
    public class SettingsViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập ngưỡng tốc độ")]
        [Range(1, 300, ErrorMessage = "Tốc độ phải từ 1 đến 300 km/h")]
        public double SpeedLimitThreshold { get; set; }

        public bool IsNotificationEnabled { get; set; }
    }
}
