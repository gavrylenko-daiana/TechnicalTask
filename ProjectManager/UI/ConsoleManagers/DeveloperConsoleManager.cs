using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using UI.Interfaces;

namespace UI.ConsoleManagers;

public class DeveloperConsoleManager : ConsoleManager<IDeveloperService, User>, IConsoleManager<User>
{
    public DeveloperConsoleManager(IDeveloperService service) : base(service)
    {
    }

    public override async Task PerformOperationsAsync()
    {
        Dictionary<string, Func<Task>> actions = new Dictionary<string, Func<Task>>
        {
            { "1", DisplayInfoDeveloper },
            { "2", UpdateInfoDeveloper }
            // { "4", CreateProjectAsync },
            // { "5", UpdateProjectAsync },
            // { "6", DeleteProjectAsync },
        };

        while (true)
        {
            Console.WriteLine("\nUser operations:");
            Console.WriteLine("1. Display information about you");
            Console.WriteLine("2. Update your information");
            Console.WriteLine("3. Display your projects");
            Console.WriteLine("4. Create new project");
            Console.WriteLine("5. Update your project");
            Console.WriteLine("6. Delete your project");
            Console.WriteLine("7. Exit");

            Console.Write("Enter the operation number: ");
            string input = Console.ReadLine()!;

            if (input == "5") break;
            if (actions.ContainsKey(input)) await actions[input]();
            else Console.WriteLine("Invalid operation number.");
        }
    }

    public async Task CreateDeveloperAsync()
    {
        
    }

    public async Task DisplayInfoDeveloper()
    {
        IEnumerable<User> developers = (await GetAllAsync()).Where(u => u.Role == UserRole.Developer);

        foreach (var developer in developers)
        {
            Console.WriteLine($"Username: {developer.Username}");
            Console.WriteLine($"Email: {developer.Email}");
        }
    }

    public async Task UpdateInfoDeveloper()
    {
        Console.Write("Enter your username.\nYour username: ");
        string userName = Console.ReadLine()!;

        User getUser = await Service.GetDeveloperByUsername(userName);
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
}