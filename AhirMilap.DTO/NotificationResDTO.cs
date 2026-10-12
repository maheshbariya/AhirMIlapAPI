using System.ComponentModel.DataAnnotations;

namespace AhirMilap.DTO
{
    public class NotificationResDTO
    {
        public int TotalCount { get; set; }
        public List<NotificationRes> Notifications { get; set; }
    }
     
    public class NotificationRes
    {
        public Guid? Id { get; set; }
        public Guid? SenderProfileId { get; set; } 
        public string? NotificationType { get; set; }
        public string? Title { get; set; }
        public string? Message { get; set; }
        public bool IsRead { get; set; }
        public string? SenderName { get; set; }
        public string? ProfileFilePath { get; set; }
        public DateTime? CreatedOn { get; set; }
    }

    public class GetNotificationReqDTO : CommonFilter
    {
        [Required]
        public string Type { get; set; } = "ALL";
    }
}
