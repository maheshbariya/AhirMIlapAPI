namespace AhirMilap.Entity
{
    public class Connection : BaseEntity
    {
        public Guid? ProfileIdFrom { get; set; }
        public Guid? ProfileIdTo { get; set; }
    }
}