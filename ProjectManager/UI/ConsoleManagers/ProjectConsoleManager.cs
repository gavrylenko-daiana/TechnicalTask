using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using UI.Interfaces;

namespace UI.ConsoleManagers;

public class ProjectConsoleManager : ConsoleManager<IProjectService, Project>, IConsoleManager<Project>
{
    private readonly ProjectTaskConsoleManager _projectTaskManager;

    public ProjectConsoleManager(IProjectService service, ProjectTaskConsoleManager projectTaskManager) : base(service)
    {
        _projectTaskManager = projectTaskManager;
    }

    public async Task DisplayProjectAsync(User user)
    {
        IEnumerable<Project> projects = await Service.GetProjectsByStakeHolder(user);
        if (!projects.Any())
        {
            Console.WriteLine("Project list is empty");
            return;
        }

        foreach (var project in projects)
        {
            Console.WriteLine($"\nName: {project.Name}");

            if (!string.IsNullOrWhiteSpace(project.Description))
                Console.WriteLine($"Description: {project.Description}");

            Console.WriteLine($"Stake Holder: {project.StakeHolder.Username} with email: {project.StakeHolder.Email}");
            Console.WriteLine($"Tester: {project.Tester.Username} with email: {project.Tester.Email}");
            Console.WriteLine($"Number of all tasks: {project.CountAllTasks}");
            Console.WriteLine($"Number of done tasks: {project.CountDoneTasks}");
            Console.WriteLine($"DueDates: {project.DueDates}");
            Console.WriteLine($"Status: {project.Progress}");

            if (project.Tasks != null && project.Tasks.Count > 0)
            {
                Console.WriteLine($"\nTask(s):");
                foreach (var task in project.Tasks)
                {
                    await _projectTaskManager.DisplayTaskAsync(task);
                }
            }
        }
    }

    public async Task DisplayAllProjectsAsync()
    {
        IEnumerable<Project> projects = await GetAllAsync();

        foreach (var project in projects)
        {
            await DisplayProjectAsync(project.StakeHolder);
        }
    }

    public async Task ChooseProjectToAddTasks(User stakeHolder)
    {
        await DisplayProjectAsync(stakeHolder);

        Console.Write($"\nEnter name of project you want to add tasks.\nName of project: ");
        var projectName = Console.ReadLine();

        try
        {
            var project = await Service.GetProjectByName(projectName!);
            await CreateTaskForProject(project);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"This name does not exist.");
        }
    }

    private async Task CreateTaskForProject(Project project)
    {
        project.Tasks = await _projectTaskManager.CreateTaskAsync(project);
        project.CountAllTasks = project.Tasks.Count;

        await UpdateAsync(project.Id, project);
    }

    public async Task CheckApproveTasksCountAsync(User stakeHolder)
    {
        var projects = await Service.GetProjectsByStakeHolder(stakeHolder);

        if (!projects.Any())
        {
            Console.WriteLine($"Stake Holder has no projects");
            return;
        }

        foreach (var project in projects)
        {
            int approveTasks = await _projectTaskManager.GetApproveTasksAsync(project);
            project.CountDoneTasks += approveTasks;
            await UpdateAsync(project.Id, project);
        }
    }

    public async Task UpdateProjectAsync(User stakeHolder)
    {
        await DisplayProjectAsync(stakeHolder);

        Console.Write($"\nEnter name of project you want to update.\nName: ");
        var projectName = Console.ReadLine()!;

        var project = await Service.GetProjectByName(projectName);

        if (project != null)
        {
            while (true)
            {
                Console.WriteLine("\nSelect which information you want to change: ");
                Console.WriteLine("1. Name");
                Console.WriteLine("2. Description");
                Console.WriteLine("3. Due date");
                Console.WriteLine("4. Exit");

                Console.Write("Enter the operation number: ");
                string input = Console.ReadLine()!;

                switch (input)
                {
                    case "1":
                        Console.Write("Please, edit name.\nName: ");
                        project.Name = Console.ReadLine()!;
                        Console.WriteLine("Name was successfully edited");
                        break;
                    case "2":
                        Console.Write("Please, edit description.\nDescription: ");
                        project.Description = Console.ReadLine()!;
                        Console.WriteLine("Description was successfully edited");
                        break;
                    case "3":
                        Console.Write("Please, edit a due date for the project.\nDue date (dd.MM.yyyy): ");
                        string[] date = Console.ReadLine()!.Split('.');
                        project.DueDates = new DateTime(int.Parse(date[2]), int.Parse(date[1]), int.Parse(date[0]));
                        Console.WriteLine("Due date was successfully edited");
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Invalid operation number.");
                        break;
                }

                await UpdateAsync(project.Id, project);
            }
        }
    }

    public async Task DeleteTesterFromProjectsAsync(User tester)
    {
        var projects = await GetTesterProjects(tester);
        
        if (projects.Any())
        {
            foreach (var project in projects)
            {
                project.Tester = null!;
                await UpdateAsync(project.Id, project);
            }
        }
    }

    public async Task<List<Project>> GetTesterProjects(User tester)
    {
        try
        {
            var projects = await Service.GetProjectByTester(tester);
            return projects;
        }
        catch
        {
            Console.WriteLine($"Task list is empty.");
        }

        return null!;
    }

    public async Task DeleteProjectAsync(string projectName)
    {
        var project = await Service.GetProjectByName(projectName);
        await DeleteAsync(project.Id);
    }

    public async Task DeleteProjectsWithSteakHolderAsync(User stakeHolder)
    {
        var projects = await Service.GetProjectsByStakeHolder(stakeHolder);

        foreach (var project in projects)
        {
            await _projectTaskManager.DeleteTasksWithProject(project);
            await DeleteAsync(project.Id);
        }
    }

    public async Task<Project> GetProjectByName(string projectName)
    {
        var project = await Service.GetProjectByName(projectName);

        return project;
    }

    public async Task<Project> GetProjectByTaskAsync(ProjectTask task)
    {
        var project = await Service.GetProjectByTask(task);

        return project;
    }

    public async Task<List<Project>> GetProjectsByStakeHolder(User stakeHolder)
    {
        var projects = await Service.GetProjectsByStakeHolder(stakeHolder);
        if (projects == null) Console.WriteLine($"Failed to get a stake holder for the project");

        return projects!;
    }

    public async Task DeleteCurrentTaskAsync(ProjectTask task)
    {
        var project = await Service.GetProjectByTask(task);
        
        if (project != null && project.Tasks.Any())
        {
            project.Tasks.RemoveAll(x => x.Id == task.Id);
            project.CountAllTasks -= 1;
            await UpdateAsync(project.Id, project);
            await _projectTaskManager.DeleteTaskAsync(task);
        }
        else
        {
            Console.WriteLine($"Failed to get project");
        }
    }

    public async Task UpdateTasksAsync(ProjectTask task)
    {
        var project = await Service.GetProjectByTask(task);

        if (project != null && project.Tasks.Any())
        {
            await _projectTaskManager.UpdateTaskAsync(task);
            await UpdateAsync(project.Id, project);
        }
        else
        {
            Console.WriteLine($"Failed to get project");
        }
    }

    public async Task DisplayOneTaskAsync(ProjectTask task)
    {
        await _projectTaskManager.DisplayTaskAsync(task);
    }

    public override Task PerformOperationsAsync(User user)
    {
        throw new NotImplementedException();
    }
}