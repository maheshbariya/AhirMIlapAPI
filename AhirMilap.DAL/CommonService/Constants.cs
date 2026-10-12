namespace AhirMilap.DAL.CommonService
{
    public static class Constants
    {
        public static string GenerateOTP(int length)
        {
            // Ensure the length is valid for a 6-digit OTP
            if (length <= 0)
                throw new ArgumentException("Length must be greater than 0");

            // Create a new instance of Random
            Random random = new Random();

            // Create a variable to hold the OTP
            string otp = string.Empty;

            // Generate the OTP by concatenating random digits
            for (int i = 0; i < length; i++)
            {
                otp += random.Next(0, 10).ToString();
            }

            return otp;
        }
    }
}
