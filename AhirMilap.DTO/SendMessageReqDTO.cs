using System.ComponentModel.DataAnnotations;

namespace AhirMilap.DTO
{
    public class SendMessageReqDTO
    {
        [Required]
        public long ChatRoomId { get; set; }

        [Required]
        public Guid? SenderProfileId { get; set; }
        public Guid? ReceiverProfileId { get; set; }

        [Required]
        public string MessageText { get; set; }
    }

    public class CreateRoomReqDTO
    {
        [Required]
        public Guid? Profile1Id { get; set; }

        [Required]
        public Guid? Profile2Id { get; set; }
    }

    public class ChatMessageResDTO
    {
        public int TotalCount { get; set; }
        public List<ChatMessageRes> Messages { get; set; }
    }

    public class ChatMessageRes
    {
        public long Id { get; set; }
        public long ChatRoomId { get; set; }
        public Guid? SenderProfileId { get; set; }
        public string MessageText { get; set; }
        public bool IsRead { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
