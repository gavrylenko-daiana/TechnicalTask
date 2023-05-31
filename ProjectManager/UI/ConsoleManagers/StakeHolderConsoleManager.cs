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
            { "3", CreateTaskToProjectAsync },
            { "4", DisplayInfoStakeHolderAndProjectAsync },
            { "5", UpdateStakeHolderAsync},
            { "6", UpdateProjectAsync },
            { "7", DeleteOneProjectAsync },
            { "8", DeleteStakeHolderAsync },
        };
    
        while (true)
        {
            Console.WriteLine("\nUser operations:");
            Console.WriteLine("1. Update your information");
            Console.WriteLine("2. Create new project");
            Console.WriteLine("3. Create tasks for project");
            Console.WriteLine("4. Display info about you and your projects");
            Console.WriteLine("5. Update your information");
            Console.WriteLine("6. Update your project");
            Console.WriteLine("7. Delete your project");
            Console.WriteLine("8. Delete your account");
            Console.WriteLine("9. Exit");

            Console.Write("Enter the operation number: ");
            string input = Console.ReadLine()!;

            if (input == "9") break;
            if (input == "8")
            {
                await actions[input](user);
                break;
            }
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
        Console.Write($"\nYour username: {stakeHolder.Username}\n" +
                          $"Your email: {stakeHolder.Email}\n" +
                          $"Your project(s):\n");

        await _projectManager.DisplayProjectAsync(stakeHolder);
    }

    public async Task UpdateProjectAsync(User stakeHolder)
    {
        await _projectManager.UpdateProjectAsync(stakeHolder);
    }

    public async Task DeleteOneProjectAsync(User stakeHolder)
    {
        await _projectManager.DisplayProjectAsync(stakeHolder);
        
        Console.WriteLine($"\nEnter the name of project you want to delete:\nName: ");
        var projectName = Console.ReadLine()!;
        
        Console.WriteLine("Are you sure? 1 - Yes, 2 - No");
        int choice = int.Parse(Console.ReadLine()!);

        if (choice == 1)
        {
            await _projectManager.DeleteProjectAsync(projectName);
        }
    }

    public async Task DeleteStakeHolderAsync(User stakeHolder)
    {
        Console.WriteLine("Are you sure? 1 - Yes, 2 - No");
        int choice = int.Parse(Console.ReadLine()!);

        if (choice == 1)
        {
            await _projectManager.DeleteProjectsWithSteakHolderAsync(stakeHolder);
            await DeleteAsync(stakeHolder.Id);
        }
    }

    public async Task CreateProjectAsync(User stakeHolder)
    {
        await _projectManager.CreateNewProjectAsync(stakeHolder);
    }

    public async Task CreateTaskToProjectAsync(User stakeHolder)
    {
        await _projectManager.ChooseProjectToAddTasks(stakeHolder);
    }
}