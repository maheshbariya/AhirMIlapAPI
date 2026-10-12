using System.ComponentModel.DataAnnotations;

namespace AhirMilap.Entity
{
    public class Company : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string CompanyName { get; set; }

        [MaxLength(100)]    
        public string? GSTNo { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EnstablishedFrom { get; set; }
        public int? CountryIdRef { get; set; }
        public int? StateIdRef { get; set; }
        public int? CityIdRef { get; set; }
        
        [MaxLength(500)]
        public string? Address1 { get; set; }

        [MaxLength(500)]
        public string? Address2 { get; set; }

        [MaxLength(10)]
        public string? PinCode { get; set; }
        
        [MaxLength(200)]
        public string? Email { get; set; }
        
        [MaxLength(15)]
        public string? ContactNo { get; set; }
        public string? UserIdRef { get; set; }

        [MaxLength(200)]
        public string? CompanyProfile { get; set; }

        public Guid? BankAccIdRef { get; set; }
    }
}
