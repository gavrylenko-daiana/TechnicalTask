using System.Net;
using System.Net.Mail;
using BLL.Abstractions.Interfaces;
using BLL.Services;
using Core.Enums;
using Core.Models;
using UI.Interfaces;

namespace UI.ConsoleManagers;

public class UserConsoleManager : ConsoleManager<IUserService, User>, IConsoleManager<User>
{
    public UserConsoleManager(IUserService service) : base(service)
    {
    }

    public override async Task PerformOperationsAsync()
    {
        Dictionary<string, Func<Task>> actions = new Dictionary<string, Func<Task>>
        {
            { "1", DisplayAllUsersAsync },
            { "2", DeleteUserAsync }
        };

        while (true)
        {
            Console.WriteLine("\nUser operations:");
            Console.WriteLine("1. Display all users");
            Console.WriteLine("2. Delete a user");
            Console.WriteLine("3. Exit");

            Console.Write("Enter the operation number: ");
            string input = Console.ReadLine()!;

            if (input == "3") break;

            if (actions.ContainsKey(input))
                await actions[input]();
            else
                Console.WriteLine("Invalid operation number.");
        }
    }

    public async Task DisplayAllUsersAsync()
    {
        IEnumerable<User> users = await GetAllAsync();

        foreach (var user in users)
        {
            Console.WriteLine($"\nName: {user.Username}" +
                              $"\nEmail: {user.Email}" +
                              $"\nRole: {user.Role}");
        }
    }
    
     public async Task UpdateUserPassword(User getUser)
    {
        string check = String.Empty;

        while (check != "exit")
        {
            Console.WriteLine("Write \n" +
                              "'exit' if you don't want to change your password \n" +
                              "'forgot' if you forgot your password.\n" +
                              "Press 'Enter' for continue update your password.");
            check = Console.ReadLine()!;
            if (check == "exit") break;
            if (check == "forgot")
            {
                await ForgotUserPassword(getUser);
                break;
            }

            Console.Write("Enter you current password.\nYour password: ");
            string password = Console.ReadLine()!;
            password = Service.GetPasswordHash(password);

            if (getUser.PasswordHash == password)
            {
                Console.Write("Enter your new password.\nYour Password: ");
                string newUserPassword = Console.ReadLine()!;

                await Service.UpdatePassword(getUser.Id, newUserPassword);
            }
            else
            {
                Console.WriteLine($"You entered the wrong password");
            }
        }
    }

    private async Task ForgotUserPassword(User getUser)
    {
        int emailCode = await SendMessageEmailUser(getUser.Email);

        Console.WriteLine("Write the four-digit number that came to your email:");
        int userCode = int.Parse(Console.ReadLine()!);

        if (userCode == emailCode)
        {
            Console.Write("Enter your new password.\nNew Password: ");
            string newUserPassword = Console.ReadLine()!;

            await Service.UpdatePassword(getUser.Id, newUserPassword);
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

    public async Task<User> AuthenticateUser(string userInput, string password)
    {
        User getUser = await Service.Authenticate(userInput, password);

        return getUser;
    }

    public async Task DeleteUserAsync()
    {
        Console.Write("Enter your name.\nYour name: ");
        string userName = Console.ReadLine() ?? throw new ArgumentNullException("Console.ReadLine()");

        User getUserName = await Service.GetUserByUsernameOrEmail(userName);
        await DeleteAsync(getUserName.Id);
    
        Console.WriteLine("The user was successfully deleted");
    }
}