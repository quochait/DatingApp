using DatingAPI.Models;
using System.Threading.Tasks;
using DatingAPI.Helpers;

namespace DatingAPI.Data
{
  public interface IUserServices
  {
    Task<PagedList<UserModel>> GetUsers(UserParams userParams);
    Task<UserModel> GetUser(string userId);
    Task<bool> UpdateUser(string userId, UserModel userForUpdate);
    Task UpdateActivity(string userId);
  }
}
