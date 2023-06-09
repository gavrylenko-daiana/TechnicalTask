using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using DAL.Abstractions.Interfaces;

namespace BLL.Services;

public class ProjectService : GenericService<Project>, IProjectService
{
    private readonly IProjectTaskService _projectTaskService;
    private readonly IUserService _userService;

    public ProjectService(IRepository<Project> repository, IProjectTaskService projectTaskService,
        IUserService userService) : base(repository)
    {
        _projectTaskService = projectTaskService;
        _userService = userService;
    }

    public async Task<bool> ProjectIsAlreadyExist(string userInput)
    {
        if (string.IsNullOrWhiteSpace(userInput)) throw new ArgumentNullException(nameof(userInput));

        var check = (await GetAll()).Any(p => p.Name == userInput);

        return check;
    }

    public async Task<Project> GetProjectByName(string projectName)
    {
        Project project = await GetByPredicate(p => p.Name == projectName);

        if (project == null) throw new ArgumentNullException(nameof(project));

        return project;
    }

    public async Task<List<Project>> GetProjectsByStakeHolder(User stakeHolder)
    {
        List<Project> projects = (await GetAll())
            .Where(p => p.StakeHolder != null && p.StakeHolder.Id == stakeHolder.Id).ToList();

        return projects;
    }

    public async Task<List<Project>> GetProjectByTester(User tester)
    {
        var projects = (await GetAll()).Where(p => p.Tester != null && p.Tester.Id == tester.Id).ToList();

        return projects;
    }

    public async Task<Project> GetProjectByTask(ProjectTask task)
    {
        var project = await GetByPredicate(p => p.Tasks.Any(t => t.Id == task.Id));

        return project;
    }

    public async Task UpdateProject(Project project)
    {
        var tasks = (await _projectTaskService.GetAll()).Where(t => project.Tasks.Any(p => p.Id == t.Id)).ToList();
        project.Tasks = tasks;
        await Update(project.Id, project);
    }

    public async Task<List<ProjectTask>> GetCompletedTask(Project project)
    {
        var tasks = project.Tasks.Where(t => t.Progress == Progress.CompletedTask).ToList();

        return tasks;
    }

    public async Task UpdateToCompletedProject(Project project)
    {
        for (int i = 0; i < project.Tasks.Count; i++)
        {
            project.Tasks[i].Progress = Progress.CompletedProject;
            await _projectTaskService.Update(project.Tasks[i].Id, project.Tasks[i]);
        }

        await Update(project.Id, project);
    }

    public async Task UpdateToWaitingTask(Project project)
    {
        for (int i = 0; i < project.Tasks.Count; i++)
        {
            project.Tasks[i].Progress = Progress.WaitingTester;
            project.CountDoneTasks -= 1;
            await _projectTaskService.Update(project.Tasks[i].Id, project.Tasks[i]);
        }

        await Update(project.Id, project);
    }

    public async Task SendMailToUser(string email, string messageEmail)
    {
        await _userService.SendMessageEmailUserAsync(email, messageEmail);
    }

    public async Task<DateTime> UpdateDueDateInProject(string[] date)
    {
        var dateTime = new DateTime(int.Parse(date[2]), int.Parse(date[1]), int.Parse(date[0]));

        return dateTime;
    }

    public async Task UpdateDueDateInTask(ProjectTask task, string[] date)
    {
        await _projectTaskService.UpdateDueDateInTaskAsync(task, date);
    }

    public async Task DeleteProject(string projectName)
    {
        var project = await GetProjectByName(projectName);
        await DeleteTasksWithProjectAsync(project);
        await Delete(project.Id);
    }

    public async Task DeleteTasksWithProjectAsync(Project project)
    {
        await _projectTaskService.DeleteTasksWithProject(project);
    }

    public async Task DeleteTaskFromProject(Project project, ProjectTask task)
    {
        project.Tasks.RemoveAll(x => x.Id == task.Id);
        project.CountAllTasks -= 1;
        await Update(project.Id, project);
        await _projectTaskService.DeleteTask(task);
    }

    public async Task DeleteProjectsWithSteakHolderAsync(User stakeHolder)
    {
        var projects = await GetProjectsByStakeHolder(stakeHolder);

        foreach (var project in projects)
        {
            await DeleteTasksWithProjectAsync(project);
            await Delete(project.Id);
        }
    }

    public async Task DeleteCurrentTaskAsync(ProjectTask task)
    {
        var project = await GetProjectByTask(task);

        if (project != null && project.Tasks.Any())
        {
            await DeleteTaskFromProject(project, task);
        }
        else
        {
            throw new Exception($"Failed to get project");
        }
    }

    public async Task CreateProject(string projectName, string projectDescription, User stakeHolder,
        DateTime enteredDate, User tester)
    {
        await Add(new Project
        {
            Name = projectName,
            Description = projectDescription,
            StakeHolder = stakeHolder,
            DueDates = enteredDate,
            Tester = tester
        });
    }

    public async Task AddTaskToProject(Project project, List<ProjectTask> tasks)
    {
        project.Tasks.AddRange(tasks);
        project.CountAllTasks = project.Tasks.Count;
        await Update(project.Id, project);
    }
    
    public async Task DeleteTesterFromProjectsAsync(User tester)
    {
        var projects = await GetProjectByTester(tester);

        if (projects.Any())
        {
            foreach (var project in projects)
            {
                project.Tester = null!;
                await Update(project.Id, project);
            }
        }
    }

    public async Task UpdateTask(ProjectTask task, List<ProjectTask> modifierTasks, Project project, ProjectTask newTask)
    {
        if (newTask != null)
        {
            modifierTasks.Add(newTask);
            project.Tasks.RemoveAll(x => x.Id == task.Id);
        }
                        
        project.Tasks.AddRange(modifierTasks);
        await Update(project.Id, project);
    }
}