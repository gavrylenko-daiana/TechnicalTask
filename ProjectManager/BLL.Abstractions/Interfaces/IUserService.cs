using Core.Enums;
using Core.Models;

namespace BLL.Abstractions.Interfaces;

public interface IUserService : IGenericService<User>
{
    Task<bool> UsernameIsAlreadyExist(string name);
    
    Task<User> Authenticate(string userInput);

    Task<User> GetUserByUsernameOrEmail(string input);

    Task UpdatePassword(Guid getUserId, string newUserPassword);

    Task<int> SendCodeToUser(string email);

    Task SendMessageEmailUserAsync(string email, string messageEmail);

    Task AddFileFromUserAsync(string path, ProjectTask projectTask);

    Task<ProjectTask> GetTaskByNameAsync(string taskName);

    Task AddNewUser(string userName, string userEmail, string password, UserRole role);

    Task<UserRole> GetRole(UserRole role, int choice);
}
