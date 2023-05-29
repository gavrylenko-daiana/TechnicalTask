using Core.Enums;

namespace Core.Models;

// общее
public class User : BaseEntity
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public UserRole Role { get; set; }
    
    public List<Project> Projects { get; set; } = null!;
    // public List<ProjectTask> ProjectTasks { get; set; } = null!;
}
