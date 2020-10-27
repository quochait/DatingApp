using DatingAPI.Models;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DatingAPI.Services.Photo
{
  public class PhotoServices : IPhotoServices
  {
    private IMongoCollection<PhotoModel> _photoCollection;

    public PhotoServices(IConfiguration _config)
    {
      var _mongoClient = new MongoClient(_config.GetSection("DatingSettings:ConnectionString").Value);
      var _database = _mongoClient.GetDatabase(_config.GetSection("DatingSettings:DatabaseName").Value);
      _photoCollection = _database.GetCollection<PhotoModel>(typeof(PhotoModel).Name);
    }

    public async Task<bool> AddPhoto(PhotoModel photoModel)
    {
      try
      {
        await _photoCollection.InsertOneAsync(photoModel);
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public async Task<List<PhotoModel>> GetPhotos(string userId)
    {
      FilterDefinition<PhotoModel> filter = Builders<PhotoModel>.Filter.Eq(p => p.UserId, userId);
      var result = await _photoCollection.Find(filter).ToListAsync();
      return result;
    }

    public Task<string> GetPhoto(string photoId)
    {
      throw new NotImplementedException();
    }
  }
}
