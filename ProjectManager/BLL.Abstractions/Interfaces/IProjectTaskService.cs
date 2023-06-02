using Core.Enums;
using Core.Models;

namespace BLL.Abstractions.Interfaces;

public interface IProjectTaskService : IGenericService<ProjectTask>
{
    // Task<List<ProjectTask>> GetTasksByProject(Project project);

    // Task<ProjectTask> GetTaskAfterCreating();

    Task<List<ProjectTask>> GetTasksByDeveloper(User developer);
    
    Task<List<ProjectTask>> GetWaitTasksByTester(User tester);
    
    Task<List<ProjectTask>> GetApproveTasks(Project project);

    Task<Priority> GetPriority(int choice, Priority priority);
}