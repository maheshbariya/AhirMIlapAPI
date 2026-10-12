namespace AhirMilap.Entity
{
    public class Siblings
    {
        public Guid Id { get; set; }
        public Guid ProfileId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Relation { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}
