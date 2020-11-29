using DatingAPI.Models.Base;

namespace DatingAPI.Models.Message
{
  public class MessageModel : BaseEntity
  {
    public string Content { get; set; }
    public string Status { get; set; }
    public string GroupId { get; set; }
    public string UserId { get; set; }
    //default sent
    public bool Type { get; set; } = true;
    public string PhotoId { get; set; }
    public MessageModel(string groupId, string userId, string toUserId, string msg)
    {
      Content = msg;
      UserId = userId;
      UserId = toUserId;
      GroupId = groupId;
    }
  }
}
