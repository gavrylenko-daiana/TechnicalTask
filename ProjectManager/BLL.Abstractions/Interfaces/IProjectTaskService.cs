using Core.Models;

namespace BLL.Abstractions.Interfaces;

public interface IProjectTaskService : IGenericService<ProjectTask>
{
    ICollection<List<ProjectTask>> GetTasksByProject(Project project);
}