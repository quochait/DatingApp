namespace DatingAPI.Models
{
  public class CacheModel
  {
    public string UserId { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }

    public CacheModel(string userId, string name, string value)
    {
      UserId = userId;
      Name = name;
      Value = value;
    }

    public CacheModel()
    {

    }
  }
}
