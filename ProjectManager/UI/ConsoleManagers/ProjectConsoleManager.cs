using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using UI.Interfaces;

namespace UI.ConsoleManagers;

public class ProjectConsoleManager : ConsoleManager<IProjectService, Project>, IConsoleManager<Project>
{
    private readonly ProjectTaskConsoleManager _projectTaskManager;
    private readonly UserConsoleManager _userManager;

    public ProjectConsoleManager(IProjectService service, ProjectTaskConsoleManager projectTaskManager,
        UserConsoleManager userManager) : base(service)
    {
        _projectTaskManager = projectTaskManager;
        _userManager = userManager;
    }

    public async Task DisplayProjectAsync(Project project)
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
        var tasks = await _projectTaskManager.CreateTaskAsync(project);
        project.Tasks.AddRange(tasks);
        project.CountAllTasks = project.Tasks.Count;

        await UpdateAsync(project.Id, project);
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

            var tasks = project.Tasks.Where(t => t.Progress == Progress.CompletedStakeHolder);
            
            if (project.CountDoneTasks == project.CountAllTasks && !tasks.Any())
            {
                await DisplayProjectAsync(project);
                Console.WriteLine("Do you want to approve this project?\nEnter 1 - Yes, 2 - No");
                int choice = int.Parse(Console.ReadLine()!);

                if (choice == 1)
                {
                    for (int i = 0; i < project.Tasks.Count; i++)
                    {
                        project.Tasks[i].Progress = Progress.CompletedStakeHolder;
                        await _projectTaskManager.UpdateAsync(project.Tasks[i].Id, project.Tasks[i]);
                    }
                    
                    await UpdateAsync(project.Id, project);
                }
                else if (choice == 2)
                {
                    for (int i = 0; i < project.Tasks.Count; i++)
                    {
                        project.Tasks[i].Progress = Progress.WaitingTester;
                        project.CountDoneTasks -= 1;
                        await _projectTaskManager.UpdateAsync(project.Tasks[i].Id, project.Tasks[i]);
                    }
                    
                    await UpdateAsync(project.Id, project);
                    
                    Console.WriteLine("Select the reason for rejection:\n" +
                                      "1. Expired due date\n" +
                                      "2. Need to fix");
                    int option = int.Parse(Console.ReadLine()!);

                    if (option == 1)
                    {
                        await _userManager.SendMessageEmailUser(project.Tester.Email,
                            $"The task with the name {project.Name} and the deadline of {project.DueDates} has expired.\nThe message was sent from the Stake Holder - {project.StakeHolder.Username}.");
                    }
                    else if (option == 2)
                    {
                        await _userManager.SendMessageEmailUser(project.Tester.Email,
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
                Console.WriteLine("4. Exit");

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
                        project.DueDates = new DateTime(int.Parse(date[2]), int.Parse(date[1]), int.Parse(date[0]));
                        Console.WriteLine("Due date was successfully edited");
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Invalid operation number.");
                        break;
                }

                await UpdateAsync(project.Id, project);
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

    public async Task<List<Project>> GetTesterProjects(User tester)
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

    public async Task DeleteProjectAsync(string projectName)
    {
        var project = await Service.GetProjectByName(projectName);
        await _projectTaskManager.DeleteTasksWithProject(project);
        await DeleteAsync(project.Id);
    }

    public async Task DeleteProjectsWithSteakHolderAsync(User stakeHolder)
    {
        var projects = await Service.GetProjectsByStakeHolder(stakeHolder);

        foreach (var project in projects)
        {
            await _projectTaskManager.DeleteTasksWithProject(project);
            await DeleteAsync(project.Id);
        }
    }

    public async Task<Project> GetProjectByName(string projectName)
    {
        var project = await Service.GetProjectByName(projectName);

        return project;
    }

    public async Task<Project> GetProjectByTaskAsync(ProjectTask task)
    {
        var project = await Service.GetProjectByTask(task);

        return project;
    }

    public async Task<List<Project>> GetProjectsByStakeHolder(User stakeHolder)
    {
        var projects = await Service.GetProjectsByStakeHolder(stakeHolder);
        if (projects == null) Console.WriteLine($"Failed to get a stake holder for the project");

        return projects!;
    }

    public async Task DeleteCurrentTaskAsync(ProjectTask task)
    {
        var project = await Service.GetProjectByTask(task);

        if (project != null && project.Tasks.Any())
        {
            project.Tasks.RemoveAll(x => x.Id == task.Id);
            project.CountAllTasks -= 1;
            await UpdateAsync(project.Id, project);
            await _projectTaskManager.DeleteTaskAsync(task);
        }
        else
        {
            Console.WriteLine($"Failed to get project");
        }
    }

    public async Task UpdateTasksAsync(ProjectTask task)
    {
        var project = await Service.GetProjectByTask(task);

        if (project != null && project.Tasks.Any())
        {
            await _projectTaskManager.UpdateTaskAsync(task);
            await UpdateAsync(project.Id, project);
        }
        else
        {
            Console.WriteLine($"Failed to get project");
        }
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