using BLL.Abstractions.Interfaces;
using Core.Models;
using UI.Interfaces;

namespace UI.ConsoleManagers;

public class ProjectTaskConsoleManager : ConsoleManager<IProjectTaskService, ProjectTask>, IConsoleManager<ProjectTask>
{
    public ProjectTaskConsoleManager(IProjectTaskService service) : base(service)
    {
    }

    public override Task PerformOperationsAsync()
    {
        throw new NotImplementedException();
    }
    
}