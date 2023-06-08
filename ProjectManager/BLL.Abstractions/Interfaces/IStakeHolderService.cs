using Core.Models;

namespace BLL.Abstractions.Interfaces;

public interface IStakeHolderService : IGenericService<User>
{
    Task<User> GetStakeHolderByUsernameOrEmail(string? username);

    Task GetProjectByTaskAsync(ProjectTask task);

    Task DeleteProjectAsync(string projectName);

    Task DeleteStakeHolder(User stakeHolder);

    Task<List<Project>> GetProjectsByStakeHolder(User stakeHolder);

    Task DeleteCurrentTask(ProjectTask task);

    Task<bool> ProjectIsAlreadyExistAsync(string projectName);

    Task<DateTime> UpdateDueDateInProjectAsync(string[] date);

    Task CreateProjectAsync(string projectName, string projectDescription, User stakeHolder,
        DateTime enteredDate, User tester);
}