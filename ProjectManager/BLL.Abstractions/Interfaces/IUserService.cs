using Core.Models;

namespace BLL.Abstractions.Interfaces;

public interface IUserService : IGenericService<User>
{
    Task<User> Authenticate(string username, string email, string password);

    Task<User> GetUserByUsernameOrEmail(string input);
        
    Task<List<User>> GetUsersByRole(string role);

    Task ResetPassword(Guid userId);
    Task UpdatePassword(Guid getUserId, string newUserPassword);
}
