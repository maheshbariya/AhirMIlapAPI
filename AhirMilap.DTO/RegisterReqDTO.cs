using System.ComponentModel.DataAnnotations;

namespace AhirMilap.DTO
{
    public class RegisterReqDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        //[EmailAddress(ErrorMessage = "Invalid Email Address.")]
        //[RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress.")]
        public string Email { get; set; } //pass email or mobile

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
        public bool IsResend { get; set; }
        public string? Phone { get; set; }
    }


    public class DrpCountryResDTO
    {
        public int? Id { get; set; }
        public string? CountryName { get; set; }
    }

    public class DrpStateResDTO
    {
        public int? Id { get; set; }
        public string? StateName { get; set; }
    }

    public class DrpCityResDTO
    {
        public int? Id { get; set; }
        public string? CityName { get; set; }
    }

    public class DrpGSTResDTO
    {
        public Guid? Id { get; set; }
        public string? GST { get; set; }
    }
}
