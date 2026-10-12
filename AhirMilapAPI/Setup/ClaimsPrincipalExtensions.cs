using System.Security.Claims;

namespace AhirMilapAPI.Setup
{
    public static class ClaimsPrincipalExtensions
    {
        //it's get claims and return userId
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            string stringId = principal.FindFirst(ClaimTypes.Sid)?.Value;

            return stringId;
        }

        public static Guid? GetProfileId(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            string profileId = principal.FindFirst("ProfileId")?.Value;

            return Guid.TryParse(profileId, out Guid id) ? id : null;
        }
    }
}
