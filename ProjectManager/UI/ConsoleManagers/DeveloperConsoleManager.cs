using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using UI.Interfaces;

namespace UI.ConsoleManagers;

public class DeveloperConsoleManager : ConsoleManager<IDeveloperService, User>, IConsoleManager<User>
{
    private readonly DevelopTasksConsoleManager _developTasksManager;
    private readonly UserConsoleManager _userConsoleManager;

    public DeveloperConsoleManager(IDeveloperService service, UserConsoleManager userConsoleManager,
        DevelopTasksConsoleManager developTasksManager) : base(service)
    {
        _userConsoleManager = userConsoleManager;
        _developTasksManager = developTasksManager;
    }

    public override async Task PerformOperationsAsync()
    {
        Dictionary<string, Func<Task>> actions = new Dictionary<string, Func<Task>>
        {
            { "1", DisplayDeveloperAsync },
            { "2", UpdateDeveloperAsync },
            { "3", AssignTasksToDevelopersAsync }
        };

        while (true)
        {
            Console.WriteLine("\nUser operations:");
            Console.WriteLine("1. Display information about you");
            Console.WriteLine("2. Update your information");
            Console.WriteLine("3. Select tasks");
            Console.WriteLine("4. Exit");

            Console.Write("Enter the operation number: ");
            string input = Console.ReadLine()!;

            if (input == "4") break;
            if (actions.ContainsKey(input)) await actions[input]();
            else Console.WriteLine("Invalid operation number.");
        }
    }

    public async Task DisplayDeveloperAsync()
    {
        await _developTasksManager.DisplayDeveloperAsync();
    }

    public async Task<Dictionary<User, List<ProjectTask>>> AssignTasksToDevelopersAsync()
    {
        var claimsTaskDeveloper = await _developTasksManager.AssignTasksToDevelopersAsync();

        return claimsTaskDeveloper;
    }
    
    public async Task UpdateDeveloperAsync()
    {
        Console.Write("Enter your username.\nYour username: ");
        string userName = Console.ReadLine()!;

        User getUser = await Service.GetDeveloperByUsernameOrEmail(userName);

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
                    getUser.Username = Console.ReadLine()!;
                    Console.WriteLine("Username was successfully edited");
                    break;
                case "2":
                    await _userConsoleManager.UpdateUserPassword(getUser);
                    break;
                case "3":
                    Console.Write("Please, edit your email.\nYour email: ");
                    getUser.Email = Console.ReadLine()!;
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