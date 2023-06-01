using BLL.Abstractions.Interfaces;
using BLL.Services;
using Core.Enums;
using Core.Models;
using DAL.Abstractions.Interfaces;
using UI.Interfaces;

namespace UI.ConsoleManagers;

public class TesterConsoleManager : ConsoleManager<ITesterService, User>, IConsoleManager<User>
{
    private readonly UserConsoleManager _userManager;
    private readonly ProjectTaskConsoleManager _projectTaskManager;

    public TesterConsoleManager(ITesterService service, UserConsoleManager userManager, ProjectTaskConsoleManager projectTaskManager) : base(service)
    {
        _userManager = userManager;
        _projectTaskManager = projectTaskManager;
    }

    public override async Task PerformOperationsAsync(User user)
    {
        Dictionary<string, Func<User, Task>> actions = new Dictionary<string, Func<User, Task>>
        {
            { "1", DisplayTesterAsync },
            { "2", UpdateTesterAsync },
            { "3", CheckTasksAsync },
            { "4", DeleteTesterAsync }
        };

        while (true)
        {
            Console.WriteLine("\nUser operations:");
            Console.WriteLine("1. Display info about you");
            Console.WriteLine("2. Update your information");
            Console.WriteLine("3. Check tasks");
            Console.WriteLine("4. Delete your account");
            Console.WriteLine("5. Exit");

            Console.Write("Enter the operation number: ");
            string input = Console.ReadLine()!;

            if (input == "4")
            {
                await actions[input](user);
                break;
            }

            if (input == "5") break;
            if (actions.ContainsKey(input)) await actions[input](user);
            else Console.WriteLine("Invalid operation number.");
        }
    }

    public async Task DisplayAllTester()
    {
        IEnumerable<User> testers = await Service.GetAllTester();

        foreach (var tester in testers)
        {
            await DisplayTesterAsync(tester);
        }
    }

    public async Task DisplayTesterAsync(User tester)
    {
        Console.WriteLine($"\nName: {tester.Username}");
        Console.WriteLine($"Email: {tester.Email}");

        //выывод тасков с прогресс.веит
    }

    public async Task CheckTasksAsync(User tester)
    {
        try
        {
            var tasks = await _projectTaskManager.GetTesterTasksAsync(tester);
            if (!tasks.Any())
            {
                Console.WriteLine("No tasks to check.");
                return;
            }

            foreach (var task in tasks)
            {
                try
                {
                    await _projectTaskManager.DisplayTaskAsync(task);
                }
                catch
                {
                    Console.WriteLine("No such task exists.");
                    return;
                }

                while (true)
                {
                     Console.WriteLine("Do you want to approve this task?\nEnter 1 - Yes, 2 - No");
                    int choice = int.Parse(Console.ReadLine()!);

                    if (choice == 1)
                    {
                        task.Progress = Progress.CompletedTester;
                        await _projectTaskManager.UpdateAsync(task.Id, task);

                        break;
                    }
                    else if (choice == 2)
                    {
                        Console.WriteLine("Select the reason for rejection:\n" +
                                          "1. Expired due date\n" +
                                          "2. Need to fix");
                        int option = int.Parse(Console.ReadLine()!);

                        if (option == 1)
                            await _userManager.SendMessageEmailUser(task.Developer.Email,
                                $"The task with the name {task.Name} and the deadline of {task.DueDates} has expired.\nThe message was sent from the tester - {task.Tester.Username}.");
                        else if (option == 2)
                            await _userManager.SendMessageEmailUser(task.Developer.Email,
                                $"The task with the name {task.Name} needs to be fixed.\nThe message was sent from the tester - {task.Tester.Username}.");
                        else
                            Console.WriteLine("Invalid operation number.");

                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid operation number.");
                    }
                }
            }
        }
        catch
        {
            Console.WriteLine("Tasks list is empty.");
        }
    }

    public async Task UpdateTesterAsync(User tester)
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
                    tester.Username = Console.ReadLine()!;
                    Console.WriteLine("Username was successfully edited");
                    break;
                case "2":
                    await _userManager.UpdateUserPassword(tester);
                    break;
                case "3":
                    Console.Write("Please, edit your email.\nYour email: ");
                    tester.Email = Console.ReadLine()!;
                    Console.WriteLine("Your email was successfully edited");
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Invalid operation number.");
                    break;
            }

            await UpdateAsync(tester.Id, tester);
        }
    }

    public async Task DeleteTesterAsync(User tester)
    {
        Console.WriteLine("Are you sure? 1 - Yes, 2 - No");
        int choice = int.Parse(Console.ReadLine()!);

        if (choice == 1)
        {
            //проекты тоже
            var tasks = await _projectTaskManager.GetTesterTasks(tester);
            await _projectTaskManager.DeleteTesterFromTasksAsync(tasks);
            await DeleteAsync(tester.Id);
        }
    }

    public async Task<User> GetTesterByName(string name)
    {
        var tester = await Service.GetTesterByName(name);

        return tester;
    }
}