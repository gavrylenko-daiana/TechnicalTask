using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using DAL.Abstractions.Interfaces;

namespace BLL.Services;

public class StakeHolderService : GenericService<User>, IStakeHolderService
{
    private readonly IProjectService _projectService;
    
    public StakeHolderService(IRepository<User> repository, IProjectService projectService) : base(repository)
    {
        _projectService = projectService;
    }

    public async Task<User> GetStakeHolderByUsernameOrEmail(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) throw new ArgumentNullException(nameof(input));

        User stakeHolder = await GetByPredicate(u => u.Role == UserRole.StakeHolder && (u.Username == input  || u.Email == input));
        
        if (stakeHolder == null) throw new ArgumentNullException(nameof(stakeHolder));

        return stakeHolder;
    }

    public async Task<Project> GetProjectByTaskAsync(ProjectTask task)
    {
        var project = await _projectService.GetProjectByTask(task);

        return project;
    }

    public async Task UpdateProjectByTasksAsync(Project project)
    {
        await _projectService.UpdateProject(project);
    }
}
