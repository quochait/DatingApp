using System;

namespace DatingAPI.Dtos
{
  public class PhotoForReturnDto
  {
    public string Id { get; set; }
    public string Url { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public string PublicId { get; set; }
  }
}
