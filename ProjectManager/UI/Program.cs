using Microsoft.Extensions.DependencyInjection;

namespace UI;

class Program
{
    static async Task Main(string[] args)
    {
        var serviceProvider = DependencyRegistration.Register();

        using (var scope = serviceProvider.CreateScope())
        {
            var appManager = scope.ServiceProvider.GetService<AppManager>();
            appManager.StartAsync().Wait();
        }
    }
}
