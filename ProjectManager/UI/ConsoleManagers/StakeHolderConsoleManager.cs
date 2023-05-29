using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using UI.Interfaces;

namespace UI.ConsoleManagers;

public class StakeHolderConsoleManager : ConsoleManager<IStakeHolderService, User>, IConsoleManager<User>
{
    private readonly UserConsoleManager _userConsoleManager;
    private readonly ProjectConsoleManager _projectManager;
    public StakeHolderConsoleManager(IStakeHolderService service, UserConsoleManager userConsoleManager, ProjectConsoleManager projectManager) : base(service)
    {
        _userConsoleManager = userConsoleManager;
        _projectManager = projectManager;
    }
    
    public override async Task PerformOperationsAsync()
    {
        Dictionary<string, Func<Task>> actions = new Dictionary<string, Func<Task>>
        {
            { "1", UpdateStakeHolderAsync },
            { "2", CreateProjectAsync },
            { "3", DisplayInfoAsync },
            // { "4", UpdateProjectAsync },
            // { "5", DeleteProjectAsync },
        };
    
        while (true)
        {
            Console.WriteLine("\nUser operations:");
            Console.WriteLine("1. Update your information");
            Console.WriteLine("2. Create new project");
            Console.WriteLine("3. Display your projects");
            Console.WriteLine("4. Update your project");
            Console.WriteLine("5. Delete your project");
            Console.WriteLine("6. Exit");

            Console.Write("Enter the operation number: ");
            string input = Console.ReadLine()!;

            if (input == "6") break;
            if (actions.ContainsKey(input)) await actions[input]();
            else Console.WriteLine("Invalid operation number.");
        }
    }

    public async Task UpdateStakeHolderAsync()
    {
        Console.Write("Enter your username or email.\nYour username or email: ");
        string userInput = Console.ReadLine()!;

        User getUser = await Service.GetStakeHolderByUsernameOrEmail(userInput);
        await DisplayInfoStakeHolderAndProjectAsync(getUser);

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
            
            await UpdateAsync(getUser.Id, getUser);
        }
    }

    private async Task DisplayInfoAsync()
    {
        Console.Write("Enter your username or email.\nYour username or email: ");
        string userInput = Console.ReadLine()!;
        
        User getUser = await Service.GetStakeHolderByUsernameOrEmail(userInput);
        await DisplayInfoStakeHolderAndProjectAsync(getUser);
    }

    private async Task DisplayInfoStakeHolderAndProjectAsync(User user)
    {
        Console.Write($"Your username: {user.Username}\n" +
                          $"Your email: {user.Email}\n" +
                          $"Your project(s):\n");

        await _projectManager.DisplayAllProjectsAsync();
    }

    public async Task CreateProjectAsync()
    {
        Console.Write("Enter your username or email.\nUsername or email: ");
        string userInput = Console.ReadLine()!;
        
        User getUser = await Service.GetStakeHolderByUsernameOrEmail(userInput);
        await _projectManager.CreateNewProjectAsync(getUser);
    }
}