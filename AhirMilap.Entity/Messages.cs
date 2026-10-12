namespace AhirMilap.Entity
{
    public class Messages
    {
        public long Id { get; set; }

        public long ChatRoomId { get; set; }

        public Guid? SenderProfileId { get; set; }

        public string MessageText { get; set; }

        public bool IsRead { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
    }
}
