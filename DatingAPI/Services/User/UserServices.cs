using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using DatingAPI.Helpers;
using DatingAPI.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using DatingAPI.Models.Relationship;
using System.Collections.Generic;
using DatingAPI.Enumerate;
using DatingAPI.Models.Group;
using DatingAPI.Services.Group;
using DatingAPI.Services.Message;
using DatingAPI.Models.Message;

namespace DatingAPI.Data
{
  public class UserServices : IUserServices
  {
    private readonly IMongoCollection<UserModel> _userCollection;
    private readonly IMongoCollection<RelationshipModel> _relationshipCollection;
    private IGroupServices _groupservice;
    private IMessageServices _messageServices;

    public UserServices(IConfiguration _config,
      IGroupServices groupservice,
      IMessageServices messageServices
      )
    {
      var _mongoClient = new MongoClient(_config.GetSection("DatingSettings:ConnectionString").Value);
      var _database = _mongoClient.GetDatabase(_config.GetSection("DatingSettings:DatabaseName").Value);
      _userCollection = _database.GetCollection<UserModel>(typeof(UserModel).Name);
      _relationshipCollection = _database.GetCollection<RelationshipModel>(typeof(RelationshipModel).Name);
      _groupservice = groupservice;
      _messageServices = messageServices;
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

    public async Task<PagedList<UserModel>> GetUsers(string userId, UserParams userParams)
    {
      FilterDefinition<UserModel> filter = Builders<UserModel>.Filter.Ne(u => u.ObjectId, userId);
      var users = _userCollection.Find(filter).ToList();

      return await PagedList<UserModel>.CreateAsync(users.AsQueryable(), userParams.PageNumber, userParams.PageSize);
    }

    public async Task UpdateActivity(string userId)
    {
      FilterDefinition<UserModel> filter = Builders<UserModel>.Filter.Eq(u => u.ObjectId, userId);
      var update = Builders<UserModel>.Update.Set(u => u.LastActive, DateTime.Now);

      UserModel user = await _userCollection.FindOneAndUpdateAsync(filter, update);
    }

    public async Task<bool> CreateMatchToUser(string userId, string userDest)
    {
      RelationshipModel relation = new RelationshipModel();
      relation.FromUserId = userId;
      relation.ToUserId = userDest;

      try
      {
        await _relationshipCollection.InsertOneAsync(relation);
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public async Task<RelationshipModel> GetStatus(string userId, string userDest)
    {
      FilterDefinition<RelationshipModel> filter = Builders<RelationshipModel>.Filter.Where(r => r.FromUserId == userId && r.ToUserId == userDest);

      RelationshipModel relationship = await _relationshipCollection.Find(filter).FirstOrDefaultAsync();
      return relationship;
    }

    public async Task<List<UserModel>> GetMatches(string userId)
    {
      List<UserModel> users = new List<UserModel>();
      FilterDefinition<RelationshipModel> filter = Builders<RelationshipModel>.Filter.Eq(u => u.Status, EnumRelationships.Matched.ToString());
      var relationships = await _relationshipCollection.Find(filter).ToListAsync();
      List<MessageModel> messages = new List<MessageModel>();
      List<GroupModel> groups = new List<GroupModel>();

      foreach (RelationshipModel relationship in relationships)
      {
        FilterDefinition<UserModel> filterUser;

        if (relationship.FromUserId == userId)
        {
          filterUser = Builders<UserModel>.Filter.Eq(u => u.ObjectId, relationship.ToUserId);
        }
        else
        {
          filterUser = Builders<UserModel>.Filter.Eq(u => u.ObjectId, relationship.FromUserId);
        }

        UserModel user = await _userCollection.Find(filterUser).FirstOrDefaultAsync();



        if (user != null && !users.Any(u => u.ObjectId == user.ObjectId))
        {
          users.Add(user);
          GroupModel group = await _groupservice.GetGroup(userId, user.ObjectId);
          groups.Add(group);
          MessageModel result = await _messageServices.GetLastMessageFromGroup(group.ObjectId);
          if (result != null)
          {
            messages.Add(result);
          }
        }
      }

      if (messages.Count > 0)
      {
        messages = messages.OrderByDescending(m => m.CreatedAt).ToList();

        List<UserModel> returnUsers = new List<UserModel>();
        foreach (MessageModel message in messages)
        {
          foreach (GroupModel group in groups)
          {
            foreach (UserModel user in users)
            {
              if (message.GroupId == group.ObjectId && (user.ObjectId == group.ToUserId || user.ObjectId == group.UserId))
              {
                if (!returnUsers.Any(r => r.ObjectId == user.ObjectId))
                {
                  returnUsers.Add(user);
                }
              }
            }
          }
        }

        return returnUsers;
      }

      else
      {
        return users;
      }
    }

    public async Task<bool> IsVerify(string email)
    {
      FilterDefinition<UserModel> filter = Builders<UserModel>.Filter.Where(u => u.Email == email && u.IsVerifyEmail == true);
      UserModel _ = await _userCollection.Find(filter).FirstOrDefaultAsync();
      if (_ == null)
      {
        return false;
      }
      return true;
    }

    public async Task<List<UserModel>> GetRequestMatches(string userId)
    {
      try
      {
        FilterDefinition<RelationshipModel> filter = Builders<RelationshipModel>
        .Filter.Where(r => r.Status == EnumRelationships.Pending.ToString() && r.FromUserId == userId);
        List<RelationshipModel> relationships = await _relationshipCollection.Find(filter).ToListAsync();
        if (relationships != null)
        {
          List<UserModel> users = new List<UserModel>();
          foreach (RelationshipModel relationship in relationships)
          {
            FilterDefinition<UserModel> filterUser = Builders<UserModel>
              .Filter.Where(u => u.ObjectId.ToString() == relationship.FromUserId.ToString());
            UserModel user = await _userCollection.Find(filterUser).FirstOrDefaultAsync();
            users.Add(user);
          }

          return users;
        }

        return null;
      }
      catch (Exception)
      {
        return null;
      }
    }

    public async Task<bool> UpdateStatusMatched(string userId, string fromUserId)
    {
      try
      {
        FilterDefinition<RelationshipModel> filter = Builders<RelationshipModel>
          .Filter.Where(r => r.ToUserId == userId && r.FromUserId == fromUserId && r.Status == EnumRelationships.Pending.ToString());
        UpdateDefinition<RelationshipModel> update = Builders<RelationshipModel>.Update.Set(r => r.Status, EnumRelationships.Matched.ToString());
        RelationshipModel relationship = await _relationshipCollection.FindOneAndUpdateAsync(filter, update);

        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }
  }
}
