using System.Security.Cryptography;
using System.Text;
using BLL.Abstractions.Interfaces;
using Core.Models;
using DAL.Abstractions.Interfaces;

namespace BLL.Services;

public class UserService : GenericService<User>, IUserService
{
    public UserService(IRepository<User> repository) : base(repository)
    {
    }

    public async Task<User> Authenticate(string userInput, string password)
    {
        if (string.IsNullOrWhiteSpace(userInput)) throw new ArgumentNullException(nameof(userInput));
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));

        User user = await GetByPredicate(u => u.Username == userInput || u.Email == userInput);

        if (user == null) throw new ArgumentNullException(nameof(user));

        return user;
    }

    public async Task<User> GetUserByUsernameOrEmail(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) throw new ArgumentNullException(nameof(input));

        User user = await GetByPredicate(u => u.Username == input || u.Email == input);
        
        if (user == null) throw new ArgumentNullException(nameof(user));

        return user;
    }

    public async Task<List<User>> GetUsersByRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role)) throw new ArgumentNullException(nameof(role));

        IEnumerable<User> users = (await GetAll()).Where(u => u.Role.ToString() == role);
            
        if (users == null) throw new ArgumentNullException(nameof(users));

        return (List<User>)users;
    }

    public async Task UpdatePassword(Guid userId, string newPassword)
    {
        if (userId == Guid.Empty) throw new ArgumentException("userId cannot be empty");
        if (string.IsNullOrWhiteSpace(newPassword)) throw new ArgumentNullException(nameof(newPassword));
    
        User user = await GetById(userId);
        
        if (user == null) throw new ArgumentNullException(nameof(user));
    
        user.PasswordHash = GetPasswordHash(newPassword);
        await Update(userId, user);
    }
}