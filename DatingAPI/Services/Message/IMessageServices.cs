using DatingAPI.Models.Message;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingAPI.Services.Message
{
  public interface IMessageServices
  {
    Task<MessageModel> GetLastMessageFromGroup(string objectId);
    Task<bool> Insert(MessageModel message);
    Task<List<MessageModel>> GetMessages(string groupId);
  }
}
