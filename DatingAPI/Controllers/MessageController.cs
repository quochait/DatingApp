using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingAPI.Data;
using DatingAPI.Hubs;
using DatingAPI.Models;
using DatingAPI.Models.Group;
using DatingAPI.Services.Group;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using DatingAPI.Services.Message;
using DatingAPI.Models.Message;

namespace DatingAPI.Controllers
{
  [Route("api/[controller]")]
  [Authorize]
  [ApiController]
  public class MessageController : ControllerBase
  {
    private IUserServices _userService;
    private IHubContext<SignalrHub> _signalr;
    private IGroupServices _groupService;
    private IMessageServices _messageServices;

    public MessageController(
      IUserServices userServices,
      IMessageServices messageServices,
      IGroupServices groupServices,
      IHubContext<SignalrHub> signalrHub
    )
    {
      _userService = userServices;
      _signalr = signalrHub;
      _groupService = groupServices;
      _messageServices = messageServices;
    }

    [HttpGet("getMatches")]
    public async Task<IActionResult> GetMatches()
    {
      string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value.ToString();

      List<UserModel> users = await _userService.GetMatches(userId);

      return Ok(users);
    }

    [HttpGet("getLatestMessage/{toUserId}")]
    public async Task<IActionResult> LastMessage(string toUserId)
    {
      string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value.ToString();
      GroupModel groupModel = await _groupService.GetGroup(userId, toUserId);
      if (groupModel == null)
      {
        // create group after matched
        await _groupService.InitGroup(userId, toUserId);
        groupModel = await _groupService.GetGroup(userId, toUserId);
        return Ok(null);
      }

      MessageModel message = await _messageServices.GetLastMessageFromGroup(groupModel.ObjectId);
      return Ok(message);
    }

    [HttpGet("listMessages/{toUserId}")]
    public async Task<IActionResult> GetMessages(string toUserId)
    {
      string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value.ToString();
      GroupModel groupModel = await _groupService.GetGroup(userId, toUserId);
      //Khoong lay duoc group id
      if (groupModel == null)
      {
        await _groupService.InitGroup(userId, toUserId);
        groupModel = await _groupService.GetGroup(userId, toUserId);
      }

      List<MessageModel> messages = await _messageServices.GetMessages(groupModel.ObjectId.ToString());
      return Ok(messages);
    }
  }
}
