using DAL.Abstractions.Interfaces;
using DAL.Services;
using Microsoft.Extensions.DependencyInjection;


namespace DAL;

public class DependencyRegistration
{
    public static void RegisterRepositories(IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    }
}