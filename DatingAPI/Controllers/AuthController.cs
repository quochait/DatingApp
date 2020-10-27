using AutoMapper;
using DatingAPI.Data;
using DatingAPI.Dtos;
using DatingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatingAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    private readonly IAuthenticationServices _authenServices;
    private readonly IConfiguration _config;
    private readonly IMapper _mapper;
    private readonly IUserServices _userServices;

    public AuthController(IAuthenticationServices authenServices, IConfiguration config, IMapper mapper, IUserServices userServices)
    {
      _authenServices = authenServices;
      _config = config;
      _mapper = mapper;
      _userServices = userServices;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserForRegisterDto userForRegisterDto)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      userForRegisterDto.Email = userForRegisterDto.Email;
      if (await _authenServices.UserExists(userForRegisterDto.Email))
      {
        return BadRequest("Email is used for other user.");
      }

      var userToCreate = _mapper.Map<UserModel>(userForRegisterDto);
      var userId = await _authenServices.Register(userToCreate, userForRegisterDto.Password);
      var userToReturn = _mapper.Map<UserForDetailedDto>(userToCreate);

      return CreatedAtRoute("GetUser", new { controller = "User", id = userId }, userToReturn);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
    {
      var userId = await _authenServices.Login(userForLoginDto.Email, userForLoginDto.Password);
      if (userId == null)
      {
        return Unauthorized();
      }

      var user = await _userServices.GetUser(userId);

      var claims = new[]
      {
        new Claim(ClaimTypes.NameIdentifier, userId),
        new Claim(ClaimTypes.Name, user.Username)
      };

      var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

      var tokenDescriptor = new SecurityTokenDescriptor()
      {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.Now.AddHours(1),
        SigningCredentials = creds
      };

      var tokenHandler = new JwtSecurityTokenHandler();
      var token = tokenHandler.CreateToken(tokenDescriptor);

      return Ok(new
      {
        token = tokenHandler.WriteToken(token),
        user
      });
    }
  }
}
