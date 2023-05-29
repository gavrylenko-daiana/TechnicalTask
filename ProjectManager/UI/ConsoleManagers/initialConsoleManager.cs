using System.Net;
using System.Net.Mail;
using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using UI.Interfaces;

namespace UI.ConsoleManagers;

public class InitialConsoleManager : ConsoleManager<IUserService, User>, IConsoleManager<User>
{
    private readonly UserConsoleManager _userConsoleManager;
    private readonly StakeHolderConsoleManager _stakeHolderManager;
    private readonly DeveloperConsoleManager _developerConsoleManager;
    private readonly TesterConsoleManager _testerConsoleManager;

    public InitialConsoleManager(IUserService userService, StakeHolderConsoleManager stakeHolderConsoleManager,
        UserConsoleManager userConsoleManager, DeveloperConsoleManager developerConsoleManager,
        TesterConsoleManager testerConsoleManager) : base(userService)
    {
        _stakeHolderManager = stakeHolderConsoleManager;
        _userConsoleManager = userConsoleManager;
        _developerConsoleManager = developerConsoleManager;
        _testerConsoleManager = testerConsoleManager;
    }

    public async Task AuthenticateUser()
    {
        Console.WriteLine("Please, write your username or email.\nUsername or Email: ");
        string userInput = Console.ReadLine()!;

        User getUser = await Service.GetUserByUsernameOrEmail(userInput);

        Console.Write("Please, write your password.\nPassword: ");
        string password = Console.ReadLine()!;

        if (getUser.PasswordHash == Service.GetPasswordHash(password))
        {
            if (getUser.Role == UserRole.User) await _userConsoleManager.PerformOperationsAsync();
            if (getUser.Role == UserRole.StakeHolder) await _stakeHolderManager.PerformOperationsAsync();
            if (getUser.Role == UserRole.Developer) await _developerConsoleManager.PerformOperationsAsync();
            if (getUser.Role == UserRole.Tester) await _testerConsoleManager.PerformOperationsAsync();
        }
        else
        {
            throw new Exception("You entered the wrong password!");
        }
    }

    public async Task CreateUserAsync()
    {
        Console.WriteLine("Create user");
        Console.Write("Please, write your username.\nUsername: ");
        string userName = Console.ReadLine()!;

        Console.Write("Please, write your email.\nEmail: ");
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

    public override async Task PerformOperationsAsync()
    {
        throw new NotImplementedException();
    }
}