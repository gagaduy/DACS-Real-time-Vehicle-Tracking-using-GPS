using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACS.Models
{
    public class UserSetting
    {
        [Key]
        public int Id { get; set; }

        public int AccountId { get; set; }
        
        [ForeignKey("AccountId")]
        public Account Account { get; set; } = null!;

        public double SpeedLimitThreshold { get; set; } = 80.0;
        
        public bool IsNotificationEnabled { get; set; } = true;
    }
}
