namespace AhirMilap.Entity
{
    public class Notifications : BaseEntity
    { 
        public Guid? SenderProfileId { get; set; }
        public Guid? ReceiverProfileId { get; set; }
        public string? NotificationType { get; set; }
        public string? Title { get; set; }
        public string? Message { get; set; } 
        public bool IsRead { get; set; } 
        public string? SenderName { get; set; }
        public string? ProfileFilePath { get; set; }
    }
}