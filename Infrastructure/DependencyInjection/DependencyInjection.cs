using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Email;
using Application.ServicesInterfaces.Email;
using Application.ServicesInterfaces.Security;
using Application.Settings;
using Infrastructure.Security;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, EmailSettings emailSettings)
        {
            // Security
            services.AddSingleton<IPasswordHasher, PasswordHasher>();

            // Email
            services.AddSingleton(emailSettings);
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<EmailNotificationService>();

            return services;
        }
    }
}
