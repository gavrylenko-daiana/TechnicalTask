using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using DAL.Abstractions.Interfaces;

namespace BLL.Services;

public class StakeHolderService : GenericService<User>, IStakeHolderService
{
    public StakeHolderService(IRepository<User> repository) : base(repository)
    {
    }

    public async Task<User> GetStakeHolderByUsername(string? username)
    {
        if (string.IsNullOrWhiteSpace(username)) throw new ArgumentNullException(nameof(username));

        User developer = await GetByPredicate(a => a.Role == UserRole.StakeHolder && a.Username == username);
            
        if (developer == null) throw new ArgumentNullException(nameof(developer));

        return developer;
    }
}
