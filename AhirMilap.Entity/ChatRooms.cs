namespace AhirMilap.Entity
{
    public class ChatRooms
    {
        public int? Id { get; set; }
        public Guid? Profile1Id { get; set; }
        public Guid? Profile2Id { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
    }
}
