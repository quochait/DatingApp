using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

namespace DatingAPI.MongoHelper
{
  public class MongoContext : IMongoContext
  {
    public MongoClient _mongoClient { get; set; }
    public IMongoDatabase _database { get; set; }

    public MongoContext(IConfiguration _config)
    {
      _mongoClient = new MongoClient(_config.GetSection("DatingSettings:ConnectionString").Value);
      _database = _mongoClient.GetDatabase(_config.GetSection("DatingSettings:DatabaseName").Value);
    }

    public IMongoCollection<T> GetCollection<T>(string name)
    {
      return _database.GetCollection<T>(name);
    }
  }
}
