using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using DAL.Abstractions.Interfaces;

namespace BLL.Services;

public class DeveloperService : UserService, IDeveloperService
{
    public DeveloperService(IRepository<User> repository) : base(repository)
    {
    }

    public async Task<User> GetDeveloperByUsername(string? username)
    {
        if (string.IsNullOrWhiteSpace(username)) throw new ArgumentNullException(nameof(username));

        User developer = await GetByPredicate(a => a.Role == UserRole.Developer && a.Username == username);
            
        if (developer == null) throw new ArgumentNullException(nameof(developer));

        return developer;
    }
}