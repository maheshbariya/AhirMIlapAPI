namespace AhirMilap.Entity
{
    public class FavoriteUser : BaseEntity
    { 
        public Guid? ProfileIdFrom { get; set; }
        public Guid? ProfileIdTo { get; set; }
    }
}
