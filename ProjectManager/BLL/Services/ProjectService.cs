using BLL.Abstractions.Interfaces;
using Core.Models;
using DAL.Abstractions.Interfaces;

namespace BLL.Services;

public class ProjectService : GenericService<Project>, IProjectService
{
    private object _stakeHolder;

    public ProjectService(IRepository<Project> repository) : base(repository)
    {
    }

    // public async Task<List<Project>> GetProjectByStakeHolder(Guid adminId)
    // {
    //     if (adminId == Guid.Empty) throw new ArgumentException("adminId cannot be empty");
    //
    //     User admin = await _stakeHolder.GetById(adminId);
    //         
    //     if (admin == null) throw new ArgumentNullException(nameof(admin));
    //
    //     IEnumerable<Project> subscription = (await GetAll()).Where(s => s.Admin.Equals(admin));
    //         
    //     if (subscription == null) throw new ArgumentNullException(nameof(subscription));
    //
    //     return (List<Project>)subscription;
    // }
    

}
