namespace AhirMilap.DTO
{
    public class Token
    {
        //public double Expires_in;
        public DateTime? Expires_in;

        public string Access_token;

        public string Refresh_token;
    }

    public class UserAccDTO
    {
        public string id { get; set; }
        public string firstName { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }
        public IList<string>? UserRolesLst { get; set; }
        public int ProfileStep { get; set; } = 0;
    }

    public class LoginResDTO
    {
        public Token? Tokens { get; set; }
        public UserAccDTO? Users { get; set; }
    }
}
