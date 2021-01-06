
using DatingAPI.Enumerate;
using DatingAPI.Models.Relationship;
using DatingAPI.Models.Result;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAPI.Services.Relationship
{
  public class RelationshipService : IRelationshipService
  {
    private IMongoCollection<RelationshipModel> _relationshipCollection;

    public RelationshipService(IConfiguration _config)
    {
      var _mongoClient = new MongoClient(_config.GetSection("DatingSettings:ConnectionString").Value);
      var _database = _mongoClient.GetDatabase(_config.GetSection("DatingSettings:DatabaseName").Value);
      _relationshipCollection = _database.GetCollection<RelationshipModel>(typeof(RelationshipModel).Name);
    }

    public async Task<List<RelationshipModel>> GetRequestsPending(string userId)
    {
      try
      {
        FilterDefinition<RelationshipModel> filter = Builders<RelationshipModel>
          .Filter.Where(r => r.ToUserId == userId && r.Status == EnumRelationships.Pending.ToString());

        List<RelationshipModel> relationships = await _relationshipCollection.Find(filter).ToListAsync();

        return relationships;
      }
      catch (Exception)
      {
        return null;
      }
    }

    public async Task<bool> UpdateRequestStatus(string userId, string fromUserId, string status)
    {
      try
      {
        FilterDefinition<RelationshipModel> filter = Builders<RelationshipModel>
          .Filter.Where(r => r.ToUserId == userId && r.FromUserId == fromUserId);
        UpdateDefinition<RelationshipModel> update = Builders<RelationshipModel>
          .Update.Set(r => r.Status, status);

        _ = await _relationshipCollection.UpdateOneAsync(filter, update);
        return true;

      }
      catch (Exception)
      {
        return false;
      }
    }

    public async Task<RelationshipModel> CheckHaveRequest(string userId, string toUserId)
    {
      FilterDefinition<RelationshipModel> filter = Builders<RelationshipModel>
         .Filter.Where(r => (r.ToUserId == userId && r.FromUserId == toUserId) || (r.FromUserId == userId && r.ToUserId == toUserId));
      var result = await _relationshipCollection.Find(filter).FirstOrDefaultAsync();
      return result;
    }

    public async Task<bool> RemoveRelationship(string userId, string toUserId)
    {
      try
      {
        FilterDefinition<RelationshipModel> filter = Builders<RelationshipModel>
        .Filter.Where(r => r.ToUserId == userId && r.FromUserId == toUserId && r.Status == EnumRelationships.Pending.ToString());
        var result = await _relationshipCollection.DeleteOneAsync(filter);
        return true;
      }
      catch (Exception)
      {
        return false;
      }
      
    }
  }
}
