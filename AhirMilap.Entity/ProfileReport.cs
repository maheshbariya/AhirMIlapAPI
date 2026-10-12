namespace AhirMilap.Entity
{
    public class ProfileReport : BaseEntity
    {
        public Guid? ProfileIdFrom { get; set; }
        public Guid? ProfileIdTo { get; set; }
        public string? Reason { get; set; } = string.Empty;
        public string? Comments { get; set; } = string.Empty ;
    }
}