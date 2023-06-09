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

    private async Task DisplayAllUsersAsync(User u)
    {
        try
        {
            IEnumerable<User> users = await GetAllAsync();

            foreach (var user in users)
            {
                Console.WriteLine($"\nName: {user.Username}" +
                                  $"\nEmail: {user.Email}" +
                                  $"\nRole: {user.Role}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task UpdateUserPassword(User getUser)
    {
        try
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
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
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

    public async Task<bool> UserUniquenessCheck(string name)
    {
        try
        {
            if (await Service.UsernameIsAlreadyExist(name))
            {
                Console.WriteLine("This username or email is already taken!");

                return false;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }

        return true;
    }

    private async Task DeleteUserAsync(User user)
    {
        try
        {
            Console.WriteLine("Are you sure? 1 - Yes, 2 - No");
            int choice = int.Parse(Console.ReadLine()!);

            if (choice == 1)
            {
                await DeleteAsync(user.Id);
                Console.WriteLine("The user was successfully deleted");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task<ProjectTask> AddFileToTaskAsync()
    {
        try
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

                    return null!;
                }

                Console.WriteLine("Select the task to which you want to attach your file.\nName of task:");
                var nameOfTask = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(nameOfTask))
                {
                    try
                    {
                        var projectTask = await Service.GetTaskByNameAsync(nameOfTask);
                        await Service.AddFileFromUserAsync(path, projectTask);
                        
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
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
    
    public async Task UpdateUserAsync(User user)
    {
        try
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
                        user.Username = Console.ReadLine()!;
                        Console.WriteLine("Username was successfully edited");
                        break;
                    case "2":
                        await UpdateUserPassword(user);
                        break;
                    case "3":
                        Console.Write("Please, edit your email.\nYour email: ");
                        user.Email = Console.ReadLine()!;
                        Console.WriteLine("Your email was successfully edited");
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Invalid operation number.");
                        break;
                }

                await UpdateAsync(user.Id, user);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
}