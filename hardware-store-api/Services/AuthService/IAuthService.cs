using hardware_store_api.Models;
using System.Security.Claims;

namespace hardware_store_api.Services.AuthService
{
    public interface IAuthService
    {
        string GetToken(User user, DateTime expire, string secretKey, string role = "User");
        string GetTokenEmail(ClaimsIdentity identity);
    }
}
