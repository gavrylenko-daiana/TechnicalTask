using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using UI.Interfaces;

namespace UI.ConsoleManagers;

public class StakeHolderConsoleManager : ConsoleManager<IStakeHolderService, User>, IConsoleManager<User>
{
    private readonly ProjectConsoleManager _projectManager;
    public StakeHolderConsoleManager(IStakeHolderService service) : base(service)
    {
    }
    
    public async Task PerformOperationsStakeHolderAsync()
    {
        Dictionary<string, Func<Task>> actions = new Dictionary<string, Func<Task>>
        {
            { "1", DisplayInfoStakeHolderAsync },
            // { "2", UpdateInfoAdminAsync },
            { "3", DisplayAllProjectsAsync },
            // { "4", DeleteAdminAsync },
            // { "5", CreateProjectAsync },
            // { "6", UpdateProjectAsync },
            // { "7", DeleteProjectAsync },
        };
    
        while (true)
        {
            Console.WriteLine("\nUser operations:");
            Console.WriteLine("1. Display all information");
            Console.WriteLine("2. Update your information");
            Console.WriteLine("3. Display your projects");
            Console.WriteLine("4. Delete your account");
            Console.WriteLine("5. Create new project");
            Console.WriteLine("6. Update your project");
            Console.WriteLine("7. Delete your project");
            Console.WriteLine("8. Exit");
    
            Console.Write("Enter the operation number: ");
            string input = Console.ReadLine()!;
    
            if (input == "8") break;
            if (actions.ContainsKey(input)) await actions[input]();
            else Console.WriteLine("Invalid operation number.");
        }
    }

    public async Task DisplayInfoStakeHolderAsync()
    {
        IEnumerable<User> admins = (await GetAllAsync()).Where(u => u.Role == UserRole.StakeHolder);

        foreach (var admin in admins)
        {
            Console.WriteLine($"Username: {admin.Username}");
            Console.WriteLine($"Email: {admin.Email}");

            foreach (var project in admin.Projects)
            {
                Console.WriteLine($"Project: ");
            }
        }
    }
    
    public async Task UpdateInfoStakeHolder()
    {
        Console.Write("Enter your username.\nYour username: ");
        string userName = Console.ReadLine()!;

        User getUser = await Service.GetStakeHolderByUsername(userName);
        if (getUser == null) throw new ArgumentNullException(nameof(getUser));

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
                    await UpdateUserPassword(getUser);
                    break;
                case "3":

                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Invalid operation number.");
                    break;
            }
        }
    }

    public override Task PerformOperationsAsync()
    {
        throw new NotImplementedException();
    }

    public async Task CreateStakeHolderAsync()
    {
        
    }
    
    private async Task DisplayAllProjectsAsync()
    {
        await _projectManager.DisplayAllProjectsAsync();
    }
}