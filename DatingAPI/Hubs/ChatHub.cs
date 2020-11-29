using DatingAPI.Models.Group;
using DatingAPI.Models.Message;
using DatingAPI.Models.Utils;
using DatingAPI.Services.Group;
using DatingAPI.Services.Message;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatingAPI.Hubs
{
  [Authorize]
  public class SignalrHub : Hub
  {
    //public List<ConnectionModel> _connection;
    private IMessageServices _messageServices;
    private IGroupServices _groupServices;

    public SignalrHub(
      IGroupServices groupServices,
      IMessageServices messageServices
    )
    {
      _groupServices = groupServices;
      _messageServices = messageServices;
    }

    public async Task SendMessage(string toUserId, string msg)
    {
      string userId = Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;

      //add content chat to database
      GroupModel groupModel = await _groupServices.GetGroup(userId, toUserId);

      if (groupModel == null)
      {
        await _groupServices.InitGroup(userId, toUserId);
        groupModel = await _groupServices.GetGroup(userId, toUserId);
      }

      MessageModel message = new MessageModel(groupModel.ObjectId.ToString(), userId, toUserId, msg);

      await Clients.User(toUserId).SendAsync("UpdateMessage", message);
      await Clients.User(userId).SendAsync("UpdateMessage", message);

      await _messageServices.Insert(message);
    }

    public override async Task OnConnectedAsync()
    {
      await Groups.AddToGroupAsync(Context.ConnectionId, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
      await base.OnConnectedAsync();
    }
    public override async Task OnDisconnectedAsync(Exception ex)
    {
      await Groups.AddToGroupAsync(Context.ConnectionId, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
      await base.OnDisconnectedAsync(ex);
    }
  }
}
