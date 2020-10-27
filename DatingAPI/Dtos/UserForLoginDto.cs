using System.ComponentModel.DataAnnotations;

namespace DatingAPI.Dtos
{
  public class UserForLoginDto
  {
    [Required]
    [EmailAddress(ErrorMessage = "Input is not Email address.")]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
  }
}
