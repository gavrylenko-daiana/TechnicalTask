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
    
    public override async Task PerformOperationsAsync(User user)
    {
        Dictionary<string, Func<User, Task>> actions = new Dictionary<string, Func<User, Task>>
        {
            { "1", UpdateStakeHolderAsync },
            { "2", CreateProjectAsync },
            { "3", DisplayInfoStakeHolderAndProjectAsync },
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
            if (actions.ContainsKey(input)) await actions[input](user);
            else Console.WriteLine("Invalid operation number.");
        }
    }

    public async Task UpdateStakeHolderAsync(User stakeHolder)
    {
        await DisplayInfoStakeHolderAndProjectAsync(stakeHolder);

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
                    stakeHolder.Username = Console.ReadLine()!;
                    Console.WriteLine("Username was successfully edited");
                    break;
                case "2":
                    await _userConsoleManager.UpdateUserPassword(stakeHolder);
                    break;
                case "3":
                    Console.Write("Please, edit your email.\nYour email: ");
                    stakeHolder.Email = Console.ReadLine()!;
                    Console.WriteLine("Your email was successfully edited");
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Invalid operation number.");
                    break;
            }
            
            await UpdateAsync(stakeHolder.Id, stakeHolder);
        }
    }

    private async Task DisplayInfoStakeHolderAndProjectAsync(User stakeHolder)
    {
        Console.Write($"Your username: {stakeHolder.Username}\n" +
                          $"Your email: {stakeHolder.Email}\n" +
                          $"Your project(s):\n");

        await _projectManager.DisplayProjectAsync(stakeHolder);
    }

    public async Task CreateProjectAsync(User stakeHolder)
    {
        await _projectManager.CreateNewProjectAsync(stakeHolder);
    }
}