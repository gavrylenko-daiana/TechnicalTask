using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using DAL.Abstractions.Interfaces;

namespace BLL.Services;

public class DeveloperService : GenericService<User>, IDeveloperService
{
    public DeveloperService(IRepository<User> repository) : base(repository)
    {
    }

    public async Task<User> GetDeveloperByUsernameOrEmail(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) throw new ArgumentNullException(nameof(input));

        User stakeHolder = await GetByPredicate(u => u.Role == UserRole.Developer && (u.Username == input  || u.Email == input));
        
        if (stakeHolder == null) throw new ArgumentNullException(nameof(stakeHolder));

        return stakeHolder;
    }
    
    public async Task<IEnumerable<User>> GetAllDeveloper()
    {
        var developer = (await GetAll()).Where(u => u.Role == UserRole.Developer);

        return developer;
    }
}