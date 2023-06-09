using Core.Models;

namespace BLL.Abstractions.Interfaces;

public interface IDeveloperService : IGenericService<User>
{
    Task UpdateProjectByTask(ProjectTask task);

    Task UpdateProgressToWaitTester(ProjectTask task);

    Task<Project> GetProjectByNameAsync(string projectName);

    Task TakeTaskByDeveloper(ProjectTask task, User developer);

    Task SendMailToUserAsync(string email, string message);

    Task<List<ProjectTask>> GetDeveloperTasks(User developer);

    Task DeleteDeveloperFromTasks(User developer);
}