using TrainingManagement.Domain.Enums;
using System;
using System.Collections.Generic;

namespace TrainingManagement.Domain.Entities;

public class Training
{
    public int Id { get; set; }
    public string TrainingName { get; set; } = string.Empty;
    public string TrainingUrl { get; set; } = string.Empty;
    public TrainingMode Mode { get; set; }
    public string Platform { get; set; } = string.Empty;
    public DateTime ExpectedStartDate { get; set; }
    public DateTime ExpectedEndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public ICollection<UserTraining> UserTrainings { get; set; } = new List<UserTraining>();
}