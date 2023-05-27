using BLL.Services;
using Core.Models;
using UI.ConsoleManagers;

namespace UI;

public class AppManager
{
    private readonly UserConsoleManager _userManager;

    public AppManager(UserConsoleManager userManager)
    {
        _userManager = userManager;
    }

    public async Task StartAsync()
    {
        while (true)
        {
            Console.WriteLine("\nAre you already registered?:");
            Console.WriteLine("1. Yes, I want to sign in.");
            Console.WriteLine("2. No, I want to register.");
            Console.WriteLine("3. Exit");

            Console.Write("Enter the operation number: ");
            string input = Console.ReadLine()!;

            switch (input)
            {
                case "1":
                    await _userManager.AuthenticateUser();
                    break;
                case "2":
                    await _userManager.CreateUserAsync();
                    break;
                case "3":
                    return;
                default:
                    Console.WriteLine("Invalid operation number.");
                    break;
            }
        }
    }
}
