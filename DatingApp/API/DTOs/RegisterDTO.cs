using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class RegisterDTO 
{
    [Required]
    [MinLength(3, ErrorMessage = "Username must be at least 3 characters long.")]
    public required string Username { get; set; }
    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    public required string Password { get; set; }
}