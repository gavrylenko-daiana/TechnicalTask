using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using UI.Interfaces;

namespace UI.ConsoleManagers;

public class ProjectConsoleManager : ConsoleManager<IProjectService, Project>, IConsoleManager<Project>
{
    private readonly ProjectTaskConsoleManager _projectTaskManager;

    public ProjectConsoleManager(IProjectService service, ProjectTaskConsoleManager projectTaskManager) : base(service)
    {
        _projectTaskManager = projectTaskManager;
    }

    private async Task DisplayProjectAsync(Project project)
    {
        Console.WriteLine($"\nName: {project.Name}");

        if (!string.IsNullOrWhiteSpace(project.Description))
            Console.WriteLine($"Description: {project.Description}");

        Console.WriteLine($"Stake Holder: {project.StakeHolder.Username} with email: {project.StakeHolder.Email}");
        Console.WriteLine($"Tester: {project.Tester.Username} with email: {project.Tester.Email}");
        Console.WriteLine($"Number of all tasks: {project.CountAllTasks}");
        Console.WriteLine($"Number of done tasks: {project.CountDoneTasks}");
        Console.WriteLine($"DueDates: {project.DueDates.Date}");

        if (project.Tasks != null && project.Tasks.Count > 0)
        {
            Console.WriteLine($"\nTask(s):");
            foreach (var task in project.Tasks)
            {
                await _projectTaskManager.DisplayTaskAsync(task);
            }
        }
    }

    public async Task DisplayProjectsAsync(User user)
    {
        IEnumerable<Project> projects = await Service.GetProjectsByStakeHolder(user);

        if (!projects.Any())
        {
            Console.WriteLine("Project list is empty");
            return;
        }

        foreach (var project in projects)
        {
            await DisplayProjectAsync(project);
        }
    }

    public async Task DisplayAllProjectsAsync()
    {
        IEnumerable<Project> projects = await GetAllAsync();

        foreach (var project in projects)
        {
            await DisplayProjectsAsync(project.StakeHolder);
        }
    }

