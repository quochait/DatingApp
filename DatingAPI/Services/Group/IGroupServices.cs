using DatingAPI.Models.Group;
using System.Threading.Tasks;

namespace DatingAPI.Services.Group
{
  public interface IGroupServices
  {
    Task<GroupModel> GetGroup(string userId, string toUserId);
    Task<bool> InitGroup(string userId, string toUserId);
  }
}
