using Core.Models;

namespace BLL.Abstractions.Interfaces;

public interface IStakeHolderService : IGenericService<User>
{
    Task<User> GetStakeHolderByUsernameOrEmail(string? username);

    Task<Project> GetProjectByTaskAsync(ProjectTask task);

    Task UpdateProjectByTasksAsync(Project project);
}