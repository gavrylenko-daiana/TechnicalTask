using BLL.Abstractions.Interfaces;
using Core.Models;
using DAL.Abstractions.Interfaces;

namespace BLL.Services;

public class ProjectTaskService : GenericService<ProjectTask>, IProjectTaskService
{
    public ProjectTaskService(IRepository<ProjectTask> repository) : base(repository)
    {
    }

    public ICollection<List<ProjectTask>> GetTasksByProject(Project project)
    {
        return project.ClaimTaskDeveloper.Values;
    }
}
