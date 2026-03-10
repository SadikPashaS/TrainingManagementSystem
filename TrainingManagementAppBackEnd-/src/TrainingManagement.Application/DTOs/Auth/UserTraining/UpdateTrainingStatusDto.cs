using System.ComponentModel.DataAnnotations;
using TrainingManagement.Domain.Enums;

namespace TrainingManagement.Application.DTOs.UserTraining;

public class UpdateTrainingStatusDto
{
    [Required]
    public int UserTrainingId { get; set; }
    
    [Required]
    public TrainingStatus Status { get; set; }
    
    public DateTime? ExpectedCompletionDate { get; set; }
    
    public string? Comments { get; set; }
}