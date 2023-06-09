using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using DAL.Abstractions.Interfaces;

namespace BLL.Services;

public class DeveloperService : GenericService<User>, IDeveloperService
{
    private readonly IProjectService _projectService;
    private readonly IProjectTaskService _projectTaskService;
    public DeveloperService(IRepository<User> repository, IProjectService projectService, IProjectTaskService projectTaskService) : base(repository)
    {
        _projectService = projectService;
        _projectTaskService = projectTaskService;
    }

    public async Task<User> GetDeveloperByUsernameOrEmail(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) throw new ArgumentNullException(nameof(input));

        User stakeHolder = await GetByPredicate(u => u.Role == UserRole.Developer && (u.Username == input  || u.Email == input));
        
        if (stakeHolder == null) throw new ArgumentNullException(nameof(stakeHolder));

        return stakeHolder;
    }
    
    public async Task<IEnumerable<User>> GetAllDeveloper()
    {
        var developer = (await GetAll()).Where(u => u.Role == UserRole.Developer);

        return developer;
    }
    
    public async Task UpdateProjectByTask(ProjectTask task)
    {
        var project = await _projectService.GetProjectByTask(task);
        await _projectService.UpdateProject(project);
    }
    
    private async Task<Project> GetProjectByTaskAsync(ProjectTask task)
    {
        var project = await _projectService.GetProjectByTask(task);

        return project;
    }

    public async Task UpdateProgressToWaitTester(ProjectTask task)
    {
        task.Progress = Progress.WaitingTester;
        await _projectTaskService.Update(task.Id, task);
        var project = await GetProjectByTaskAsync(task);
        project.Tasks.First(t => t.Id == task.Id).Progress = Progress.WaitingTester;
        await _projectService.Update(project.Id, project);
    }

    public async Task<Project> GetProjectByNameAsync(string projectName)
    {
        var project = await _projectService.GetProjectByName(projectName);

        return project;
    }
    
    public async Task SendMailToUserAsync(string email, string message)
    {
        await _projectService.SendMailToUser(email, message);
    }

    public async Task TakeTaskByDeveloper(ProjectTask task, User developer)
    {
        task.Developer = developer;
        task.Progress = Progress.InProgress;
        await _projectTaskService.Update(task.Id, task);
    }
    
    public async Task<List<ProjectTask>> GetDeveloperTasks(User developer)
    {
        try
        {
            var tasks = await _projectTaskService.GetTasksByDeveloper(developer);
            
            return tasks;
        }
        catch
        {
            throw new Exception($"Task list is empty.");
        }
    }

    public async Task DeleteDeveloperFromTasks(User developer)
    {
        var tasks = await GetDeveloperTasks(developer);
        await _projectTaskService.DeleteDeveloperFromTasksAsync(tasks);
        await Delete(developer.Id);
    }
}