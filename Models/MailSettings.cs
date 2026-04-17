namespace DACS.Models
{
    public class MailSettings
    {
        public string SmtpServer { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 587;
        public string SenderName { get; set; } = "GPS Management";
        public string SenderEmail { get; set; } = string.Empty;
        public string AppPassword { get; set; } = string.Empty;
    }
}
