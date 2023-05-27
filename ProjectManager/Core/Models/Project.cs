using Core.Enums;

namespace Core.Models;

public class Project : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<ProjectTask> Tasks { get; set; } = null!; // общий список тасков
    public List<ProjectTask> DoneTasks { get; set; } = null!; // список тасков, которые выполнены
    public DateTime DueDates { get; set; }
    public User StakeHolder { get; set; }
    public Progress Progress { get; set; }
}
