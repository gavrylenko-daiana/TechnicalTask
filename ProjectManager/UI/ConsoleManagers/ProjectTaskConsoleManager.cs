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

            Console.Write("Please, write description.\nDescription: ");
            string taskDescription = Console.ReadLine()!;

            Console.Write("Enter a due date for the task.\n" +
                          "(Please note that the deadline should not exceed the deadline for the implementation of the project itself. Otherwise, the term will be set automatically - the maximum.)\n" +
                          "Due date (dd.MM.yyyy): ");
            string[] date = Console.ReadLine()!.Split('.');
            DateTime enteredDate = new DateTime(int.Parse(date[2]), int.Parse(date[1]), int.Parse(date[0]));

            DateTime term = enteredDate <= project.DueDates.Date
                ? enteredDate
                : project.DueDates.Date;

            var priority = Priority.Low;
            Console.WriteLine("Select task priority: \n1) Urgent; 2) High; 3) Medium; 4) Low; 5) Minor;");
            int choice = int.Parse(Console.ReadLine()!);

            try
            {
                priority = choice switch
                {
                    1 => Priority.Urgent,
                    2 => Priority.High,
                    3 => Priority.Medium,
                    4 => Priority.Low,
                    5 => Priority.Minor,
                };
            }
            catch
            {
                Console.WriteLine("Such a type of priority does not exist!");
            }

            var item = new ProjectTask
            {
                Name = taskName,
                Description = taskDescription,
                DueDates = term,
                Priority = priority,
                Tester = project.Tester
            };

            await CreateAsync(item);

            tasks.Add(item);

            Console.WriteLine("Are you want to add next task in this project?\nWrite 'exit' - No, Press 'Enter' - yes");
            exit = Console.ReadLine()!;
        }

        return tasks;
    }

    // public async Task<ProjectTask> GetTaskAfterCreating()
    // {
    //     ProjectTask tasks = await Service.GetTaskAfterCreating();
    //     
    //     return tasks;
    // }

    public async Task DeleteTasksWithProject(Project project)
    {
        foreach (var task in project.Tasks)
        {
            await DeleteAsync(task.Id);
        }
    }

    // public async Task<List<ProjectTask>> GetTasksByProject(Project project)
    // {
    //     List<ProjectTask> tasks = await Service.GetTasksByProject(project);
    //
    //     return tasks;
    // }

    public async Task DisplayTaskAsync(ProjectTask task)
    {
        Console.WriteLine($"Name: {task.Name}");

        if (!string.IsNullOrWhiteSpace(task.Description))
            Console.WriteLine($"Description: {task.Description}");

        if (task.Developer != null)
            Console.WriteLine($"Developer performing task: {task.Developer.Username}");

        Console.WriteLine($"Tester: {task.Tester.Username}");
        Console.WriteLine($"Priority: {task.Priority}");
        Console.WriteLine($"DueDates: {task.DueDates}");
        Console.WriteLine($"Status: {task.Progress}\n");
    }

    public async Task DisplayAllTaskByProject(List<ProjectTask> tasks)
    {
        foreach (var task in tasks)
        {
            await DisplayTaskAsync(task);
        }
    }

    public async Task<int> GetApproveTasksAsync(Project project)
    {
        int countApproveTasks = 0;

        var approveTasks = await Service.GetApproveTasks(project);

        if (approveTasks.Any())
        {
            countApproveTasks += approveTasks.Count;
            Console.WriteLine($"{project.Name} has {approveTasks.Count} approve task:");
            foreach (var task in approveTasks)
            {
                Console.WriteLine($"- {task}");
            }
        }

        return countApproveTasks;
    }

    public async Task<List<ProjectTask>> GetDeveloperTasks(User developer)
    {
        try
        {
            var tasks = await Service.GetTasksByDeveloper(developer);
            return tasks;
        }
        catch
        {
            Console.WriteLine($"Task list is empty.");
        }

        return null!;
    }

    public async Task DeleteDeveloperFromTasksAsync(List<ProjectTask> tasks)
    {
        if (tasks.Any())
        {
            foreach (var task in tasks)
            {
                task.Developer = null!;
                await UpdateAsync(task.Id, task);
            }
        }
    }

    public async Task<List<ProjectTask>> GetTesterTasksAsync(User tester)
    {
        var tasks = await Service.GetTasksByTester(tester);

        return tasks;
    }

    public override Task PerformOperationsAsync(User user)
    {
        throw new NotImplementedException();
    }
}