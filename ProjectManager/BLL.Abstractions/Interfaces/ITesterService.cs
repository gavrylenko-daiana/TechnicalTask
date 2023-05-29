using Core.Models;

namespace BLL.Abstractions.Interfaces;

public interface ITesterService : IGenericService<User>
{
    Task<IEnumerable<User>> GetAllTester();

    Task<User> GetTesterByName(string testerName);
}