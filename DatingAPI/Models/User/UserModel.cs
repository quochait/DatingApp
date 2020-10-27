using DatingAPI.Models.Base;
using System;

namespace DatingAPI.Models
{
  public class UserModel : BaseEntity
  {
    public string Username { get; set; }
    public string Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime LastActive { get; set; }
    public string Introduction { get; set; }
    public string LookingFor { get; set; }
    public string Interests { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public int Height { get; set; }
    public string MainPhotoUrl { get; set; } = string.Empty;
    public bool IsVerifyEmail { get; set; } = false;
    public string Email { get; set; }
  }
}
