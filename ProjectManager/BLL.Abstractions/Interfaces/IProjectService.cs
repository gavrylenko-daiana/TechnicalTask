using Core.Models;

namespace BLL.Abstractions.Interfaces;

public interface IProjectService : IGenericService<Project>
{
    Task<bool> ProjectIsAlreadyExist(string userInput);

    Task<Project> GetProjectByName(string projectName);

    Task<List<Project>> GetProjectByTester(User tester);

    Task<List<Project>> GetProjectsByStakeHolder(User stakeHolder);

    Task<Project> GetProjectByTask(ProjectTask task);

    Task UpdateProject(Project project);

    Task<List<ProjectTask>> GetCompletedTask(Project project);

    Task UpdateToCompletedProject(Project project);

    Task UpdateToWaitingTask(Project project);

    Task SendMailToUser(string email, string messageEmail);

    Task<DateTime> UpdateDueDateInProject(string[] date);

    Task UpdateDueDateInTask(ProjectTask task, string[] date);

    Task DeleteProject(string projectName);

    Task DeleteTasksWithProjectAsync(Project project);

    Task DeleteTaskFromProject(Project project, ProjectTask task);

    Task DeleteProjectsWithSteakHolderAsync(User stakeHolder);

    Task DeleteCurrentTaskAsync(ProjectTask task);

    Task CreateProject(string projectName, string projectDescription, User stakeHolder,
        DateTime enteredDate, User tester);

    Task AddTaskToProject(Project project, List<ProjectTask> tasks);
}
