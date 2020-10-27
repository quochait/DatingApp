using DatingAPI.Models.Base;

namespace DatingAPI.Models.Group
{
  public class GroupModel : BaseEntity
  {
    public string Name { get; set; }
    public string Status { get; set; }
  }
}
