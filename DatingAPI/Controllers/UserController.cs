using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingAPI.Data;
using DatingAPI.Dtos;
using DatingAPI.Helpers;
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
    private readonly IUserServices _userServices;
    private IMapper _mapper;

    public UserController(IUserServices userServices, IMapper mapper)
    {
      _userServices = userServices;
      _mapper = mapper;
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
      var users = await _userServices.GetUsers(userParams);
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
    public async Task<IActionResult> SetMainPhoto(string userId, [FromForm]string photoUrl)
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
  }
}
