using System.ComponentModel.DataAnnotations;

namespace AhirMilap.DTO
{
    public class LoginReqDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        //[EmailAddress(ErrorMessage = "Invalid Email Address.")]
        //[RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress.")]
        public string Email { get; set; } //pass email or mobile

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }

    public class CreateUserReqDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid email.")]
        public string Email { get; set; } //pass email or mobile

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}
