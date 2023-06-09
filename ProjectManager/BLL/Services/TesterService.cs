using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using DAL.Abstractions.Interfaces;

namespace BLL.Services;

public class TesterService : GenericService<User>, ITesterService
{
    private readonly IProjectService _projectService;
    private readonly IProjectTaskService _projectTaskService;

    public TesterService(IRepository<User> repository, IProjectService projectService, IProjectTaskService projectTaskService) : base(repository)
    {
        _projectService = projectService;
        _projectTaskService = projectTaskService;
    }

    public async Task<IEnumerable<User>> GetAllTester()
    {
        var testers = (await GetAll()).Where(u => u.Role == UserRole.Tester);

        return testers;
    }
    
    public async Task<User> GetTesterByName(string testerName)
    {
        var tester = await GetByPredicate(u => u.Username == testerName && u.Role == UserRole.Tester);

        return tester;
    }
    
    private async Task<Project> GetProjectByTaskAsync(ProjectTask task)
    {
        var project = await _projectService.GetProjectByTask(task);

        return project;
    }

    public async Task UpdateProjectByTask(ProjectTask task)
    {
        var project = await _projectService.GetProjectByTask(task);
        await _projectService.UpdateProject(project);
    }
    
    public async Task<List<ProjectTask>> GetTesterTasksAsync(User tester)
    {
        var tasks = await _projectTaskService.GetWaitTasksByTester(tester);

        return tasks;
    }

    public async Task AddCompletedTask(ProjectTask task)
    {
        task.Progress = Progress.CompletedTask;
        await _projectTaskService.Update(task.Id, task);

        var project = await GetProjectByTaskAsync(task);
        project.CountDoneTasks += 1;
        project.Tasks.First(t => t.Id == task.Id).Progress = task.Progress; 
                        
        await _projectService.Update(project.Id, project);
    }

    public async Task ReturnTaskInProgress(ProjectTask task)
    {
        task.Progress = Progress.InProgress;
        var project = await GetProjectByTaskAsync(task);
        project.CountDoneTasks -= 1;
        project.Tasks.First(t => t.Id == task.Id).Progress = task.Progress; 
                        
        await _projectTaskService.Update(task.Id, task);
        await _projectService.Update(project.Id, project);
    }

    public async Task SendMailToUserAsync(string email, string message)
    {
        await _projectService.SendMailToUser(email, message);
    }
    
    public async Task DeleteTesterAsync(User tester)
    {
        await _projectService.DeleteTesterFromProjectsAsync(tester);
        await _projectTaskService.DeleteTesterFromTasksAsync(tester);
        await Delete(tester.Id);
    }
    
    public async Task<List<ProjectTask>> GetWaitTasksByTesterAsync(User tester)
    {
        var tasks = await _projectTaskService.GetWaitTasksByTester(tester);

        return tasks;
    }
}
