using DatingAPI.Models.Message;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAPI.Services.Message
{
  public class MessageServices : IMessageServices
  {
    private IMongoCollection<MessageModel> _messageCollection;

    public MessageServices(
      IConfiguration _config  
    )
    {
      var _mongoClient = new MongoClient(_config.GetSection("DatingSettings:ConnectionString").Value);
      var _database = _mongoClient.GetDatabase(_config.GetSection("DatingSettings:DatabaseName").Value);
      _messageCollection = _database.GetCollection<MessageModel>(typeof(MessageModel).Name);
    }
    public async Task<MessageModel> GetLastMessageFromGroup(string objectId)
    {
      try
      {
        FilterDefinition<MessageModel> filter = Builders<MessageModel>.
          Filter.Eq(m => m.GroupId, objectId);
        
        MessageModel messageModel = await _messageCollection.Find(filter).SortByDescending(m => m.CreatedAt).FirstOrDefaultAsync();

        return messageModel;
      }
      catch (Exception)
      {
        return null;
      }
    }

   
    public async Task<List<MessageModel>> GetMessages(string groupId)
    {
      try
      {
        FilterDefinition<MessageModel> filter = Builders<MessageModel>.Filter.Eq(m => m.GroupId, groupId);
        List<MessageModel> messages = await _messageCollection.Find(filter).ToListAsync();
        return messages;
      }
      catch (Exception)
      {
        return null;
      }
    }

    public async Task<bool> Insert(MessageModel message)
    {
      try
      {
        await _messageCollection.InsertOneAsync(message);
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }
  }
}
