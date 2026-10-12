using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AhirMilap.DTO;

namespace AhirMilap.DAL.CommonService
{
    public class JwtManager
    {
        public readonly JwtOptions jwtOptions;

        public JwtManager(IOptions<JwtOptions> jwtOptions)
        {
            this.jwtOptions = jwtOptions.Value;
        }
         

        public Token GetToken(List<Claim> authClaims)
        {
            var jwtAccess = new JwtSecurityToken(
               issuer: jwtOptions.Issuer,
               audience: jwtOptions.Audience,
               expires: jwtOptions.AccessExpiration, 
               claims: authClaims,
               signingCredentials: jwtOptions.AccessSigningCredentials
               );

            var jwtRefresh = new JwtSecurityToken(
               issuer: jwtOptions.Issuer,
               audience: jwtOptions.Audience,
               expires: jwtOptions.RefreshExpiration,
               claims: authClaims,
               signingCredentials: jwtOptions.RefreshSigningCredentials
               );

            var encodedJwtA = new JwtSecurityTokenHandler().WriteToken(jwtAccess);
            var encodedJwtR = new JwtSecurityTokenHandler().WriteToken(jwtRefresh);
            var token = new Token
            {
                //Expires_in = jwtOptions.AccessValidFor.TotalMilliseconds,
                Expires_in = jwtOptions.AccessExpiration,
                Access_token = encodedJwtA,
                Refresh_token = encodedJwtR
            };

            return token;
        }
    }
}
