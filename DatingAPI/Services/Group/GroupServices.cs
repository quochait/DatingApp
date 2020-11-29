using DatingAPI.Models.Group;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace DatingAPI.Services.Group
{
  public class GroupServices : IGroupServices
  {
    private IMongoCollection<GroupModel> _groupCollection;
    public GroupServices(IConfiguration _config)
    {
      var _mongoClient = new MongoClient(_config.GetSection("DatingSettings:ConnectionString").Value);
      var _database = _mongoClient.GetDatabase(_config.GetSection("DatingSettings:DatabaseName").Value);
      _groupCollection = _database.GetCollection<GroupModel>(typeof(GroupModel).Name);
    }

    //public async Task<bool> AddGroup(string userId, string toUserId)
    //{
    //  GroupModel group = new GroupModel();
    //  group.UserId = userId;
    //  group.ToUserId = userId;
    //  try
    //  {
    //    await _groupCollection.InsertOneAsync(group);
    //    return true;
    //  }
    //  catch (Exception)
    //  {
    //    return false;
    //  }
    //}

    public async Task<GroupModel> GetGroup(string userId, string toUserId)
    {
      FilterDefinition<GroupModel> filter = Builders<GroupModel>.Filter.Where(g => g.UserId == userId && g.ToUserId == toUserId);
      try
      {
        GroupModel group = await _groupCollection.Find(filter).FirstOrDefaultAsync();
        if (group == null)
        {
          filter = Builders<GroupModel>.Filter.Where(g => g.UserId == toUserId && g.ToUserId == userId);
          group = await _groupCollection.Find(filter).FirstOrDefaultAsync();
          if (group == null)
          {
            return null;
          }

          return group;
        }

        return group;
      }
      catch (Exception)
      {
        return null;
      }
    }

    public async Task<bool> InitGroup(string userId, string toUserId)
    {
      GroupModel groupModel = await GetGroup(userId, toUserId);

      if (groupModel == null)
      {
        GroupModel group = new GroupModel() { UserId = userId, ToUserId = toUserId };
        try
        {
          await _groupCollection.InsertOneAsync(group);
          return true;
        }
        catch (Exception)
        {
          return false;
        }
      }

      return true;
    }
  }
}
