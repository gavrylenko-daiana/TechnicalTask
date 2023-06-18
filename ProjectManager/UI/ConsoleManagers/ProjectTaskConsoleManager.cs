using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using UI.Interfaces;

namespace UI.ConsoleManagers;

public class ProjectTaskConsoleManager : ConsoleManager<IProjectTaskService, ProjectTask>, IConsoleManager<ProjectTask>
{
    public ProjectTaskConsoleManager(IProjectTaskService service) : base(service)
    {
    }

    public async Task DisplayTaskAsync(ProjectTask task)
    {
        Console.WriteLine($"\nName: {task.Name}");

        if (!string.IsNullOrWhiteSpace(task.Description))
            Console.WriteLine($"Description: {task.Description}");

        if (task.Developer != null)
            Console.WriteLine($"Developer performing task: {task.Developer.Username}");

        Console.WriteLine($"Tester: {task.Tester.Username}");
        Console.WriteLine($"Priority: {task.Priority}");
        Console.WriteLine($"DueDates: {task.DueDates.Date}");
        Console.WriteLine($"Status: {task.Progress}\n");
    }

    public async Task DisplayAllTaskByProject(List<ProjectTask> tasks)
    {
        if (tasks.Any())
        {
            foreach (var task in tasks)
            {
                await DisplayTaskAsync(task);
            }
        }
        else
        {
            Console.WriteLine("Tasks list is empty");
        }
    }

    public async Task DisplayAllTasks()
    {
        var tasks = await GetAllAsync();
        var projectTasks = tasks.ToList();

        if (projectTasks.Any())
        {
            foreach (var task in projectTasks)
            {
                await DisplayTaskAsync(task);
            }
        }
        else
        {
            throw new Exception("Tasks list is empty");
        }
    }
    
    public override Task PerformOperationsAsync(User user)
    {
        throw new NotImplementedException();
    }
}