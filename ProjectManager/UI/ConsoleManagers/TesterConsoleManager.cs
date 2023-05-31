using BLL.Abstractions.Interfaces;
using BLL.Services;
using Core.Models;
using DAL.Abstractions.Interfaces;
using UI.Interfaces;

namespace UI.ConsoleManagers;

public class TesterConsoleManager : ConsoleManager<ITesterService, User>, IConsoleManager<User>
{
    private ProjectTaskConsoleManager _projectTaskManager; // для проверки через прогресс какие задачи Tester должен проверить
    private ProjectConsoleManager _projectManager; // для обновления количеста выполненых задач в проекте
    public TesterConsoleManager(ITesterService service, ProjectTaskConsoleManager projectTaskManager, ProjectConsoleManager projectManager) : base(service)
    {
        _projectTaskManager = projectTaskManager;
        _projectManager = projectManager;
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