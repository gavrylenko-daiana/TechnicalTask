using System.Net;
using System.Net.Mail;
using BLL.Abstractions.Interfaces;
using BLL.Services;
using Core.Enums;
using Core.Models;
using UI.Interfaces;

namespace UI.ConsoleManagers;

public class UserConsoleManager : ConsoleManager<IUserService, User>, IConsoleManager<User>
{
    private readonly ProjectTaskConsoleManager _projectTaskManager;

    public UserConsoleManager(IUserService service, ProjectTaskConsoleManager projectTaskManager) : base(service)
    {
        _projectTaskManager = projectTaskManager;
    }

    public override async Task PerformOperationsAsync(User user)
    {
        Dictionary<string, Func<User, Task>> actions = new Dictionary<string, Func<User, Task>>
        {
            { "1", DisplayAllUsersAsync },
            { "2", DeleteUserAsync }
        };

        while (true)
        {
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("\nUser operations:");
            Console.WriteLine("1. Display all users");
            Console.WriteLine("2. Delete a user");
            Console.WriteLine("3. Exit");

            Console.Write("Enter the operation number: ");
            string input = Console.ReadLine()!;

            if (input == "2")
            {
                await actions[input](user);
                break;
            }

            if (input == "3") break;
            if (actions.ContainsKey(input))
                await actions[input](user);
            else
                Console.WriteLine("Invalid operation number.");
        }
    }

    public async Task DisplayAllUsersAsync(User u)
    {
        IEnumerable<User> users = await GetAllAsync();

        foreach (var user in users)
        {
            Console.WriteLine($"\nName: {user.Username}" +
                              $"\nEmail: {user.Email}" +
                              $"\nRole: {user.Role}");
        }
    }

    public async Task UpdateUserPassword(User getUser)
    {
        string check = String.Empty;

        while (check != "exit")
        {
            Console.WriteLine("Write \n" +
                              "'exit' if you don't want to change your password \n" +
                              "'forgot' if you forgot your password.\n" +
                              "Press 'Enter' for continue update your password.");
            check = Console.ReadLine()!;
            if (check == "exit") return;
            if (check == "forgot")
            {
                await ForgotUserPassword(getUser);
                break;
            }

            Console.Write("Enter you current password.\nYour password: ");
            string password = Console.ReadLine()!;
            password = Service.GetPasswordHash(password);

            if (getUser.PasswordHash == password)
            {
                Console.Write("Enter your new password.\nYour Password: ");
                string newUserPassword = Console.ReadLine()!;

                await Service.UpdatePassword(getUser.Id, newUserPassword);
                return;
            }
            else
            {
                Console.WriteLine($"You entered the wrong password");
            }
        }

        await UpdateAsync(getUser.Id, getUser);
    }

    public async Task ForgotUserPassword(User getUser)
    {
        try
        {
            int emailCode = await Service.SendCodeToUser(getUser.Email);
            Console.WriteLine("Write the four-digit number that came to your email:");
            int userCode = int.Parse(Console.ReadLine()!);

            if (userCode == emailCode)
            {
                Console.Write("Enter your new password.\nNew Password: ");
                string newUserPassword = Console.ReadLine()!;

                await Service.UpdatePassword(getUser.Id, newUserPassword);
                Console.WriteLine("Your password was successfully edit.");
            }
            else
            {
                Console.WriteLine("You entered the wrong code.");
            }
        }
        catch
        {
            Console.WriteLine($"You got an error!");
        }
    }

    public async Task DeleteUserAsync(User user)
    {
        Console.WriteLine("Are you sure? 1 - Yes, 2 - No");
        int choice = int.Parse(Console.ReadLine()!);

        if (choice == 1)
        {
            await DeleteAsync(user.Id);
            Console.WriteLine("The user was successfully deleted");
        }
    }

    public async Task SendMessageEmailUser(string email, string messageEmail)
    {
        await Service.SendMessageEmailUserAsync(email, messageEmail);
    }

    public async Task<User> AuthenticateUser(string userInput, string password)
    {
        User getUser = await Service.Authenticate(userInput, password);

        return getUser;
    }

    public async Task<User> GetUserByUsernameOrEmailAsync(string input)
    {
        var getUser = await Service.GetUserByUsernameOrEmail(input);

        return getUser;
    }

    public async Task<ProjectTask> AddFileToTaskAsync()
    {
        Console.WriteLine($"Write the path to your file.\nPath to your file:");
        var path = Console.ReadLine();
        
        if (!string.IsNullOrWhiteSpace(path))
        {
            try
            {
                await _projectTaskManager.DisplayAllTasks();
            }
            catch
            {
                Console.WriteLine("Tasks list is empty");
            }

            Console.WriteLine("Select the task to which you want to attach your file.\nName of task:");
            var nameOfTask = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(nameOfTask))
            {
                try
                {
                    var projectTask = await _projectTaskManager.GetTaskByNameAsync(nameOfTask);
                    await _projectTaskManager.AddFileFromUserAsync(path, projectTask);

                    return projectTask;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine("You didn't enter anything");
            }
        }
        else
        {
            Console.WriteLine("You didn't enter anything");
        }

        return null!;
    }
}