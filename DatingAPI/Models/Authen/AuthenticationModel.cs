using DatingAPI.Models.Base;
using System;

namespace DatingAPI.Models.Authen
{
  public class AuthenticationModel : BaseEntity
  {
    public string Email { get; set; }
    public string UserId { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
  }
}
