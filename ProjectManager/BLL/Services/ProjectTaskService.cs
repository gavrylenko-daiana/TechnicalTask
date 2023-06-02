using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using DAL.Abstractions.Interfaces;

namespace BLL.Services;

public class ProjectTaskService : GenericService<ProjectTask>, IProjectTaskService
{
    public ProjectTaskService(IRepository<ProjectTask> repository) : base(repository)
    {
    }

    // public async Task<List<ProjectTask>> GetTasksByProject(Project project)
    // {
    //     var tasks = project.Tasks.All(tasks).ToList();
    //
    //     return tasks;
    // }

    public async Task<List<ProjectTask>> GetTasksByDeveloper(User developer)
    {
        var tasks = (await GetAll()).Where(t => t.Developer != null && t.Developer.Id == developer.Id).ToList();

        return tasks;
    }
    
    public async Task<List<ProjectTask>> GetTasksByTester(User tester)
    {
        var tasks = (await GetAll()).Where(t => t.Tester != null && t.Tester.Id == tester.Id).ToList();

        return tasks;
    }

    public async Task<List<ProjectTask>> GetWaitTasksByTester(User tester)
    {
        var tasks = (await GetAll()).Where(t => t.Tester != null
                                                && t.Tester.Id == tester.Id
                                                && t.Progress == Progress.WaitingTester).ToList();
        
        return tasks;
    }

    public Task<List<ProjectTask>> GetApproveTasks(Project project)
    {
        var approveTasks = project.Tasks.Where(t => t.Progress == Progress.CompletedTester).ToList();
        
        return Task.FromResult(approveTasks);
    }

    public async Task<Priority> GetPriority(int choice, Priority priority)
    {
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

            return priority;
        }
        catch
        {
            Console.WriteLine("Such a type of priority does not exist!");
            return 0;
        }
    }

    // public async Task<ProjectTask> GetTaskAfterCreating()
    // {
    //     ProjectTask task = (await GetAll()).LastOrDefault()!;
    //
    //     return task;
    // }
}