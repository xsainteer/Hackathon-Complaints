using System.Net;
using System.Net.Mail;
using Application.Repositories;
using Infrastructure.AI;
using Infrastructure.AI.Ollama;
using Infrastructure.AI.Vectors;
using Infrastructure.Database.Repositories;
using Infrastructure.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddSingleton<EmailService>();
        
        services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
        
        services.AddSingleton<SmtpClient>(sp =>
        {
            var smtpSettings = sp.GetRequiredService<IOptions<SmtpSettings>>().Value;
            var client = new SmtpClient(smtpSettings.Host, smtpSettings.Port)
            {
                Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password),
                EnableSsl = true
            };
            return client;
        });
        
        services.Configure<OllamaSettings>(configuration.GetSection("OllamaSettings"));

        services.AddScoped<OllamaClient>();
        
        services.Configure<QDrantSettings>(configuration.GetSection("QDrantSettings"));
        
        services.AddSingleton<QdrantClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<QDrantSettings>>().Value;

            if (string.IsNullOrWhiteSpace(options.Host))
                throw new Exception("QDrantSettings.Host is not configured.");

            return new QdrantClient(
                host: options.Host,
                port: options.Port,
                https: false,
                apiKey: null,
                grpcTimeout: TimeSpan.FromSeconds(30),
                loggerFactory: sp.GetRequiredService<ILoggerFactory>());
        });

        services.AddScoped<VectorService>();
        
        return services;
    }
}