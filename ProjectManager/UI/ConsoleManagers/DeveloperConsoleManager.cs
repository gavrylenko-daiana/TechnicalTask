using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using UI.Interfaces;

namespace UI.ConsoleManagers;

public class DeveloperConsoleManager : ConsoleManager<IDeveloperService, User>, IConsoleManager<User>
{
    private readonly UserConsoleManager _userConsoleManager;
    private readonly ProjectConsoleManager _projectManager;
    private readonly ProjectTaskConsoleManager _projectTaskManager;
    private readonly TesterConsoleManager _testerManager;

    public DeveloperConsoleManager(IDeveloperService service, UserConsoleManager userConsoleManager,
        ProjectConsoleManager projectManager, ProjectTaskConsoleManager projectTaskManager, TesterConsoleManager testerManager) : base(service)
    {
        _userConsoleManager = userConsoleManager;
        _projectManager = projectManager;
        _projectTaskManager = projectTaskManager;
        _testerManager = testerManager;
    }

    public override async Task PerformOperationsAsync(User user)
    {
        Dictionary<string, Func<User, Task>> actions = new Dictionary<string, Func<User, Task>>
        {
            { "1", DisplayDeveloperAsync },
            { "2", UpdateDeveloperAsync },
            { "3", AssignTasksToDeveloperAsync },
            { "3", SubmitByTesterAsync }
        };

        while (true)
        {
            Console.WriteLine("\nUser operations:");
            Console.WriteLine("1. Display information about you");
            Console.WriteLine("2. Update your information");
            Console.WriteLine("3. Select tasks");
            Console.WriteLine("4. Submit a task for review");
            Console.WriteLine("5. Exit");

            Console.Write("Enter the operation number: ");
            string input = Console.ReadLine()!;

            if (input == "5") break;
            if (actions.ContainsKey(input)) await actions[input](user);
            else Console.WriteLine("Invalid operation number.");
        }
    }

    public async Task AssignTasksToDeveloperAsync(User developer)
    {
        await _projectManager.DisplayAllProjectsAsync();

        Console.WriteLine("Write the name of the project from which you want to take tasks.");
        var projectName = Console.ReadLine()!;

        var project = await _projectManager.GetProjectByName(projectName);
        var tasks = project.Tasks;

        if (tasks.Count != 0)
        {
            await _projectTaskManager.DisplayAllTaskByProject(tasks);

            foreach (var task in tasks)
            {
                if (task.Developer == null && task.Progress == Progress.Planned)
                {
                    Console.WriteLine($"Can {developer.Username} take task {task.Name}?\nPlease, write '1' - yes or '2' - no");
                    var choice = int.Parse(Console.ReadLine()!);

                    if (choice == 1)
                    {
                        task.Developer = developer;
                        task.Progress = Progress.InProgress;
                        await _projectTaskManager.UpdateAsync(task.Id, task);
                        
                        // await _userConsoleManager.SendMessageEmailUser(developer.Email, "The task has been changed from Planned to InProgress");
                    }
                }
            }
        }
        else
        {
            Console.WriteLine($"Task list is empty!");
        }

        await _projectManager.UpdateAsync(project.Id, project);
    }

    public async Task SubmitByTesterAsync(User developer)
    {
        // await _userConsoleManager.SendMessageEmailUser(developer.Email, "The task has been changed from InProgress to WaitingTester");
    }

    public async Task DisplayDeveloperAsync(User developer)
    {
        Console.WriteLine($"\nUsername: {developer.Username}");
        Console.WriteLine($"Email: {developer.Email}");

        var tasks = await _projectTaskManager.DisplayDeveloperTasks(developer);

        if (tasks != null)
        {
            Console.WriteLine($"Your current task(s): ");
            foreach (var task in tasks)
            {
                Console.WriteLine(task);
            }
        }
    }

    public async Task UpdateDeveloperAsync(User developer)
    {
        while (true)
        {
            Console.WriteLine("\nSelect which information you want to change: ");
            Console.WriteLine("1. Username");
            Console.WriteLine("2. Password");
            Console.WriteLine("3. Email");
            Console.WriteLine("4. Exit");

            Console.Write("Enter the operation number: ");
            string input = Console.ReadLine()!;

            switch (input)
            {
                case "1":
                    Console.Write("Please, edit your username.\nYour name: ");
                    developer.Username = Console.ReadLine()!;
                    Console.WriteLine("Username was successfully edited");
                    break;
                case "2":
                    await _userConsoleManager.UpdateUserPassword(developer);
                    break;
                case "3":
                    Console.Write("Please, edit your email.\nYour email: ");
                    developer.Email = Console.ReadLine()!;
                    Console.WriteLine("Your email was successfully edited");
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Invalid operation number.");
                    break;
            }
        }
    }
}