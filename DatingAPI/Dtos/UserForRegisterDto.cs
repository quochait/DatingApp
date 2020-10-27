using System;
using System.ComponentModel.DataAnnotations;

namespace DatingAPI.Dtos
{
  public class UserForRegisterDto
  {
    [Required]
    public string Username { get; set; }
    [Required]
    [StringLength(12, MinimumLength = 8, ErrorMessage = "You must specify password between 8 and 12 characters.")]
    public string Password { get; set; }
    [Required]
    public string Gender { get; set; }
    [Required]
    public string KnownAs { get; set; }
    [Required]
    public DateTime DateOfBirth { get; set; }
    [Required]
    public string City { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Country { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastActive { get; set; }

    public UserForRegisterDto()
    {
      CreatedAt = DateTime.Now;
      LastActive = DateTime.Now;
    }
  }
}
