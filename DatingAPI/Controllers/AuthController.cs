using AutoMapper;
using DatingAPI.Data;
using DatingAPI.Dtos;
using DatingAPI.Models;
using DatingAPI.Models.Authen;
using DatingAPI.Models.Result;
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

      return Ok();
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
        Expires = DateTime.UtcNow.AddHours(3),
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

    [HttpPost("sendTokenResetPassword")]
    public async Task<IActionResult> GetTokenResetPassword([FromForm] string email)
    {
      AuthenticationModel result = await _authenServices.GetUser(email);
      if (result != null)
      {
        bool isVerify = await _userServices.IsVerify(email);
        if (isVerify)
        {
          _authenServices.SendTokenResetPassword(result.UserId, email);
          return Ok();
        }

        return BadRequest("Email is not verify.");
      }

      return BadRequest("Not found user.");
    }

    [HttpPost("verifyTokenResetPassword")]
    public async Task<IActionResult> CheckTokenResetPassword([FromForm] string email, [FromForm] string token, [FromForm] string password)
    {
      AuthenticationModel authenModel = await _authenServices.GetUser(email);
      if (authenModel == null)
      {
        return BadRequest("User email not exists");
      }

      string userId = authenModel.UserId;

      ResultModel result = _authenServices.CheckTokenReset(userId, token);
      if (result.True)
      {
        await _authenServices.UpdatePassword(userId, password);
        return Ok();
      }

      return BadRequest(result.Error);
    }
  }
}
