using DatingAPI.Models.Base;

namespace DatingAPI.Models
{
  public class PhotoModel : BaseEntity
  {
    public string Url { get; set; }
    public string Description { get; set; }
    public string PublicId { get; set; }
    public string UserId { get; set; }
  }
}