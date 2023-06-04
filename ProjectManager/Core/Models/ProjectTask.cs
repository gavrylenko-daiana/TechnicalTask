using Core.Enums;

namespace Core.Models;

public class ProjectTask : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DueDates { get; set; }
    public Priority Priority { get; set; }
    public Progress Progress { get; set; }
    public User Developer { get; set; }
    public User Tester { get; set; }
    
    public List<string> UploadedFiles { get; set; } = new List<string>();
}
