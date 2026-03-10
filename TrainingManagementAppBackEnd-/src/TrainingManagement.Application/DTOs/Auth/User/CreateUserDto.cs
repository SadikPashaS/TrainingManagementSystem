using System.ComponentModel.DataAnnotations;
using TrainingManagement.Domain.Enums;

namespace TrainingManagement.Application.DTOs.User;

public class CreateUserDto
{
    [Required]
    [StringLength(50)]
    public string UserId { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    public UserRole Role { get; set; } = UserRole.NormalUser;
}