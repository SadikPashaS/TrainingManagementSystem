using TrainingManagement.Domain.Enums;

namespace TrainingManagement.Domain.Entities
{
    public class UserTraining
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TrainingId { get; set; }
        public TrainingStatus Status { get; set; }
        public DateTime? ExpectedCompletionDate { get; set; }
        public string? Comments { get; set; }
        public DateTime AssignedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        
        // Navigation properties
        public User User { get; set; }
        public Training Training { get; set; }
    }
}