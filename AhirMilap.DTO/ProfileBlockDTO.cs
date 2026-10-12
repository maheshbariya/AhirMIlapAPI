using System.ComponentModel.DataAnnotations;

namespace AhirMilap.DTO
{
    public class ProfileBlockReqDTO
    {
        [Required]
        public Guid? ProfileIdFrom { get; set; }
        [Required]
        public Guid? ProfileIdTo { get; set; }
    }
    public class ProfileBlockResDTO
    {
        public Guid? Id { get; set; }
        public Guid? ProfileIdTo { get; set; }
        public string? FullName { get; set; }
        public string? ProfileFilePath { get; set; }
        public string? Occupation { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
