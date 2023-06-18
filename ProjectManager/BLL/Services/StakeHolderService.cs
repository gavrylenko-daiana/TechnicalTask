using System.ComponentModel;
using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using DAL.Abstractions.Interfaces;

namespace BLL.Services;

public class StakeHolderService : GenericService<User>, IStakeHolderService
{
    private readonly IProjectService _projectService;
    private readonly ITesterService _testerService;
    private readonly IProjectTaskService _projectTaskService;

    public StakeHolderService(IRepository<User> repository, IProjectService projectService,
        ITesterService testerService, IProjectTaskService projectTaskService) : base(repository)
    {
        _projectService = projectService;
        _testerService = testerService;
        _projectTaskService = projectTaskService;
    }

    public async Task<User> GetStakeHolderByUsernameOrEmail(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) throw new ArgumentNullException(nameof(input));

        try
        {
            User stakeHolder = await GetByPredicate(u =>
                u.Role == UserRole.StakeHolder && (u.Username == input || u.Email == input));

            if (stakeHolder == null) throw new ArgumentNullException(nameof(stakeHolder));

            return stakeHolder;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task UpdateProjectByTask(ProjectTask task)
    {
        if (task == null) throw new ArgumentNullException(nameof(task));

        try
        {
            var project = await _projectService.GetProjectByTask(task);
            await _projectService.UpdateProject(project);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task DeleteProjectAsync(string projectName)
    {
        if (string.IsNullOrWhiteSpace(projectName)) throw new ArgumentNullException(nameof(projectName));

        try
        {
            await _projectService.DeleteProject(projectName);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task DeleteStakeHolder(User stakeHolder)
    {
        if (stakeHolder == null) throw new ArgumentNullException(nameof(stakeHolder));

        try
        {
            await _projectService.DeleteProjectsWithSteakHolderAsync(stakeHolder);
            await Delete(stakeHolder.Id);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<List<Project>> GetProjectsByStakeHolder(User stakeHolder)
    {
        if (stakeHolder == null) throw new ArgumentNullException(nameof(stakeHolder));

        try
        {
            var projects = await _projectService.GetProjectsByStakeHolder(stakeHolder);

            return projects;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task DeleteCurrentTask(ProjectTask task)
    {
        if (task == null) throw new ArgumentNullException(nameof(task));

        try
        {
            await _projectService.DeleteCurrentTaskAsync(task);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> ProjectIsAlreadyExistAsync(string projectName)
    {
        if (string.IsNullOrWhiteSpace(projectName)) throw new ArgumentNullException(nameof(projectName));

        try
        {
            var check = await _projectService.ProjectIsAlreadyExist(projectName);

            return check;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<DateTime> UpdateDueDateInProjectAsync(string[] date)
    {
        if (date == null) throw new ArgumentNullException(nameof(date));

        try
        {
            var dateTime = await _projectService.UpdateDueDateInProject(date);

            return dateTime;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task CreateProjectAsync(string projectName, string projectDescription, User stakeHolder,
        DateTime enteredDate, User tester)
    {
        if (stakeHolder == null) throw new ArgumentNullException(nameof(stakeHolder));
        if (tester == null) throw new ArgumentNullException(nameof(tester));
        if (string.IsNullOrWhiteSpace(projectName)) throw new ArgumentNullException(nameof(projectName));
        if (string.IsNullOrWhiteSpace(projectDescription)) throw new ArgumentNullException(nameof(projectDescription));
        if (enteredDate == default(DateTime)) throw new ArgumentException("date cannot be empty");

        try
        {
            await _projectService.CreateProject(projectName, projectDescription, stakeHolder, enteredDate, tester);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<User> GetTesterByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

        try
        {
            var tester = await _testerService.GetTesterByName(name);

            return tester;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    
    public async Task<bool> ProjectTaskIsAlreadyExistAsync(string taskName)
    {
        if (string.IsNullOrWhiteSpace(taskName)) throw new ArgumentNullException(nameof(taskName));

        try
        {
            var check = await _projectTaskService.ProjectTaskIsAlreadyExist(taskName);

            return check;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<DateTime> CreateDueDateForTaskAsync(Project project, string[] date)
    {
        if (project == null) throw new ArgumentNullException(nameof(project));
        if (date == null) throw new ArgumentNullException(nameof(date));

        try
        {
            DateTime term = await _projectTaskService.CreateDueDateForTask(project, date);

            return term;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<Priority> GetPriorityAsync(int choice, Priority priority)
    {
        if (!Enum.IsDefined(typeof(Priority), priority))
            throw new InvalidEnumArgumentException(nameof(priority), (int)priority, typeof(Priority));
        if (choice <= 0 || choice >= 6) throw new ArgumentOutOfRangeException(nameof(choice));

        try
        {
            priority = await _projectTaskService.GetPriority(choice, priority);

            return priority;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<Project> GetProjectByNameAsync(string projectName)
    {
        if (string.IsNullOrWhiteSpace(projectName)) throw new ArgumentNullException(nameof(projectName));
        
        try
        {
            Project project = await _projectService.GetProjectByName(projectName);

            return project;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task AddTaskToProjectAsync(Project project, List<ProjectTask> tasks)
    {
        if (project == null) throw new ArgumentNullException(nameof(project));
        if (tasks == null) throw new ArgumentNullException(nameof(tasks));
        
        try
        {
            await _projectService.AddTaskToProject(project, tasks);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<ProjectTask> CreateTask(string taskName, string taskDescription, DateTime term, Priority priority,
        User tester, User stakeHolder)
    {
        if (tester == null) throw new ArgumentNullException(nameof(tester));
        if (term == default(DateTime)) throw new ArgumentException("date cannot be empty");
        if (!Enum.IsDefined(typeof(Priority), priority))
            throw new InvalidEnumArgumentException(nameof(priority), (int)priority, typeof(Priority));
        if (string.IsNullOrWhiteSpace(taskName)) throw new ArgumentNullException(nameof(taskName));
        if (string.IsNullOrWhiteSpace(taskDescription)) throw new ArgumentNullException(nameof(taskDescription));

        try
        {
            var task = await _projectTaskService.CreateTaskAsync(taskName, taskDescription, term, priority, tester, stakeHolder);

            return task;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}