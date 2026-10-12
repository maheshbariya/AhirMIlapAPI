using Microsoft.Extensions.Configuration;

namespace AhirMilap.DAL.CommonService
{
    public class GenerateDomainURL
    {
        private readonly string _domainPath;

        public GenerateDomainURL(IConfiguration configuration)
        {
            _domainPath = configuration["Domain:Path"] ?? string.Empty;
        }

        public string GetFullUrl(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return string.Empty;

            filePath = filePath.Replace("\\", "/");

            return $"{_domainPath.TrimEnd('/')}/{filePath.TrimStart('/')}";
        }
    }
}