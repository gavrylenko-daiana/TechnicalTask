using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using UI.Interfaces;

namespace UI.ConsoleManagers;

public class DevelopTasksConsoleManager : ConsoleManager<IDeveloperService, User>, IConsoleManager<User>
{
    private readonly ProjectTaskConsoleManager _projectTaskManager;
    private readonly ProjectConsoleManager _projectManager;
    
    public DevelopTasksConsoleManager(IDeveloperService service, ProjectTaskConsoleManager projectTaskManager, ProjectConsoleManager projectManager) : base(service)
    {
        _projectTaskManager = projectTaskManager;
        _projectManager = projectManager;
    }
    
    public async Task<Dictionary<User, List<ProjectTask>>> AssignTasksToDevelopersAsync()
    {
        var claimsTaskEmployee = new Dictionary<User, List<ProjectTask>>();
        await _projectManager.DisplayAllProjectsAsync();

        Console.WriteLine("Write the name of the project from which you want to take tasks.");
        var projectName = Console.ReadLine()!;

        var project = await _projectManager.GetProjectByName(projectName);
        var developers = await Service.GetAllDeveloper();
        var tasks = await _projectTaskManager.GetTasksByProject(project);

        Console.WriteLine("list of all developers.");
        await DisplayDeveloperAsync();

        foreach (var developer in developers)
        {
            await _projectTaskManager.DisplayAllTaskByProject(tasks);

            if (tasks.Count != 0)
            {
                foreach (var task in tasks)
                {
                    Console.WriteLine($"Can {developer.Username} take task {task.Name}?\nPlease, write '1' - yes or '2' - no");
                    var choice = int.Parse(Console.ReadLine()!);

                    if (choice == 1)
                    {
                        if (!claimsTaskEmployee.TryGetValue(developer, out var developerTasks))
                        {
                            developerTasks = new List<ProjectTask>();
                            claimsTaskEmployee[developer] = developerTasks;
                        }
                        developerTasks.Add(task);
                    }
                }

                foreach (var projectTask in claimsTaskEmployee[developer])
                {
                    tasks.Remove(projectTask);
                }
            }
            else
            {
                break;
            }
        }

        return claimsTaskEmployee;
    }

    public override Task PerformOperationsAsync()
    {
        throw new NotImplementedException();
    }
    
    public async Task DisplayDeveloperAsync()
    {
        IEnumerable<User> developers = (await GetAllAsync()).Where(u => u.Role == UserRole.Developer);

        foreach (var developer in developers)
        {
            Console.WriteLine($"Username: {developer.Username}");
            Console.WriteLine($"Email: {developer.Email}");
        }
    }
}