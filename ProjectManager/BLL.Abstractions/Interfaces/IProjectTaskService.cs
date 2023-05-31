using Core.Models;

namespace BLL.Abstractions.Interfaces;

public interface IProjectTaskService : IGenericService<ProjectTask>
{
    // Task<List<ProjectTask>> GetTasksByProject(Project project);

    // Task<ProjectTask> GetTaskAfterCreating();

    Task<IEnumerable<ProjectTask>> GetTasksByUser(User developer);
}