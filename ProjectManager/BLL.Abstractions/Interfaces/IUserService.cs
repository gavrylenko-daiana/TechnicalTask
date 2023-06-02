using Core.Models;

namespace BLL.Abstractions.Interfaces;

public interface IUserService : IGenericService<User>
{
    Task<User> Authenticate(string userInput, string password);

    Task<User> GetUserByUsernameOrEmail(string input);
        
    Task<List<User>> GetUsersByRole(string role);

    Task UpdatePassword(Guid getUserId, string newUserPassword);

    Task<int> SendCodeToUser(string email);

    Task SendMessageEmailUserAsync(string email, string messageEmail);
}
