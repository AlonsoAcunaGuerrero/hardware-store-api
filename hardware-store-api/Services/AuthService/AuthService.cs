using hardware_store_api.Models;
using hardware_store_api.Exceptions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Net;

namespace hardware_store_api.Services.AuthService
{
    public class AuthService : IAuthService
    {
        public string GetToken(User user, DateTime expire, string secretKey, string role = "User")
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = expire,
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, role),
                }),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

        public string GetTokenEmail(ClaimsIdentity identity)
        {
            if(identity == null)
            {
                throw new HttpStatusException(HttpStatusCode.BadRequest, "The token not contain claims.");
            }

            var claimEmail = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);

            if (claimEmail == null)
            {
                throw new HttpStatusException(HttpStatusCode.BadRequest, "The token not contain user's email.");
            }

            return claimEmail.Value;
        }
    }
}
