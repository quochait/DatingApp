using DatingAPI.Models.Base;

namespace DatingAPI.Models.Group
{
  public class GroupModel : BaseEntity
  {
    public string Status { get; set; }
    public string UserId { get; set; }
    public string ToUserId { get; set; }
  }
}
