using DatingAPI.Models.Base;

namespace DatingAPI.Models.Message
{
  public class MessageModel : BaseEntity
  {
    public string Content { get; set; }
    public string Status { get; set; }
    public string GroupId { get; set; }
    public string UserId { get; set; }
    public int Type { get; set; }
    public string PhotoId { get; set; }
  }
}
