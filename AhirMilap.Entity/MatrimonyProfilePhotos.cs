namespace AhirMilap.Entity
{
    public class MatrimonyProfilePhotos : BaseEntity
    {
        public Guid? ProfileId { get; set; }
        public string? PhotoPath { get; set; }
        public int? SortOrder { get; set; }
    }
}
