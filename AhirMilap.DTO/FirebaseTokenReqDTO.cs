using System.ComponentModel.DataAnnotations;

namespace AhirMilap.DTO
{
    public class FirebaseTokenReqDTO
    { 
        [Required]
        [MaxLength(500)]
        public string FcmToken { get; set; }
        public bool IsLogOut { get; set; }
    }

    public class NotificationReqDTO
    {
        public Guid? Id { get; set; }

        [Required]
        public Guid? SenderProfileId { get; set; }

        [Required]
        public Guid? ReceiverProfileId { get; set; }

        [Required, MaxLength(100)]
        public string? Title { get; set; }

        [Required, MaxLength(500)]
        public string? Message { get; set; }

        [Required, MaxLength(50)]
        public string? NotificationType { get; set; }
    }

    public class ChatListResDTO
    {
        public long ChatRoomId { get; set; } 
        public Guid OppositeProfileId { get; set; } 
        public string? FullName { get; set; } 
        public string? ProfileFilePath { get; set; } 
        public string? LastMessage { get; set; } 
        public DateTime? LastMessageTime { get; set; } 
        public int UnreadCount { get; set; } 
    }
}
