using System.Net;
using System.Net.Mail;
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
    
    public async Task<int> SendCodeToUser(string email)
    {
        Random rand = new Random();
        int emailCode = rand.Next(1000, 9999);
        string fromMail = "dayana01001@gmail.com";
        string fromPassword = "oxizguygokwxgxgb";

        MailMessage message = new MailMessage();
        message.From = new MailAddress(fromMail);
        message.Subject = "Verify code for update password.";
        message.To.Add(new MailAddress($"{email}"));
        message.Body = $"<html><body> Your code: {emailCode} </body></html>";
        message.IsBodyHtml = true;

        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(fromMail, fromPassword),
            EnableSsl = true,
        };

        smtpClient.Send(message);

        return emailCode;
    }
    
    public async Task SendMessageEmailUserAsync(string email, string messageEmail)
    {
        Random rand = new Random();
        string fromMail = "dayana01001@gmail.com";
        string fromPassword = "oxizguygokwxgxgb";

        MailMessage message = new MailMessage();
        message.From = new MailAddress(fromMail);
        message.Subject = "Change notification.";
        message.To.Add(new MailAddress($"{email}"));
        message.Body = $"<html><body> {messageEmail} </body></html>";
        message.IsBodyHtml = true;

        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(fromMail, fromPassword),
            EnableSsl = true,
        };

        smtpClient.Send(message);
    }
}