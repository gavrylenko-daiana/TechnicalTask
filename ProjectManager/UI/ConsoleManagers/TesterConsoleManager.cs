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
    private readonly ProjectConsoleManager _projectManager;

    public TesterConsoleManager(ITesterService service, UserConsoleManager userManager,
        ProjectTaskConsoleManager projectTaskManager, ProjectConsoleManager projectManager) : base(service)
    {
        _userManager = userManager;
        _projectTaskManager = projectTaskManager;
        _projectManager = projectManager;
    }

    public override async Task PerformOperationsAsync(User user)
    {
        Dictionary<string, Func<User, Task>> actions = new Dictionary<string, Func<User, Task>>
        {
            { "1", DisplayTesterAsync },
            { "2", UpdateTesterAsync },
            { "3", CheckTasksAsync },
            { "4", AddFileToTask },
            { "5", DeleteTesterAsync }
        };

        while (true)
        {
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("\nUser operations:");
            Console.WriteLine("1. Display info about you");
            Console.WriteLine("2. Update your information");
            Console.WriteLine("3. Check tasks");
            Console.WriteLine("4. Add file to task");
            Console.WriteLine("5. Delete your account");
            Console.WriteLine("6. Exit");

            Console.Write("Enter the operation number: ");
            string input = Console.ReadLine()!;

            if (input == "5")
            {
                await actions[input](user);
                break;
            }

            if (input == "6") break;
            if (actions.ContainsKey(input)) await actions[input](user);
            else Console.WriteLine("Invalid operation number.");
        }
    }

    public async Task DisplayNameOfAllTester()
    {
        try
        {
            IEnumerable<User> testers = await Service.GetAllTester();
        
            if (testers == null)
            {
                throw new Exception("No testers");
            }

            Console.WriteLine("\nList of testers:");
            foreach (var tester in testers)
            {
                Console.WriteLine($"- {tester.Username}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    private async Task DisplayTesterAsync(User tester)
    {
        try
        {
            Console.WriteLine($"\nName: {tester.Username}");
            Console.WriteLine($"Email: {tester.Email}");

            var tasks = await _projectTaskManager.GetWaitTasksByTesterAsync(tester);

            if (tasks != null)
            {
                Console.WriteLine("\nTasks awaiting your review:");
                await _projectTaskManager.DisplayAllTaskByProject(tasks);
            }
            else
            {
                Console.WriteLine("Tasks did not come to check.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    private async Task CheckTasksAsync(User tester)
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

                        var project = await _projectManager.GetProjectByTaskAsync(task);
                        project.CountDoneTasks += 1;
                        project.Tasks.First(t => t.Id == task.Id).Progress = task.Progress; 

                        await _projectManager.UpdateAsync(project.Id, project);

                        break;
                    }
                    else if (choice == 2)
                    {
                        task.Progress = Progress.InProgress;
                        var project = await _projectManager.GetProjectByTaskAsync(task);
                        project.CountDoneTasks -= 1;
                        project.Tasks.First(t => t.Id == task.Id).Progress = task.Progress; 
                        
                        await _projectTaskManager.UpdateAsync(task.Id, task);
                        await _projectManager.UpdateAsync(project.Id, project);
                        
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

    private async Task AddFileToTask(User stakeHolder)
    {
        try
        {
            var task = await _userManager.AddFileToTaskAsync();
            var project = await _projectManager.GetProjectByTaskAsync(task);
            await _projectManager.UpdateAsync(project.Id, project);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    private async Task UpdateTesterAsync(User tester)
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
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    private async Task DeleteTesterAsync(User tester)
    {
        try
        {
            Console.WriteLine("Are you sure? 1 - Yes, 2 - No");
            int choice = int.Parse(Console.ReadLine()!);

            if (choice == 1)
            {
                await _projectManager.DeleteTesterFromProjectsAsync(tester);
                await _projectTaskManager.DeleteTesterFromTasksAsync(tester);
                await DeleteAsync(tester.Id);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task<User> GetTesterByName(string name)
    {
        try
        {
            var tester = await Service.GetTesterByName(name);

            return tester;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
}