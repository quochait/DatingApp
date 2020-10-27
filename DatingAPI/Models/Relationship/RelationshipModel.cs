using DatingAPI.Models.Base;

namespace DatingAPI.Models.Relationship
{
  public class RelationshipModel : BaseEntity
  {
    public string FromUserId { get; set; }
    public string ToUserId { get; set; }
    public string Status { get; set; }
  }
}
