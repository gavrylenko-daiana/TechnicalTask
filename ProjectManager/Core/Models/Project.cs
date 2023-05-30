using Core.Enums;

namespace Core.Models;

public class Project : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    // public List<ProjectTask> Tasks { get; set; } = null!; // общий список тасков
    // public List<ProjectTask> DoneTasks { get; set; } = null!; // список тасков, которые выполнены
    public int CountAllTasks { get; set; } = 0;
    public int CountDoneTasks { get; set; } = 0;
    public DateTime DueDates { get; set; }
    public User StakeHolder { get; set; }
    public User Tester { get; set; }
    public List<ProjectTask> Tasks { get; set; }
    public IDictionary<User, List<ProjectTask>> ClaimTaskDeveloper = new Dictionary<User, List<ProjectTask>>(); // для разработчика только таски 
    public Progress Progress { get; set; }
}
