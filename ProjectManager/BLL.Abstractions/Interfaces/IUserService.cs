using Core.Models;

namespace BLL.Abstractions.Interfaces;

public interface IUserService : IGenericService<User>
{
    Task<bool> UsernameIsAlreadyExist(string name);
    
    Task<User> Authenticate(string userInput, string password);

    Task<User> GetUserByUsernameOrEmail(string input);
        
    Task<List<User>> GetUsersByRole(string role);

    Task UpdatePassword(Guid getUserId, string newUserPassword);

    Task<int> SendCodeToUser(string email);

    Task SendMessageEmailUserAsync(string email, string messageEmail);

    Task AddFileFromUserAsync(string path, ProjectTask projectTask);

    Task<ProjectTask> GetTaskByNameAsync(string taskName);
}
