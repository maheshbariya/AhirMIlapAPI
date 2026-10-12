namespace AhirMilap.Entity
{
    public class FirebaseToken : BaseEntity
    {
        public string? UserId { get; set; }
        public string? FcmToken { get; set; }
        public bool IsLogOut { get; set; }

        public string? ReceiverName { get; set; }
        public string? SenderName { get; set; }
    }
}
