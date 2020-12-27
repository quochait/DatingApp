using DatingAPI.Models;
using System.Threading.Tasks;
using DatingAPI.Helpers;
using DatingAPI.Models.Relationship;
using System.Collections.Generic;
using DatingAPI.Dtos;

namespace DatingAPI.Data
{
  public interface IUserServices
  {
    Task<PagedList<UserModel>> GetUsers(string userId, UserParams userParams);
    Task<UserModel> GetUser(string userId);
    Task<bool> UpdateUser(string userId, UserModel userForUpdate);
    Task UpdateActivity(string userId);
    Task<bool> CreateMatchToUser(string userId, string userDest);
    Task<RelationshipModel> GetStatus(string userId, string userDest);
    Task<List<UserModel>> GetMatches(string userId);
    Task<bool> IsVerify(string email);
    Task<List<UserModel>> GetRequestMatches(string userId);
    Task<bool> UpdateStatusMatched(string userId, string userDest);
  }
}
