using TrainingManagement.Domain.Enums;
using System;
using System.Collections.Generic;

namespace TrainingManagement.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    
    // Navigation properties
    public ICollection<UserTraining> UserTrainings { get; set; } = new List<UserTraining>();
}