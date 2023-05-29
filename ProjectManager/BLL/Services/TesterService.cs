using BLL.Abstractions.Interfaces;
using Core.Enums;
using Core.Models;
using DAL.Abstractions.Interfaces;

namespace BLL.Services;

public class TesterService : GenericService<User>, ITesterService
{
    public TesterService(IRepository<User> repository) : base(repository)
    {
    }

    public async Task<IEnumerable<User>> GetAllTester()
    {
        var testers = (await GetAll()).Where(u => u.Role == UserRole.Tester);

        return testers;
    }
    
    public async Task<User> GetTesterByName(string testerName)
    {
        var tester = await GetByPredicate(u => u.Username == testerName && u.Role == UserRole.Tester);

        return tester;
    }
}
