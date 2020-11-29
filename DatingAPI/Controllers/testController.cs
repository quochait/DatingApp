using DatingAPI.Data;
using DatingAPI.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using DatingAPI.Helpers;
using DatingAPI.MongoHelper;
using AutoMapper;
using MongoDB.Driver;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace DatingAPI.Controllers
{
  [ServiceFilter(typeof(LogUserActivity))]
  //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [Route("api/[controller]")]
  [ApiController]
  public class testController : ControllerBase
  {
    private readonly IUserServices _userServices;
    private readonly IMapper _mapper;

    public testController(IUserServices userServices)
    {
      _userServices = userServices;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
    {
      var users = await _userServices.GetUsers("12312312", userParams);
      var usersToReturn = _mapper.Map<List<UserForListDto>>(users);

      Response.AddPagitation(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

      return Ok(usersToReturn);
    }

    [HttpGet]
    public async Task<IActionResult> GetUser(string userId)
    {
      //var user = await _userServices.GetUser(userId);
      //var userToReturn = _mapper.Map<UserForDetailedDto>(user);
      //return Ok(userToReturn);
      return Ok("test");
    }

    //[HttpPut("{id}")]
    //public async Task<IActionResult> UpdateUser(string userId, [FromBody] UserForUpdateDto userForUpdateDto)
    //{
    //  if (userId != User.FindFirst(ClaimTypes.NameIdentifier).Value)
    //  {
    //    return Unauthorized();
    //  }

    //  var user = await _userServices.GetUser(userId);
    //  _mapper.Map(userForUpdateDto, user);

    //  try
    //  {
    //    await _userServices.UpdateUser(userId, user);
    //    return NoContent();
    //  }
    //  catch (Exception)
    //  {
    //    throw new Exception($"Update failed user have id {userId}.");
    //  }
    //}
  }
}
