using Core.Models;

namespace BLL.Abstractions.Interfaces;

public interface IStakeHolderService : IGenericService<User>
{
    Task<User> GetStakeHolderByUsername(string? username);
}