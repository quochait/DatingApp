using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace DatingAPI.Models.Base
{
  public class BaseEntity
  {
    [BsonId()]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ObjectId { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
    [BsonDateTimeOptions]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
  }
}
