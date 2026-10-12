using System.ComponentModel.DataAnnotations;

namespace AhirMilap.DTO
{
    public class UpdatePasswordRequestDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress.")]
        public string Email { get; set; }
        public string Password { get; set; }
    }


    //update password
    public class ChangePassReqDTO
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[#$^+=!*()@%&_]).{7,30}$", ErrorMessage = "Please enter strong password ie. Abc@123")]
        public string NewPassword { get; set; }
    }

    public class SendOtpRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress.")]
        public string? Email { get; set; }

        public string? Phone { get; set; }

        public bool IsResend { get; set; }
    }

    public class VerifyOtpRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress.")]
        public string Email { get; set; }

        [Required]
        public int? otp { get; set; }
    }

}
