using Application.Repositories;
using Infrastructure.Database.Repositories;
using Infrastructure.Email;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddSingleton<EmailService>();
        
        return services;
    }
}