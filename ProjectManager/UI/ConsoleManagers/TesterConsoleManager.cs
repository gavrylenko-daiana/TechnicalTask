using BLL.Abstractions.Interfaces;
using BLL.Services;
using Core.Models;
using DAL.Abstractions.Interfaces;
using UI.Interfaces;

namespace UI.ConsoleManagers;

public class TesterConsoleManager : ConsoleManager<ITesterService, User>, IConsoleManager<User>
{
    public TesterConsoleManager(ITesterService service) : base(service)
    {
    }
    
    public override Task PerformOperationsAsync(User user)
    {
        throw new NotImplementedException();
    }

    public async Task DisplayAllTester()
    {
        IEnumerable<User> testers = await Service.GetAllTester();

        foreach (var tester in testers)
        {
            Console.WriteLine($"\nName: {tester.Username}");
            Console.WriteLine($"{tester.Email}");
        }
    }

    public async Task<User> GetTesterByName(string name)
    {
        var tester = await Service.GetTesterByName(name);

        return tester;
    }
}