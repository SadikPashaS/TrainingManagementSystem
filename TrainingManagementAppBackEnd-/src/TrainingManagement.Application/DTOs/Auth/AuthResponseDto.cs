using System;
using TrainingManagement.Domain.Enums;

namespace TrainingManagement.Application.DTOs.Auth;

public class AuthResponseDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime TokenExpiry { get; set; }
}