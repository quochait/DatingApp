using DatingAPI.Models.Base;

namespace DatingAPI.Models.Notification
{
  public class NotificationModel : BaseEntity
  {
    public string UserId { get; set; }
    public string Content { get; set; }
  }
}
