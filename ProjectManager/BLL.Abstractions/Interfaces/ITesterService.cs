using Core.Models;

namespace BLL.Abstractions.Interfaces;

public interface ITesterService : IGenericService<User>
{
    Task<IEnumerable<User>> GetAllTester();

    Task<User> GetTesterByName(string testerName);

    Task<List<ProjectTask>> GetTesterTasksAsync(User tester);

    Task AddCompletedTask(ProjectTask task);

    Task ReturnTaskInProgress(ProjectTask task);

    Task SendMailToUserAsync(string email, string message);

    Task<List<ProjectTask>> GetWaitTasksByTesterAsync(User tester);

    Task UpdateProjectByTask(ProjectTask task);

    Task DeleteTesterAsync(User tester);
}