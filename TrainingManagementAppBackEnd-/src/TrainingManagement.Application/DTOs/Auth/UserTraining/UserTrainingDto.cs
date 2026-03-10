using System;
using TrainingManagement.Domain.Enums;

namespace TrainingManagement.Application.DTOs.UserTraining
{
    public class UserTrainingDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int TrainingId { get; set; }
        public string TrainingName { get; set; } = string.Empty; // ADD THIS LINE
        public TrainingStatus Status { get; set; }
        public DateTime? ExpectedCompletionDate { get; set; }
        public string? Comments { get; set; }
        public DateTime AssignedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
    }
}