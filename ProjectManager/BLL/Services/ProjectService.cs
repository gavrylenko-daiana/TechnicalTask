using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using DAL.Abstractions.Interfaces;

namespace BLL.Services;

public class ProjectService : GenericService<Project>, IProjectService
{
    public ProjectService(IRepository<Project> repository) : base(repository)
    {
    }

    // public async Task<List<User>?> GetDevelopersByProject(Project project)
    // {
    //     var getDevelopers = project.ClaimTaskDeveloper.Keys
    //         .Where(u => u.Role == UserRole.Developer).ToList();
    //
    //     if (getDevelopers == null)
    //         throw new NullReferenceException("Developers is not responsible for this project");
    //     
    //     return getDevelopers;
    // }

    public async Task<Project> GetProjectByName(string projectName)
    {
        Project project = await GetByPredicate(p => p.Name == projectName);
        
        if (project == null) throw new ArgumentNullException(nameof(project));

        return project;
    }

    public async Task<List<Project>> GetProjectsByStakeHolder(User stakeHolder)
    {
        List<Project> projects = (await GetAll()).Where(p => p.StakeHolder != null && p.StakeHolder.Id == stakeHolder.Id).ToList();

        return projects;
    }

    public async Task<Project> GetProjectByTask(ProjectTask task)
    {
        var project = await GetByPredicate(p => p.Tasks.Any(t => t.Id == task.Id));
        
        return project;
    }
}
