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
}