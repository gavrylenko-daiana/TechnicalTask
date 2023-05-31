using Core.Models;

namespace BLL.Abstractions.Interfaces;

public interface IProjectTaskService : IGenericService<ProjectTask>
{
    // Task<List<ProjectTask>> GetTasksByProject(Project project);

    // Task<ProjectTask> GetTaskAfterCreating();

    Task<List<ProjectTask>> GetTasksByDeveloper(User developer);
    Task<List<ProjectTask>> GetTasksByTester(User tester);
    Task<List<ProjectTask>> GetApproveTasks(Project project);
}