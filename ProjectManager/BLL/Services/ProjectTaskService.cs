using BLL.Abstractions.Interfaces;
using Core.Models;
using DAL.Abstractions.Interfaces;

namespace BLL.Services;

public class ProjectTaskService : GenericService<ProjectTask>, IProjectTaskService
{
    public ProjectTaskService(IRepository<ProjectTask> repository) : base(repository)
    {
    }
    
    public Task Add(ProjectTask obj)
    {
        throw new NotImplementedException();
    }

    public Task Delete(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<ProjectTask> GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    

    public Task<ProjectTask> GetByPredicate(Func<ProjectTask, bool> predicate)
    {
        throw new NotImplementedException();
    }

    public Task Update(Guid id, ProjectTask obj)
    {
        throw new NotImplementedException();
    }
}