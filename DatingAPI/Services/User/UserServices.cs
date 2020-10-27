using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingAPI.Helpers;
using DatingAPI.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace DatingAPI.Data
{
  public class UserServices : IUserServices
  {
    private readonly IMongoCollection<UserModel> _userCollection;
   
    public UserServices(IConfiguration _config)
    {
      var _mongoClient = new MongoClient(_config.GetSection("DatingSettings:ConnectionString").Value);
      var _database = _mongoClient.GetDatabase(_config.GetSection("DatingSettings:DatabaseName").Value);
      _userCollection = _database.GetCollection<UserModel>(typeof(UserModel).Name);
    }

    public async Task<UserModel> GetUser(string userId)
    {
      FilterDefinition<UserModel> filter = Builders<UserModel>.Filter.Eq(m => m.ObjectId, userId);
      UserModel user = await _userCollection.Find(filter).FirstOrDefaultAsync();
      if (user != null)
      {
        return user;
      }
      return null;
    }

    public async Task<bool> UpdateUser(string userId, UserModel userForUpdate)
    {
      try
      {
        FilterDefinition<UserModel> filter = Builders<UserModel>.Filter.Eq(m => m.ObjectId, userId);
        await _userCollection.ReplaceOneAsync(filter, userForUpdate);

        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public async Task<PagedList<UserModel>> GetUsers(UserParams userParams)
    {
      List<UserModel> users = _userCollection.Find(x => true).ToList();
      return await PagedList<UserModel>.CreateAsync(users.AsQueryable(), userParams.PageNumber, userParams.PageSize);
    }

    public async Task<bool> AddUser(UserModel user)
    {
      try
      {
        await _userCollection.InsertOneAsync(user);
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public async Task UpdateActivity(string userId)
    {
      FilterDefinition<UserModel> filter = Builders<UserModel>.Filter.Eq(u => u.ObjectId, userId);
      var update = Builders<UserModel>.Update.Set(u => u.LastActive, DateTime.Now);

      UserModel user = await _userCollection.FindOneAndUpdateAsync(filter, update);
    }
  }
}
