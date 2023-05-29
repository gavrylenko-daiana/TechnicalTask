using Core.Models;

namespace BLL.Abstractions.Interfaces;

public interface IDeveloperService : IGenericService<User>
{
    Task<User> GetDeveloperByUsernameOrEmail(string? userName);
}