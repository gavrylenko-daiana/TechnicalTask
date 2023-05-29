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

    public async Task DisplayAllTester()
    {
        IEnumerable<User> testers = await Service.GetAllTester();

        foreach (var tester in testers)
        {
            Console.WriteLine($"Name: {tester.Username}");
            Console.WriteLine($"{tester.Email}\n");
        }
    }
    
    public override Task PerformOperationsAsync()
    {
        throw new NotImplementedException();
    }
}