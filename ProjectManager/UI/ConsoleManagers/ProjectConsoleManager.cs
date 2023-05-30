using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using UI.Interfaces;

namespace UI.ConsoleManagers;

public class ProjectConsoleManager : ConsoleManager<IProjectService, Project>, IConsoleManager<Project>
{
    private readonly TesterConsoleManager _testerManager;
    private readonly ProjectTaskConsoleManager _projectTaskManager;

    public ProjectConsoleManager(IProjectService service, ProjectTaskConsoleManager projectTaskManager, TesterConsoleManager testerManager) : base(service)
    {
        _projectTaskManager = projectTaskManager;
        _testerManager = testerManager;
    }

    public async Task DisplayProjectAsync(User user)
    {
        Project project = await Service.GetProjectByStakeHolder(user);
        
        Console.WriteLine($"Name: {project.Name}");

        if (!string.IsNullOrWhiteSpace(project.Description))
            Console.WriteLine($"Description: {project.Description}");

        Console.WriteLine($"Stake Holder: {project.StakeHolder}");
        Console.WriteLine($"Tester: {project.Tester}");
        Console.WriteLine($"Number of all tasks: {project.CountAllTasks}");
        Console.WriteLine($"Number of done tasks: {project.CountDoneTasks}");

        foreach (var kvp in project.ClaimTaskDeveloper)
        {
            Console.WriteLine($"Developer: {kvp.Key}");
            Console.WriteLine("Tasks:");
            foreach (var task in kvp.Value)
            {
                Console.WriteLine($"\t{task}");
            }
        }

        Console.WriteLine($"DueDates: {project.DueDates}");
        Console.WriteLine($"Status: {project.Progress}");
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

        string projectDescript;
        Console.WriteLine("Optionally add a description to the project.\nPress 'Enter' to add");
        ConsoleKeyInfo keyInfo = Console.ReadKey();

        if (keyInfo.Key == ConsoleKey.Enter)
            projectDescript = Console.ReadLine()!;
        else
            projectDescript = "empty";

        Console.Write("Enter a due date for the task.\nDue date (dd.MM.yyyy): ");
        string[] date = Console.ReadLine()!.Split('.');
        DateTime enteredDate = new DateTime(int.Parse(date[2]), int.Parse(date[1]), int.Parse(date[0]));
        
        await _testerManager.DisplayAllTester();
        Console.Write("\nWrite the username of the person who will be the tester for this project.\nTester: ");
        string testerName = Console.ReadLine()!;

        var tester = await _testerManager.GetTesterByName(testerName);
        var tasks = await _projectTaskManager.CreateTaskAsync();
        var countAllTasks = tasks.Count;

        await CreateAsync(new Project
        {
            Name = projectName,
            Description = projectDescript,
            Progress = Progress.Planned,
            StakeHolder = getUser,
            DueDates = enteredDate,
            Tester = tester,
            CountAllTasks = countAllTasks
        });
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