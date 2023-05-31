using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using UI.Interfaces;

namespace UI.ConsoleManagers;

public class ProjectConsoleManager : ConsoleManager<IProjectService, Project>, IConsoleManager<Project>
{
    private readonly TesterConsoleManager _testerManager;
    private readonly ProjectTaskConsoleManager _projectTaskManager;
    private readonly UserConsoleManager _userConsoleManager;

    public ProjectConsoleManager(IProjectService service, ProjectTaskConsoleManager projectTaskManager,
        TesterConsoleManager testerManager, UserConsoleManager userConsoleManager) : base(service)
    {
        _projectTaskManager = projectTaskManager;
        _testerManager = testerManager;
        _userConsoleManager = userConsoleManager;
    }

    public async Task DisplayProjectAsync(User user)
    {
        IEnumerable<Project> projects = await Service.GetProjectsByStakeHolder(user);

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

    public async Task CreateNewProjectAsync(User getUser)
    {
        Console.WriteLine("Create project");
        Console.Write("Please, write name of project.\nName: ");
        string projectName = Console.ReadLine()!;

        string projectDescription;
        Console.WriteLine("Optionally add a description to the project.\nPress 'Enter' to add");
        ConsoleKeyInfo keyInfo = Console.ReadKey();

        if (keyInfo.Key == ConsoleKey.Enter)
            projectDescription = Console.ReadLine()!;
        else
            projectDescription = "empty";

        Console.Write("Enter a due date for the project.\nDue date (dd.MM.yyyy): ");
        string[] date = Console.ReadLine()!.Split('.');
        DateTime enteredDate = new DateTime(int.Parse(date[2]), int.Parse(date[1]), int.Parse(date[0]));

        await _testerManager.DisplayAllTester();
        Console.Write("\nWrite the username of the person who will be the tester for this project.\nTester: ");
        string testerName = Console.ReadLine()!;

        var tester = await _testerManager.GetTesterByName(testerName);

        await CreateAsync(new Project
        {
            Name = projectName,
            Description = projectDescription,
            Progress = Progress.Planned,
            StakeHolder = getUser,
            DueDates = enteredDate,
            Tester = tester
        });
    }

    public async Task ChooseProjectToAddTasks(User stakeHolder)
    {
        await DisplayProjectAsync(stakeHolder);

        Console.Write($"\nEnter name of project you want to add tasks.\nName: ");
        var projectName = Console.ReadLine();

        try
        {
            var project = await Service.GetProjectByName(projectName);
            await CreateTaskForProject(project);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"This name does not exist. Exception: {ex}");
        }
    }

    private async Task CreateTaskForProject(Project project)
    {
        project.Tasks = await _projectTaskManager.CreateTaskAsync(project);
        project.CountAllTasks = project.Tasks.Count;

        await UpdateAsync(project.Id, project);
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

    public override Task PerformOperationsAsync(User user)
    {
        throw new NotImplementedException();
    }
}