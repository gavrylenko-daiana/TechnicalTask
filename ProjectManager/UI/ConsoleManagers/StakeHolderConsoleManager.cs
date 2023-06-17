using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using UI.Interfaces;

namespace UI.ConsoleManagers;

public class StakeHolderConsoleManager : ConsoleManager<IStakeHolderService, User>, IConsoleManager<User>
{
    private readonly UserConsoleManager _userConsoleManager;
    private readonly ProjectConsoleManager _projectManager;
    private readonly TesterConsoleManager _testerManager;

    public StakeHolderConsoleManager(IStakeHolderService service, UserConsoleManager userConsoleManager,
        ProjectConsoleManager projectManager, TesterConsoleManager testerManager) : base(service)
    {
        _userConsoleManager = userConsoleManager;
        _projectManager = projectManager;
        _testerManager = testerManager;
    }

    public override async Task PerformOperationsAsync(User user)
    {
        Dictionary<string, Func<User, Task>> actions = new Dictionary<string, Func<User, Task>>
        {
            { "1", DisplayInfoStakeHolderAndProjectAsync },
            { "2", CreateProjectAsync },
            { "3", CreateTaskToProjectAsync },
            { "4", AddFileToTask },
            { "5", CheckApprovedTasksAsync },
            { "6", UpdateStakeHolderAsync },
            { "7", UpdateProjectAsync },
            { "8", DeleteTasksAsync },
            { "9", DeleteOneProjectAsync },
            { "10", DeleteStakeHolderAsync },
        };

        while (true)
        {
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("User operations:");
            Console.WriteLine("1. Display info about you and your projects");
            Console.WriteLine("2. Create new project");
            Console.WriteLine("3. Create tasks for project");
            Console.WriteLine("4. Add file to task");
            Console.WriteLine("5. Check approved tasks");
            Console.WriteLine("6. Update your information");
            Console.WriteLine("7. Update your project");
            Console.WriteLine("8. Delete task from project");
            Console.WriteLine("9. Delete your project");
            Console.WriteLine("10. Delete your account");
            Console.WriteLine("11. Exit");

            Console.Write("Enter the operation number: ");
            string input = Console.ReadLine()!;

            if (input == "10")
            {
                await actions[input](user);
                break;
            }

            if (input == "11") break;
            if (actions.ContainsKey(input)) await actions[input](user);
            else Console.WriteLine("Invalid operation number.");
        }
    }

    private async Task UpdateStakeHolderAsync(User stakeHolder)
    {
        try
        {
            await _userConsoleManager.UpdateUserAsync(stakeHolder);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    private async Task DisplayInfoStakeHolderAndProjectAsync(User stakeHolder)
    {
        try
        {
            Console.Write($"\nYour username: {stakeHolder.Username}\n" +
                          $"Your email: {stakeHolder.Email}\n");
        
            Console.Write("\nYour project(s):");
            await _projectManager.DisplayProjectsAsync(stakeHolder);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    private async Task CheckApprovedTasksAsync(User stakeHolder)
    {
        await _projectManager.CheckApproveTasksCountAsync(stakeHolder);
    }

    private async Task AddFileToTask(User stakeHolder)
    {
        try
        {
            var task = await _userConsoleManager.AddFileToTaskAsync();
            await Service.UpdateProjectByTask(task);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    private async Task UpdateProjectAsync(User stakeHolder)
    {
        await _projectManager.UpdateProjectAsync(stakeHolder);
    }

    private async Task DeleteOneProjectAsync(User stakeHolder)
    {
        try
        {
            await _projectManager.DisplayProjectsAsync(stakeHolder);

            Console.WriteLine($"\nEnter the name of project you want to delete:\nName: ");
            var projectName = Console.ReadLine()!;

            Console.WriteLine("Are you sure? 1 - Yes, 2 - No");
            int choice = int.Parse(Console.ReadLine()!);

            if (choice == 1)
            {
                await Service.DeleteProjectAsync(projectName);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    private async Task DeleteStakeHolderAsync(User stakeHolder)
    {
        try
        {
            Console.WriteLine("Are you sure? 1 - Yes, 2 - No");
            int choice = int.Parse(Console.ReadLine()!);

            if (choice == 1)
            {
                await Service.DeleteStakeHolder(stakeHolder);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    private async Task DeleteTasksAsync(User stakeHolder)
    {
        try
        {
            var projects = await Service.GetProjectsByStakeHolder(stakeHolder);

            if (projects == null)
            {
                Console.WriteLine("Failed to get projects.");
                return;
            }

            foreach (var project in projects)
            {
                var tasks = project.Tasks;

                foreach (var task in tasks)
                {
                    await _projectManager.DisplayOneTaskAsync(task);
                    Console.WriteLine("\nAre you want to delete this task?\n1 - Yes, 2 - No");
                    var option = int.Parse(Console.ReadLine()!);

                    if (option == 1)
                    {
                        await Service.DeleteCurrentTask(task);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    private async Task CreateProjectAsync(User stakeHolder)
    {
        try
        {
            Console.WriteLine("Create project");
            Console.Write("Please, write name of project.\nName: ");
            string projectName = Console.ReadLine()!;
            
            if (await Service.ProjectIsAlreadyExistAsync(projectName)) return;

            string projectDescription;
            Console.WriteLine("Optionally add a description to the project.\nPress 'Enter' to add");
            ConsoleKeyInfo keyInfo = Console.ReadKey();

            if (keyInfo.Key == ConsoleKey.Enter)
                projectDescription = Console.ReadLine()!;
            else
                projectDescription = "empty";

            Console.Write("Enter a due date for the project.\nDue date (dd.MM.yyyy): ");
            string[] date = Console.ReadLine()!.Split('.');
            DateTime enteredDate = await Service.UpdateDueDateInProjectAsync(date);
            await _testerManager.DisplayNameOfAllTester();
            Console.Write("\nWrite the username of the person who will be the tester for this project.\nTester: ");
            string testerName = Console.ReadLine()!;
            var tester = await Service.GetTesterByNameAsync(testerName);
            
            if (tester == null)
            {
                Console.WriteLine(nameof(tester));
                return;
            }

            await Service.CreateProjectAsync(projectName, projectDescription, stakeHolder, enteredDate, tester);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    private async Task CreateTaskToProjectAsync(User stakeHolder)
    {
        try
        {
            await _projectManager.ChooseProjectToAddTasks(stakeHolder);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
}