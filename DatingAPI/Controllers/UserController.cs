using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingAPI.Data;
using DatingAPI.Dtos;
using DatingAPI.Enumerate;
using DatingAPI.Helpers;
using DatingAPI.Models;
using DatingAPI.Models.Group;
using DatingAPI.Models.Message;
using DatingAPI.Models.Relationship;
using DatingAPI.Services.Group;
using DatingAPI.Services.Message;
using DatingAPI.Services.Relationship;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingAPI.Controllers
{
  [Route("api/[controller]")]
  [ServiceFilter(typeof(LogUserActivity))]
  [Authorize]
  [ApiController]
  public class UserController : ControllerBase
  {
    private IMapper _mapper;
    private readonly IUserServices _userServices;
    private readonly IAuthenticationServices _authenticationServices;
    private readonly IRelationshipService _relationshipService;
    private readonly string UserId;
    private readonly IGroupServices _groupService;
    private IMessageServices _messageServices;
    public UserController(
      IUserServices userServices,
      IMapper mapper,
      IAuthenticationServices authenticationServices,
      IRelationshipService relationshipService,
      IMessageServices messageServices,
      IGroupServices groupServices)
    {
      _userServices = userServices;
      _mapper = mapper;
      _authenticationServices = authenticationServices;
      _relationshipService = relationshipService;
      _groupService = groupServices;
      _messageServices = messageServices;
      //UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUser(string userId)
    {
      var user = await _userServices.GetUser(userId);
      var userToReturn = _mapper.Map<UserForDetailedDto>(user);
      return Ok(userToReturn);
    }

    //get list user with param
    [HttpGet("getUsers")]
    public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
    {
      string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

      var users = await _userServices.GetUsers(userId, userParams);
      var usersToReturn = _mapper.Map<List<UserForListDto>>(users);

      Response.AddPagitation(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

      return Ok(usersToReturn);
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUser(string userId, [FromBody] UserForUpdateDto userForUpdateDto)
    {
      if (userId != User.FindFirst(ClaimTypes.NameIdentifier).Value)
      {
        return Unauthorized();
      }

      var user = await _userServices.GetUser(userId);
      _mapper.Map(userForUpdateDto, user);

      try
      {
        await _userServices.UpdateUser(userId, user);
        return NoContent();
      }
      catch (Exception)
      {
        throw new Exception($"Update failed user have id {userId}.");
      }
    }

    [HttpPost("{userId}/setMain")]
    public async Task<IActionResult> SetMainPhoto(string userId, [FromForm] string photoUrl)
    {
      if (userId != User.FindFirst(ClaimTypes.NameIdentifier).Value)
      {
        return Unauthorized();
      }

      var userFromRepo = await _userServices.GetUser(userId);

      if (userFromRepo.MainPhotoUrl == photoUrl)
      {
        return BadRequest("Photo already the main photo.");
      }

      userFromRepo.MainPhotoUrl = photoUrl;

      try
      {
        await _userServices.UpdateUser(userId, userFromRepo);
        return NoContent();
      }
      catch (Exception)
      {
        return BadRequest("Could set photo to main");
      }
    }

    [HttpGet("{userId}/getEmailVerify")]
    public async Task<IActionResult> GetEmailVerify(string userId)
    {
      var user = await _userServices.GetUser(userId);
      if (user == null)
      {
        BadRequest("Not found user.");
      }

      if (_authenticationServices.SendTokenVerifyEmail(user.ObjectId, user.Email))
      {
        return Ok();
      }

      return BadRequest("Can't send email to: " + user.Email);
    }

    [HttpGet("haveRequest/{toUserId}")]
    public async Task<IActionResult> IsHaveRequest(string toUserId)
    {
      string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value.ToString();
      RelationshipModel relationship = await _relationshipService.CheckHaveRequest(userId, toUserId);
      return Ok(relationship);
    }

    [HttpGet("{userId}/checkToken/{token}")]
    public IActionResult CheckTokenVerifyEmail(string userId, string token)
    {
      var result = _authenticationServices.CheckTokenEmail(userId, token);

      if (result.True)
      {
        return Ok();
      }

      return BadRequest(result.Error);
    }

    [HttpPost("{userId}/getMatch")]
    public async Task<IActionResult> CreateMatch(string userId, [FromForm] string userDest)
    {
      if (await _userServices.CreateMatchToUser(userId, userDest))
      {
        return Ok();
      }

      return BadRequest();
    }

    [HttpPost("{userId}/getStatusRelationship")]
    public async Task<IActionResult> GetStatusRelationship(string userId, [FromForm] string userDest)
    {
      RelationshipModel relationship = await _userServices.GetStatus(userId, userDest);

      return Ok(relationship);
    }

    [HttpGet("getRequestsMatches")]
    public async Task<IActionResult> GetRequestMatches()
    {
      string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
      List<UserModel> users = await _userServices.GetRequestMatches(userId);

      return Ok(users);
    }

    [HttpPost("updateStatusMatch")]
    public async Task<IActionResult> UpdateStatusMatchToUser(string fromUserId)
    {
      string userId = User.FindFirst(ClaimTypes.Name).Value;

      if (await _userServices.UpdateStatusMatched(userId, fromUserId))
      {
        return Ok();
      }
      else
      {
        return BadRequest();
      }
    }

    [HttpGet("getRequestsPending")]
    public async Task<IActionResult> GetRequestsPending()
    {
      string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
      RelationshipUsersDto relationshipToReturn = new RelationshipUsersDto();

      List<UserModel> users = new List<UserModel>();
      relationshipToReturn.Relationships = await _relationshipService.GetRequestsPending(userId);
      if (relationshipToReturn.Relationships != null)
      {
        foreach (RelationshipModel relationship in relationshipToReturn.Relationships)
        {
          UserModel user = await _userServices.GetUser(relationship.FromUserId.ToString());
          users.Add(user);
        }

        relationshipToReturn.Users = users;
      }

      return Ok(relationshipToReturn);
    }

    [HttpGet("updateRelationship/{toUserId}")]
    public async Task<IActionResult> UpdateRelationship(string toUserId)
    {
      string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
      UserModel user = await _userServices.GetUser(toUserId);
      if (user != null)
      {
        await _groupService.InitGroup(userId, toUserId);
        GroupModel group = await _groupService.GetGroup(userId, toUserId);
        MessageModel message = new MessageModel(group.ObjectId.ToString(), userId, toUserId, "");
        await _messageServices.Insert(message);
        bool result = await _relationshipService.UpdateRequestStatus(userId, toUserId, EnumRelationships.Matched.ToString());
        if (result)
        {
          return Ok();
        }
      }

      return BadRequest();
    }

    [HttpGet("denyrequest/{toUserId}")]
    public async Task<IActionResult> RemoveRequest(string toUserId)
    {
      string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value.ToString();
      var _ = await _relationshipService.RemoveRelationship(userId, toUserId);
      if (_)
      {
        return Ok();
      }
      return BadRequest();
    }
  }
}
