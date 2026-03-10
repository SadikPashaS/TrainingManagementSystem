using TrainingManagement.Application.DTOs.UserTraining; // Add this line

namespace TrainingManagement.Application.DTOs.Dashboard;

public class UserDashboardDto
{
    public string WelcomeMessage { get; set; } = string.Empty;
    public int PendingCount { get; set; }
    public int InProgressCount { get; set; }
    public int DelayedCount { get; set; }
    public int CompletedCount { get; set; }
    public List<UserTrainingDto> MyTrainings { get; set; } = new();
}