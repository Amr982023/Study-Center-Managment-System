using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Services;
using Application.ServicesInterfaces;
using Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application.DependencyInjection
{
    public static class ApplicationServiceCollectionExtensions
    {

        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            services.AddScoped<IEnrollmentAppService,EnrollmentAppService>();

            services.AddScoped<EnrollmentDomainService>();

            return services;
        }

    }
}
