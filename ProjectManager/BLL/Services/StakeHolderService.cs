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

    public async Task<User> GetStakeHolderByUsernameOrEmail(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) throw new ArgumentNullException(nameof(input));

        User stakeHolder = await GetByPredicate(u => u.Role == UserRole.StakeHolder && (u.Username == input  || u.Email == input));
        
        if (stakeHolder == null) throw new ArgumentNullException(nameof(stakeHolder));

        return stakeHolder;
    }

}
