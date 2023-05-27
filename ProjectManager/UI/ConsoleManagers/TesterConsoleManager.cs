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
    
    public override Task PerformOperationsAsync()
    {
        throw new NotImplementedException();
    }

    public async Task CreateTesterAsync()
    {
        
    }
}