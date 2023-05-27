using BLL.Abstractions.Interfaces;
using BLL.Services;
using Core.Enums;
using Core.Models;
using UI.Interfaces;

namespace UI.ConsoleManagers;

public class UserConsoleManager : ConsoleManager<IUserService, User>, IConsoleManager<User>
{
    private readonly DeveloperConsoleManager _developerConsoleManager;
    private readonly TesterConsoleManager _testerConsoleManager;
    private readonly StakeHolderConsoleManager _stakeHolderConsoleManager;

    public UserConsoleManager(IUserService service,
        DeveloperConsoleManager developerConsoleManager, TesterConsoleManager testerConsoleManager,
        StakeHolderConsoleManager stakeHolderConsoleManager) : base(service)
    {
        _developerConsoleManager = developerConsoleManager;
        _testerConsoleManager = testerConsoleManager;
        _stakeHolderConsoleManager = stakeHolderConsoleManager;
    }

    public override async Task PerformOperationsAsync()
    {
        Dictionary<string, Func<Task>> actions = new Dictionary<string, Func<Task>>
        {
            { "1", DisplayAllUsersAsync },
            { "2", CreateUserAsync },
            { "3", DeleteUserAsync },
        };

        while (true)
        {
            Console.WriteLine("\nUser operations:");
            Console.WriteLine("1. Display all users");
            Console.WriteLine("2. Create a new user");
            Console.WriteLine("3. Delete a user");
            Console.WriteLine("4. Exit");

            Console.Write("Enter the operation number: ");
            string input = Console.ReadLine()!;

            if (input == "4") break;

            if (actions.ContainsKey(input))
                await actions[input]();
            else
                Console.WriteLine("Invalid operation number.");
        }
    }

    public async Task AuthenticateUser()
    {
        Console.WriteLine("Please, write your username or email.\nUsername or Email: ");
        string userInput = Console.ReadLine()!;

        User getUser = await GetByPredicateAsync(u => u.Username == userInput || u.Email == userInput);

        if (getUser == null)
            throw new ArgumentNullException(nameof(getUser));

        Console.Write("Please, write your password.\nPassword: ");
        string password = Console.ReadLine()!;

        if (getUser.PasswordHash == Service.GetPasswordHash(password))
        {
            if (getUser.Role == UserRole.User) await PerformOperationsAsync();
            if (getUser.Role == UserRole.Developer) await _developerConsoleManager.PerformOperationsAsync();
            // if (getUser.Role == UserRole.StakeHolder) await _stakeHolderConsoleManager.PerformOperationsStakeHolderAsync();
            // if (getUser.Role == UserRole.Tester) await _testerConsoleManager.PerformOperationsAsync();
        }
        else
        {
            throw new Exception("You entered the wrong password!");
        }
    }

    public async Task DisplayAllUsersAsync()
    {
        IEnumerable<User> users = await GetAllAsync();

        foreach (var user in users)
        {
            Console.WriteLine($"\nName: {user.Username}" +
                              $"Email: {user.Email}" +
                              $"\nRole: {user.Role}");
        }
    }

    public async Task CreateUserAsync()
    {
        Console.WriteLine("Create user");
        Console.Write("Please, write your username.\nUsername: ");
        string userName = Console.ReadLine()!;

        Console.WriteLine("Please, write your email.\nEmail: ");
        string userEmail = Console.ReadLine()!;

        Console.Write("Please, write your password.\nPassword: ");
        string password = Console.ReadLine()!;

        UserRole role = await SelectRoleUser();

        Console.WriteLine("User was successfully added");

        await CreateAsync(new User
        {
            Username = userName,
            Email = userEmail,
            PasswordHash = Service.GetPasswordHash(password),
            Role = role
        });
    }
  
    private async Task<UserRole> SelectRoleUser()
    {
        UserRole role = UserRole.Developer;
        Console.WriteLine("Enter role for user: \n1) User(Admin); 2) Developer; 3) Tester; 4) StakeHolder");
        int choice = int.Parse(Console.ReadLine()!);

        try
        {
            role = choice switch
            {
                1 => UserRole.User,
                2 => UserRole.Developer,
                3 => UserRole.Tester,
                4 => UserRole.StakeHolder,
            };
        }
        catch
        {
            Console.WriteLine("Such a type of subscription does not exist!");
        }

        return role;
    }
    
    public async Task DeleteUserAsync()
    {
        Console.Write("Enter your name.\nYour name: ");
        string userName = Console.ReadLine() ?? throw new ArgumentNullException("Console.ReadLine()");
    
        User getUserName = await GetByPredicateAsync(u => u.Username == userName);
        if (getUserName == null) throw new ArgumentNullException(nameof(getUserName));
        await DeleteAsync(getUserName.Id);
    
        Console.WriteLine("The user was successfully deleted");
    }
}