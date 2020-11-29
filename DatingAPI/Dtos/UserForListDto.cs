using System;

namespace DatingAPI.Dtos
{
  public class UserForListDto
  {
    public string ObjectId { get; set; }
    public string Username { get; set; }
    public string Gender { get; set; }
    public int Age { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastActive { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string MainPhotoUrl { get; set; }
  }
}
