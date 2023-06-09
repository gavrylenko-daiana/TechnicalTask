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

    public async Task<List<ProjectTask>> CreateTaskAsync(Project project)
    {
        var tasks = new List<ProjectTask>();
        string exit = String.Empty;

        while (exit != "exit")
        {
            Console.WriteLine("Create task");
            Console.Write("Please, write name of task.\nName: ");
            string taskName = Console.ReadLine()!;

            if (await Service.ProjectTaskIsAlreadyExist(taskName)) return null!;

            Console.Write("Please, write description.\nDescription: ");
            string taskDescription = Console.ReadLine()!;
            Console.Write("Enter a due date for the task.\n" +
                          "(Please note that the deadline should not exceed the deadline for the implementation of the project itself. Otherwise, the term will be set automatically - the maximum.)\n" +
                          "Due date (dd.MM.yyyy): ");
            string[] date = Console.ReadLine()!.Split('.');
            var term = await Service.CreateDueDateForTask(project, date);
            var priority = Priority.Low;
            Console.WriteLine("Select task priority: \n1) Urgent; 2) High; 3) Medium; 4) Low; 5) Minor;");
            int choice = int.Parse(Console.ReadLine()!);
            priority = await Service.GetPriority(choice, priority);

            var item = await Service.CreateTaskAsync(taskName, taskDescription, term, priority, project.Tester);

            tasks.Add(item);
            Console.WriteLine("Are you want to add next task in this project?\nWrite 'exit' - No, Press 'Enter' - yes");
            exit = Console.ReadLine()!;
        }

        return tasks;
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