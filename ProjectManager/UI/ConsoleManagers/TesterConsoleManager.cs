using BLL.Abstractions.Interfaces;
using BLL.Services;
using Core.Enums;
using Core.Models;
using DAL.Abstractions.Interfaces;
using UI.Interfaces;

namespace UI.ConsoleManagers;

public class TesterConsoleManager : ConsoleManager<ITesterService, User>, IConsoleManager<User>
{
    private readonly ProjectTaskConsoleManager _projectTaskManager; // для проверки через прогресс какие задачи Tester должен проверить
    private readonly UserConsoleManager _userManager;

    public TesterConsoleManager(ITesterService service, ProjectTaskConsoleManager projectTaskManager,
        UserConsoleManager userManager) : base(service)
    {
        _projectTaskManager = projectTaskManager;
        _userManager = userManager;
    }

    public override Task PerformOperationsAsync(User user)
    {
        throw new NotImplementedException();
    }

    public async Task DisplayAllTester()
    {
        IEnumerable<User> testers = await Service.GetAllTester();

        foreach (var tester in testers)
        {
            Console.WriteLine($"\nName: {tester.Username}");
            Console.WriteLine($"{tester.Email}");
        }
    }

    public async Task CheckTasksAsync(User tester)
    {
        try
        {
            var tasks = await _projectTaskManager.GetTesterTasksAsync(tester);

            foreach (var task in tasks)
            {
                try
                {
                    await _projectTaskManager.DisplayTaskAsync(task);
                }
                catch
                {
                    Console.WriteLine("No such task exists.");
                }

                while (true)
                {
                    Console.WriteLine("Do you want to approve this task?\nEnter 1 - Yes, 2 - No");
                    int choice = int.Parse(Console.ReadLine()!);

                    if (choice == 1)
                    {
                        task.Progress = Progress.CompletedTester;
                        await _projectTaskManager.UpdateAsync(task.Id, task);
                        
                        return;
                    }
                    else if (choice == 2)
                    {
                        Console.WriteLine("Select the reason for rejection:\n" +
                                          "1) Expired due date\n" +
                                          "2) Need to fix");
                        int option = int.Parse(Console.ReadLine()!);

                        if (option == 1)
                            await _userManager.SendMessageEmailUser(task.Developer.Email,  $"The task with the name {task.Name} and the deadline of {task.DueDates} has expired.\nThe message was sent from the tester - {task.Tester.Username}.");
                        else if (option == 2)
                            await _userManager.SendMessageEmailUser(task.Developer.Email, $"The task with the name {task.Name} needs to be fixed.\nThe message was sent from the tester - {task.Tester.Username}.");
                        else 
                            Console.WriteLine("Invalid operation number.");
                        
                        return;
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

    public async Task<User> GetTesterByName(string name)
    {
        var tester = await Service.GetTesterByName(name);

        return tester;
    }
}