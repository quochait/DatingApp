using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingAPI.Data;
using DatingAPI.Dtos;
using DatingAPI.Helpers;
using DatingAPI.Models.Relationship;
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

    public UserController(IUserServices userServices, IMapper mapper, IAuthenticationServices authenticationServices)
    {
      _userServices = userServices;
      _mapper = mapper;
      _authenticationServices = authenticationServices;
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
  }
}
