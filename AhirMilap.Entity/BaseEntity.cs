namespace AhirMilap.Entity
{
    public class BaseEntity
    {
        public Guid? Id { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
