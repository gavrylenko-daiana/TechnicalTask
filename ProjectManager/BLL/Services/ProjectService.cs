using System.ComponentModel;
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

        try
        {
            var check = (await GetAll()).Any(p => p.Name == userInput);

            return check;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<Project> GetProjectByName(string projectName)
    {
        if (string.IsNullOrWhiteSpace(projectName)) throw new ArgumentNullException(nameof(projectName));
        
        try
        {
            Project project = await GetByPredicate(p => p.Name == projectName);

            if (project == null) throw new ArgumentNullException(nameof(project));

            return project;
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
            List<Project> projects = (await GetAll())
                .Where(p => p.StakeHolder != null && p.StakeHolder.Id == stakeHolder.Id).ToList();

            return projects;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<List<Project>> GetProjectByTester(User tester)
    {
        if (tester == null) throw new ArgumentNullException(nameof(tester));
        
        try
        {
            var projects = (await GetAll()).Where(p => p.Tester != null && p.Tester.Id == tester.Id).ToList();

            return projects;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<Project> GetProjectByTask(ProjectTask task)
    {
        if (task == null) throw new ArgumentNullException(nameof(task));
        
        try
        {
            var project = await GetByPredicate(p => p.Tasks.Any(t => t.Id == task.Id));

            return project;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task UpdateProject(Project project)
    {
        if (project == null) throw new ArgumentNullException(nameof(project));
        
        try
        {
            var tasks = (await _projectTaskService.GetAll()).Where(t => project.Tasks.Any(p => p.Id == t.Id)).ToList();
            project.Tasks = tasks;
            await Update(project.Id, project);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<List<ProjectTask>> GetCompletedTask(Project project)
    {
        if (project == null) throw new ArgumentNullException(nameof(project));
        
        try
        {
            var tasks = project.Tasks.Where(t => t.Progress == Progress.CompletedTask).ToList();

            return tasks;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task UpdateToCompletedProject(Project project)
    {
        if (project == null) throw new ArgumentNullException(nameof(project));
        
        try
        {
            for (int i = 0; i < project.Tasks.Count; i++)
            {
                project.Tasks[i].Progress = Progress.CompletedProject;
                await _projectTaskService.Update(project.Tasks[i].Id, project.Tasks[i]);
            }

            await Update(project.Id, project);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task UpdateToWaitingTask(Project project)
    {
        if (project == null) throw new ArgumentNullException(nameof(project));
        
        try
        {
            for (int i = 0; i < project.Tasks.Count; i++)
            {
                project.Tasks[i].Progress = Progress.WaitingTester;
                project.CountDoneTasks -= 1;
                await _projectTaskService.Update(project.Tasks[i].Id, project.Tasks[i]);
            }

            await Update(project.Id, project);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task SendMailToUser(string email, string messageEmail)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));
        if (string.IsNullOrWhiteSpace(messageEmail)) throw new ArgumentNullException(nameof(messageEmail));
        
        try
        {
            await _userService.SendMessageEmailUserAsync(email, messageEmail);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<DateTime> UpdateDueDateInProject(string[] date)
    {
        if (date == null) throw new ArgumentNullException(nameof(date));

        try
        {
            var dateTime = new DateTime(int.Parse(date[2]), int.Parse(date[1]), int.Parse(date[0]));

            return dateTime;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task UpdateDueDateInTask(ProjectTask task, string[] date)
    {
        if (task == null) throw new ArgumentNullException(nameof(task));
        if (date == null) throw new ArgumentNullException(nameof(date));
        
        try
        {
            await _projectTaskService.UpdateDueDateInTaskAsync(task, date);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task DeleteProject(string projectName)
    {
        if (string.IsNullOrWhiteSpace(projectName)) throw new ArgumentNullException(nameof(projectName));
       
        try
        {
            var project = await GetProjectByName(projectName);
            await DeleteTasksWithProjectAsync(project);
            await Delete(project.Id);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task DeleteTasksWithProjectAsync(Project project)
    {
        if (project == null) throw new ArgumentNullException(nameof(project));
        
        try
        {
            await _projectTaskService.DeleteTasksWithProject(project);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task DeleteTaskFromProject(Project project, ProjectTask task)
    {
        if (project == null) throw new ArgumentNullException(nameof(project));
        if (task == null) throw new ArgumentNullException(nameof(task));
        
        try
        {
            project.Tasks.RemoveAll(x => x.Id == task.Id);
            project.CountAllTasks -= 1;
            await Update(project.Id, project);
            await _projectTaskService.DeleteTask(task);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task DeleteProjectsWithSteakHolderAsync(User stakeHolder)
    {
        if (stakeHolder == null) throw new ArgumentNullException(nameof(stakeHolder));
        
        try
        {
            var projects = await GetProjectsByStakeHolder(stakeHolder);

            foreach (var project in projects)
            {
                await DeleteTasksWithProjectAsync(project);
                await Delete(project.Id);
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task DeleteCurrentTaskAsync(ProjectTask task)
    {
        if (task == null) throw new ArgumentNullException(nameof(task));
        
        try
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
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task CreateProject(string projectName, string projectDescription, User stakeHolder,
        DateTime enteredDate, User tester)
    {
        if (stakeHolder == null) throw new ArgumentNullException(nameof(stakeHolder));
        if (tester == null) throw new ArgumentNullException(nameof(tester));
        if (string.IsNullOrWhiteSpace(projectName)) throw new ArgumentNullException(nameof(projectName));
        if (string.IsNullOrWhiteSpace(projectDescription)) throw new ArgumentNullException(nameof(projectDescription));
        if (enteredDate == default(DateTime)) throw new ArgumentException("date cannot be empty");
        
        try
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
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task AddTaskToProject(Project project, List<ProjectTask> tasks)
    {
        if (project == null) throw new ArgumentNullException(nameof(project));
        if (tasks == null) throw new ArgumentNullException(nameof(tasks));
        
        try
        {
            project.Tasks.AddRange(tasks);
            project.CountAllTasks = project.Tasks.Count;
            await Update(project.Id, project);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task DeleteTesterFromProjectsAsync(User tester)
    {
        if (tester == null) throw new ArgumentNullException(nameof(tester));
        
        try
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
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task UpdateTask(ProjectTask task, List<ProjectTask> modifierTasks, Project project,
        ProjectTask newTask)
    {
        if (task == null) throw new ArgumentNullException(nameof(task));
        if (modifierTasks == null) throw new ArgumentNullException(nameof(modifierTasks));
        if (project == null) throw new ArgumentNullException(nameof(project));
        if (newTask == null) throw new ArgumentNullException(nameof(newTask));
        
        try
        {
            if (newTask != null)
            {
                modifierTasks.Add(newTask);
                project.Tasks.RemoveAll(x => x.Id == task.Id);
            }

            project.Tasks.AddRange(modifierTasks);
            await Update(project.Id, project);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}