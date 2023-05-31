using BLL.Abstractions.Interfaces;
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
    //     var tasks = project.ClaimTaskDeveloper.Values
    //         .SelectMany(tasks => tasks)
    //         .ToList();
    //
    //     return tasks;
    // }

    public async Task<IEnumerable<ProjectTask>> GetTasksByUser(User developer)
    {
        var tasks = (await GetAll()).Where(t => t.Developer.Id == developer.Id);
        
        return tasks;
    }

    // public async Task<ProjectTask> GetTaskAfterCreating()
    // {
    //     ProjectTask task = (await GetAll()).LastOrDefault()!;
    //
    //     return task;
    // }
}
