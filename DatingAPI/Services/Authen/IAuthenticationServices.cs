using DatingAPI.Models;
using System.Threading.Tasks;

namespace DatingAPI.Data
{
  public interface IAuthenticationServices
  {
    Task<string> Register(UserModel user, string password);
    Task<string> Login(string email, string password);
    Task<bool> UserExists(string email);
    Task<bool> VerifyEmail(string email);
  }
}
