using System.ComponentModel.DataAnnotations;

namespace TrainingManagement.Application.DTOs.UserTraining;

public class AssignTrainingDto
{
    [Required]
    public int UserId { get; set; }
    
    [Required]
    public int TrainingId { get; set; }
}