using System.ComponentModel.DataAnnotations;

namespace AhirMilap.DTO
{
    public class ProfileReportReqDTO
    {
        [Required]
        public Guid? ProfileIdFrom { get; set; }
        [Required]
        public Guid? ProfileIdTo { get; set; }
        [Required]
        public string? Reason { get; set; }
        public string? Comments { get; set; } = string.Empty;
    }
    public class ProfileReportResDTO
    {
        public Guid? Id { get; set; }
        public Guid? ProfileIdTo { get; set; }
        public string? FullName { get; set; }
        public string? ProfileFilePath { get; set; }
        public string? Occupation { get; set; }
        public string? Reason { get; set; }
        public string? Comments { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
