using System.ComponentModel.DataAnnotations;
using TrainingManagement.Domain.Enums;

namespace TrainingManagement.Application.DTOs.Training;

public class CreateTrainingDto
{
    [Required]
    [StringLength(100)]
    public string TrainingName { get; set; } = string.Empty;
    
    [Required]
    [Url]
    public string TrainingUrl { get; set; } = string.Empty;
    
    [Required]
    public TrainingMode Mode { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Platform { get; set; } = string.Empty;
    
    [Required]
    public DateTime ExpectedStartDate { get; set; }
    
    [Required]
    public DateTime ExpectedEndDate { get; set; }
}