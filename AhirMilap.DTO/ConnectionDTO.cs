using System.ComponentModel.DataAnnotations;

namespace AhirMilap.DTO
{
    public class ConnectionReqDTO
    {
        [Required]
        public Guid? ProfileIdFrom { get; set; }
        [Required]
        public Guid? ProfileIdTo { get; set; }
    }
    public class ConnectionResDTO
    {
        public Guid? Id { get; set; }
        public Guid? ProfileIdTo { get; set; }
        public Guid? ProfileId { get; set; }
        public string? UserId { get; set; }
        public string? FullName { get; set; }
        public string? ProfileFilePath { get; set; }
        public string? Occupation { get; set; }
        public int? StatusId { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}