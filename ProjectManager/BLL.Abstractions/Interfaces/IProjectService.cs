using Core.Models;

namespace BLL.Abstractions.Interfaces;

public interface IProjectService : IGenericService<Project>
{
    // Task<List<User>?> GetDevelopersByProject(Project project);

    Task<Project> GetProjectByName(string projectName);

    Task<List<Project>> GetProjectsByStakeHolder(User stakeHolder);

    Task<Project> GetProjectByTask(ProjectTask task);
}
