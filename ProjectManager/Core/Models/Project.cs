using Core.Enums;

namespace Core.Models;

public class Project : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; } = null!;
    public int CountAllTasks { get; set; } = 0;
    public int CountDoneTasks { get; set; } = 0;
    public DateTime DueDates { get; set; }
    public User StakeHolder { get; set; }
    public User Tester { get; set; }
    public List<User> Developers { get; set; } = new List<User>();
    public List<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
}
