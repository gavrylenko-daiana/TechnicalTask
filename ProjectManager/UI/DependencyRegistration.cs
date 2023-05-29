using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Core.Models;
using UI.ConsoleManagers;
using UI.Interfaces;

namespace UI;

public class DependencyRegistration
{
    public static IServiceProvider Register()
    {
        var services = new ServiceCollection();

        services.AddScoped<AppManager>();
        services.AddScoped<UserConsoleManager>();
        services.AddScoped<ProjectConsoleManager>();
        services.AddScoped<ProjectTaskConsoleManager>();
        services.AddScoped<DeveloperConsoleManager>();
        services.AddScoped<TesterConsoleManager>();
        services.AddScoped<StakeHolderConsoleManager>();
        services.AddScoped<InitialConsoleManager>();
        services.AddScoped<DevelopTasksConsoleManager>();

        foreach (Type type in typeof(IConsoleManager<>).Assembly.GetTypes()
                     .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces()
                         .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsoleManager<>))))
        {
            Type interfaceType = type.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsoleManager<>));
            services.AddScoped(interfaceType, type);
        }

        BLL.DependencyRegistration.RegisterServices(services);

        return services.BuildServiceProvider();
    }
}