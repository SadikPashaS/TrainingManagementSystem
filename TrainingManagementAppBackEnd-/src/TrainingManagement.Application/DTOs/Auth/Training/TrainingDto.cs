using TrainingManagement.Domain.Enums;

namespace TrainingManagement.Application.DTOs.Training;

public class TrainingDto
{
    public int Id { get; set; }
    public string TrainingName { get; set; } = string.Empty;
    public string TrainingUrl { get; set; } = string.Empty;
    public TrainingMode Mode { get; set; }
    public string Platform { get; set; } = string.Empty;
    public DateTime ExpectedStartDate { get; set; }
    public DateTime ExpectedEndDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}