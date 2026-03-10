using System.ComponentModel.DataAnnotations;

namespace TrainingManagement.Application.DTOs.Auth;

public class LoginDto
{
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;
}