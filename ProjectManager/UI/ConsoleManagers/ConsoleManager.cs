using System.Net;
using System.Net.Mail;
using BLL.Abstractions.Interfaces;
using BLL.Services;
using Core.Models;

namespace UI.ConsoleManagers;

public abstract class ConsoleManager<TService, TEntity>
    where TEntity : BaseEntity
    where TService : IGenericService<TEntity>
{
    protected readonly TService Service;
    private readonly IUserService _userService;

    protected ConsoleManager(TService service)
    {
        Service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public abstract Task PerformOperationsAsync();

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        try
        {
            return await Service.GetAll();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetAllAsync: {ex.Message}");

            return Enumerable.Empty<TEntity>();
        }
    }

    public virtual async Task<TEntity> GetByIdAsync(Guid id)
    {
        try
        {
            return await Service.GetById(id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetByIdAsync: {ex.Message}");

            return null!;
        }
    }

    public virtual async Task<TEntity> GetByPredicateAsync(Func<TEntity, bool> predicate)
    {
        try
        {
            return await Service.GetByPredicate(predicate);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetByPredicateAsync: {ex.Message}");

            return null!;
        }
    }

    public virtual async Task CreateAsync(TEntity entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        try
        {
            await Service.Add(entity);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CreateAsync: {ex.Message}");
        }
    }

    public virtual async Task UpdateAsync(Guid id, TEntity entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        try
        {
            await Service.Update(id, entity);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdateAsync: {ex.Message}");
        }
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        try
        {
            await Service.Delete(id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DeleteAsync: {ex.Message}");
        }
    }
    
     public async Task UpdateUserPassword(User getAdmin)
    {
        string check = String.Empty;

        while (check != "exit")
        {
            Console.WriteLine("Write \n" +
                              "'exit' if you don't want to change your password \n" +
                              "'forgot' if you forgot your password.");
            check = Console.ReadLine()!;
            if (check == "exit") break;
            if (check == "forgot")
            {
                await ForgotUserPassword();
                break;
            }

            Console.Write("Enter you current password.\nYour password:");
            string password = Console.ReadLine()!;
            password = Service.GetPasswordHash(password);

            if (getAdmin.PasswordHash == password)
            {
                Console.Write("Enter your new password.\nYour Password: ");
                string newUserPassword = Console.ReadLine()!;
                newUserPassword = Service.GetPasswordHash(newUserPassword);

                await _userService.UpdatePassword(getAdmin.Id, newUserPassword);
            }
            else
            {
                Console.WriteLine($"You entered the wrong password");
            }
        }
    }

    private async Task ForgotUserPassword()
    {
        Console.Write("Please, write your email:\nEmail:");
        string email = Console.ReadLine()!;
        var getUser = await _userService.GetByPredicate(u => u.Email == email);

        if (getUser == null) throw new ArgumentException("User was not found");

        int emailCode = await SendMessageEmailUser(email);
        Console.WriteLine("Write the four-digit number that came to your email:");
        int userCode = int.Parse(Console.ReadLine()!);

        if (userCode == emailCode)
        {
            Console.Write("Enter your new password.\nNew Password: ");
            string newUserPassword = Console.ReadLine()!;
            newUserPassword = Service.GetPasswordHash(newUserPassword);

            await _userService.UpdatePassword(getUser.Id, newUserPassword);
        }
        else
        {
            throw new ArgumentException("You entered the wrong code.");
        }
    }

    public async Task<int> SendMessageEmailUser(string email)
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
}