using DatingAPI.Models;
using DatingAPI.Models.Authen;
using DatingAPI.Models.Result;
using System.Threading.Tasks;

namespace DatingAPI.Data
{
  public interface IAuthenticationServices
  {
    Task<string> Register(UserModel user, string password);
    Task<string> Login(string email, string password);
    Task<bool> UserExists(string email);
    ResultModel CheckTokenEmail(string userId, string token);
    ResultModel CheckTokenReset(string userId, string token);
    bool SendTokenVerifyEmail(string userId, string email);
    bool SendTokenResetPassword(string userid, string email);

    Task<bool> UpdatePassword(string userId, string newPassword);
    Task<AuthenticationModel> GetUser(string email);
  }
}
