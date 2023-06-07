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
            DateTime enteredDate = new DateTime(int.Parse(date[2]), int.Parse(date[1]), int.Parse(date[0]));

            DateTime now = DateTime.Now;
            if (enteredDate < now)
            {
                enteredDate = now.AddDays(1);
            }

            DateTime term = enteredDate <= project.DueDates.Date
                ? enteredDate
                : project.DueDates.Date;

            var priority = Priority.Low;
            Console.WriteLine("Select task priority: \n1) Urgent; 2) High; 3) Medium; 4) Low; 5) Minor;");
            int choice = int.Parse(Console.ReadLine()!);

            priority = await Service.GetPriority(choice, priority);

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

    public async Task DeleteTasksWithProject(Project project)
    {
        foreach (var task in project.Tasks)
        {
            await DeleteTaskAsync(task);
        }
    }

    public async Task DeleteTaskAsync(ProjectTask task)
    {
        await DeleteAsync(task.Id);
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

    public async Task UpdateTaskAsync(Project project)
    {
        var tasks = project.Tasks;
        
        foreach (var task in tasks)
        {
            Console.WriteLine($"Are you want to update {task.Name}?\nEnter '1' - Yes, '2' - No");
            var option = int.Parse(Console.ReadLine()!);
        
            if (option == 1)
            {
                while (true)
                {
                    Console.WriteLine("\nSelect which information you want to change: ");
                    Console.WriteLine("1. Name");
                    Console.WriteLine("2. Description");
                    Console.WriteLine("3. Due date");
                    Console.WriteLine("4. Exit");

                    Console.Write("Enter the operation number: ");
                    string input = Console.ReadLine()!;

                    switch (input)
                    {
                        case "1":
                            Console.Write("Please, edit name.\nName: ");
                            task.Name = Console.ReadLine()!;
                            Console.WriteLine("Name was successfully edited");
                            break;
                        case "2":
                            Console.Write("Please, edit description.\nDescription: ");
                            task.Description = Console.ReadLine()!;
                            Console.WriteLine("Description was successfully edited");
                            break;
                        case "3":
                            Console.Write("Please, edit a due date for the task.\nDue date (dd.MM.yyyy): ");
                            string[] date = Console.ReadLine()!.Split('.');
                            task.DueDates = new DateTime(int.Parse(date[2]), int.Parse(date[1]), int.Parse(date[0]));
                            Console.WriteLine("Due date was successfully edited");
                            break;
                        case "4":
                            return;
                        default:
                            Console.WriteLine("Invalid operation number.");
                            break;
                    }

                    await UpdateAsync(task.Id, task);
                }
            }
        }
    }

    public async Task AddFileFromUserAsync(string path, ProjectTask projectTask)
    {
        await Service.AddFileToDirectory(path, projectTask);
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

    private async Task<List<ProjectTask>> GetTesterTasks(User tester)
    {
        try
        {
            var tasks = await Service.GetTasksByTester(tester);
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

    public async Task DeleteTesterFromTasksAsync(User tester)
    {
        var tasks = await GetTesterTasks(tester);

        if (tasks.Any())
        {
            foreach (var task in tasks)
            {
                task.Tester = null!;
                await UpdateAsync(task.Id, task);
            }
        }
    }

    public async Task<List<ProjectTask>> GetWaitTasksByTesterAsync(User tester)
    {
        var tasks = await Service.GetWaitTasksByTester(tester);

        return tasks;
    }

    public async Task<List<ProjectTask>> GetTesterTasksAsync(User tester)
    {
        var tasks = await Service.GetWaitTasksByTester(tester);

        return tasks;
    }

    public override Task PerformOperationsAsync(User user)
    {
        throw new NotImplementedException();
    }

    public async Task<ProjectTask> GetTaskByNameAsync(string taskName)
    {
        var task = await Service.GetTaskByName(taskName);
        if (task == null) throw new ArgumentNullException(nameof(task));

        return task;
    }
}