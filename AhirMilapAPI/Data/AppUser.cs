using Microsoft.AspNetCore.Identity;

namespace AhirMilapAPI.Data
{
    public class AppUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? OTP { get; set; }
        public DateTime? OTPExpiry { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
