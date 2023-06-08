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

    public async Task GetProjectByTaskAsync(ProjectTask task)
    {
        var project = await _projectService.GetProjectByTask(task);
        await _projectService.UpdateProject(project);
    }
    
    public async Task DeleteProjectAsync(string projectName)
    {
        await _projectService.DeleteProject(projectName);
    }

    public async Task DeleteStakeHolder(User stakeHolder)
    {
        await _projectService.DeleteProjectsWithSteakHolderAsync(stakeHolder);
        await Delete(stakeHolder.Id);
    }

    public async Task<List<Project>> GetProjectsByStakeHolder(User stakeHolder)
    {
        var projects = await _projectService.GetProjectsByStakeHolder(stakeHolder);

        return projects;
    }

    public async Task DeleteCurrentTask(ProjectTask task)
    {
        await _projectService.DeleteCurrentTaskAsync(task);
    }

    public async Task<bool> ProjectIsAlreadyExistAsync(string projectName)
    {
        var check= await _projectService.ProjectIsAlreadyExist(projectName);

        return check;
    }

    public async Task<DateTime> UpdateDueDateInProjectAsync(string[] date)
    {
        var dateTime = await _projectService.UpdateDueDateInProject(date);

        return dateTime;
    }

    public async Task CreateProjectAsync(string projectName, string projectDescription, User stakeHolder,
        DateTime enteredDate, User tester)
    {
        await _projectService.CreateProject(projectName, projectDescription, stakeHolder, enteredDate, tester);
    }
}
