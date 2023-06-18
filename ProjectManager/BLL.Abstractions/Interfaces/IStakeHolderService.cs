using Core.Enums;
using Core.Models;

namespace BLL.Abstractions.Interfaces;

public interface IStakeHolderService : IGenericService<User>
{
    Task UpdateProjectByTask(ProjectTask task);

    Task DeleteProjectAsync(string projectName);

    Task DeleteStakeHolder(User stakeHolder);

    Task<List<Project>> GetProjectsByStakeHolder(User stakeHolder);

    Task DeleteCurrentTask(ProjectTask task);

    Task<bool> ProjectIsAlreadyExistAsync(string projectName);

    Task<DateTime> UpdateDueDateInProjectAsync(string[] date);

    Task CreateProjectAsync(string projectName, string projectDescription, User stakeHolder,
        DateTime enteredDate, User tester);

    Task<User> GetTesterByNameAsync(string name);

    Task<ProjectTask> CreateTask(string taskName, string taskDescription, DateTime term, Priority priority,
        User tester, User stakeHolder);

    Task AddTaskToProjectAsync(Project project, List<ProjectTask> tasks);

    Task<Project> GetProjectByNameAsync(string projectName);

    Task<Priority> GetPriorityAsync(int choice, Priority priority);

    Task<DateTime> CreateDueDateForTaskAsync(Project project, string[] date);

    Task<bool> ProjectTaskIsAlreadyExistAsync(string taskName);
}