    public async Task ChooseProjectToAddTasks(User stakeHolder)
    {
        await DisplayProjectsAsync(stakeHolder);

        Console.Write($"\nEnter name of project you want to add tasks.\nName of project: ");
        var projectName = Console.ReadLine();

        try
        {
            var project = await Service.GetProjectByName(projectName!);
            await CreateTaskForProject(project);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"This name does not exist.");
        }
    }

    private async Task CreateTaskForProject(Project project)
    {
        try
        {
            var tasks = await _projectTaskManager.CreateTaskAsync(project);
            await Service.AddTaskToProject(project, tasks);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public async Task CheckApproveTasksCountAsync(User stakeHolder)
    {
        var projects = await Service.GetProjectsByStakeHolder(stakeHolder);

        if (!projects.Any())
        {
            Console.WriteLine($"Stake Holder has no projects");
            return;
        }

        foreach (var project in projects)
        {
            Console.WriteLine($"{project.Name} has {project.CountDoneTasks} approve task " +
                              $"out of {project.CountAllTasks}.");

            var tasks = await Service.GetCompletedTask(project);

            if (project.CountDoneTasks == project.CountAllTasks && !tasks.Any())
            {
                await DisplayProjectAsync(project);
                Console.WriteLine("Do you want to approve this project?\nEnter 1 - Yes, 2 - No");
                int choice = int.Parse(Console.ReadLine()!);

                if (choice == 1)
                {
                    await Service.UpdateToCompletedProject(project);
                }
                else if (choice == 2)
                {
                    await Service.UpdateToWaitingTask(project);
                    Console.WriteLine("Select the reason for rejection:\n" +
                                      "1. Expired due date\n" +
                                      "2. Need to fix");
                    int option = int.Parse(Console.ReadLine()!);

                    if (option == 1)
                    {
                        await Service.SendMailToUser(project.Tester.Email,
                            $"The task with the name {project.Name} and the deadline of {project.DueDates} has expired.\nThe message was sent from the Stake Holder - {project.StakeHolder.Username}.");
                    }
                    else if (option == 2)
                    {
                        await Service.SendMailToUser(project.Tester.Email,
                            $"The task with the name {project.Name} needs to be fixed.\nThe message was sent from the Stake Holder - {project.StakeHolder.Username}.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid operation number.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid operation number.");
                }
            }
        }
    }

    public async Task UpdateProjectAsync(User stakeHolder)
    {
        await DisplayProjectsAsync(stakeHolder);

        Console.Write($"\nEnter name of project you want to update.\nName: ");
        var projectName = Console.ReadLine()!;

        var project = await Service.GetProjectByName(projectName);

        if (project != null)
        {
            while (true)
            {
                Console.WriteLine("\nSelect which information you want to change: ");
                Console.WriteLine("1. Name");
                Console.WriteLine("2. Description");
                Console.WriteLine("3. Due date");
                Console.WriteLine("4. Tasks");
                Console.WriteLine("5. Exit");

                Console.Write("Enter the operation number: ");
                string input = Console.ReadLine()!;

                switch (input)
                {
                    case "1":
                        Console.Write("Please, edit name.\nName: ");
                        project.Name = Console.ReadLine()!;
                        Console.WriteLine("Name was successfully edited");
                        break;
                    case "2":
                        Console.Write("Please, edit description.\nDescription: ");
                        project.Description = Console.ReadLine()!;
                        Console.WriteLine("Description was successfully edited");
                        break;
                    case "3":
                        Console.Write("Please, edit a due date for the project.\nDue date (dd.MM.yyyy): ");
                        string[] date = Console.ReadLine()!.Split('.');
                        project.DueDates = await Service.UpdateDueDateInProject(date);
                        Console.WriteLine("Due date was successfully edited");
                        break;
                    case "4":
                        await UpdateTaskAsync(project);
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Invalid operation number.");
                        break;
                }

                await UpdateAsync(project.Id, project);
            }
        }
    }

    public async Task UpdateTaskInProjectAsync(Project project)
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
                            await Service.UpdateDueDateInTask(task, date);
                            Console.WriteLine("Due date was successfully edited");
                            break;
                        case "4":
                            return;
                        default:
                            Console.WriteLine("Invalid operation number.");
                            break;
                    }

                    await _projectTaskManager.UpdateAsync(task.Id, task);
                    await Service.UpdateProject(project);
                }
            }
        }
    }

    public async Task DeleteTesterFromProjectsAsync(User tester)
    {
        var projects = await GetTesterProjects(tester);

        if (projects.Any())
        {
            foreach (var project in projects)
            {
                project.Tester = null!;
                await UpdateAsync(project.Id, project);
            }
        }
    }

    private async Task<List<Project>> GetTesterProjects(User tester)
    {
        try
        {
            var projects = await Service.GetProjectByTester(tester);
            return projects;
        }
        catch
        {
            Console.WriteLine($"Task list is empty.");
        }

        return null!;
    }

    //------------------

    public async Task<Project> GetProjectByName(string projectName)
    {
        var project = await Service.GetProjectByName(projectName);

        return project;
    }

    public async Task DeleteCurrentTaskAsync(ProjectTask task)
    {
        try
        {
            await Service.DeleteCurrentTaskAsync(task);
        }
        catch
        {
            Console.WriteLine($"Failed to get project");
        }
    }

    private async Task UpdateTaskAsync(Project project)
    {
        if (project == null)
        {
            Console.WriteLine("Failed to get project.");
            return;
        }

        var tasks = project.Tasks;

        foreach (var task in tasks)
        {
            await DisplayOneTaskAsync(task);
            Console.WriteLine("\nAre you want to update this task?\n1 - Yes, 2 - No");
            var option = int.Parse(Console.ReadLine()!);

            if (option == 1)
            {
                await UpdateTasksAsync(task);
                await UpdateAsync(project.Id, project);
            }
        }
    }

    private async Task UpdateTasksAsync(ProjectTask task)
    {
        var project = await Service.GetProjectByTask(task);

        if (project != null && project.Tasks.Any())
        {
            await UpdateTaskInProjectAsync(project);
        }
        else
        {
            Console.WriteLine($"Failed to get project");
        }
    }

    public async Task<bool> ProjectIsAlreadyExistAsync(string name)
    {
        var check = await Service.ProjectIsAlreadyExist(name);

        return check;
    }

    public async Task DisplayOneTaskAsync(ProjectTask task)
    {
        await _projectTaskManager.DisplayTaskAsync(task);
    }

    public override Task PerformOperationsAsync(User user)
    {
        throw new NotImplementedException();
    }
}