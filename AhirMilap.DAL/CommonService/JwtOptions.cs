using Microsoft.IdentityModel.Tokens;

namespace AhirMilap.DAL.CommonService
{
    public class JwtOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Subject { get; set; }
        public byte[] AccessSecret { get; set; }
        public byte[] RefreshSecret { get; set; }
        public DateTime IssuedAt => DateTime.Now;
        public TimeSpan AccessValidFor { get; set; } = TimeSpan.FromMinutes(60);
        public TimeSpan RefreshValidFor { get; set; } = TimeSpan.FromMinutes(43200);
        public DateTime NotBefore => DateTime.Now;
        public DateTime AccessExpiration => IssuedAt.Add(AccessValidFor);
        public DateTime RefreshExpiration => IssuedAt.Add(RefreshValidFor);
        public SigningCredentials AccessSigningCredentials { get; set; }
        public SigningCredentials RefreshSigningCredentials { get; set; }
    }
}
