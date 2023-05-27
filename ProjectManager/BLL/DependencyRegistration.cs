using BLL.Abstractions.Interfaces;
using BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BLL;

public class DependencyRegistration
{
    public static void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IProjectTaskService, ProjectTaskService>();
        services.AddScoped<IDeveloperService, DeveloperService>();
        services.AddScoped<ITesterService, TesterService>();
        services.AddScoped<IStakeHolderService, StakeHolderService>();

        DAL.DependencyRegistration.RegisterRepositories(services);
    }
}