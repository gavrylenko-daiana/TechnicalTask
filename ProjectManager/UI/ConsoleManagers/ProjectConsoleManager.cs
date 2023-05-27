using BLL.Abstractions.Interfaces;
using Core.Models;
using UI.Interfaces;

namespace UI.ConsoleManagers;

public class ProjectConsoleManager : ConsoleManager<IProjectService, Project>, IConsoleManager<Project>
{
    public ProjectConsoleManager(IProjectService service) : base(service)
    {
    }

    public override Task PerformOperationsAsync()
    {
        throw new NotImplementedException();
    }
    
    public async Task DisplayAllProjectsAsync()
    {
        IEnumerable<Project> projects = await GetAllAsync();

        foreach (var project in projects)
        {
            Console.WriteLine($"Name: {project.Name}");
            Console.WriteLine($"Description: {project.Description}");
            Console.WriteLine($"StakeHolder: {project.StakeHolder}");
            Console.WriteLine($"DueDates: {project.DueDates}");
            Console.WriteLine($"Status: {project.Progress}");
        }
    }
}