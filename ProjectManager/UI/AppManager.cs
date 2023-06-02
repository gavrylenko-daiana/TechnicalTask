using UI.ConsoleManagers;

namespace UI;

public class AppManager
{
    private readonly InitialConsoleManager _helperManager;

    public AppManager(InitialConsoleManager helperConsoleManager)
    {
        _helperManager = helperConsoleManager;
    }

    public async Task StartAsync()
    {
        while (true)
        {
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("\nAre you already registered?:");
            Console.WriteLine("1. Yes, I want to sign in.");
            Console.WriteLine("2. No, I want to register.");
            Console.WriteLine("3. Exit");

            Console.Write("Enter the operation number: ");
            string input = Console.ReadLine()!;

            switch (input)
            {
                case "1":
                    await _helperManager.AuthenticateUser();
                    break;
                case "2":
                    await _helperManager.CreateUserAsync();
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
