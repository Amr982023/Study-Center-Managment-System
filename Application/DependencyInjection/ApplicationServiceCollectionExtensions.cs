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
            services.AddScoped<IGradeService, GradeService>();
            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<ISubjectGradeHandlerService, SubjectGradeHandlerService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IGroupScheduleService, GroupScheduleService>();
            services.AddScoped<IClassSessionService, ClassSessionService>();
            services.AddScoped<IStudentGroupAggregationService, StudentGroupAggregationService>();
            services.AddScoped<IStudentRegistrationService, StudentRegistrationService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IExamService, ExamService>();
            services.AddScoped<IExamResultService, ExamResultService>();
            services.AddScoped<IExamStatusService, ExamStatusService>();
            services.AddScoped<ISessionStatusService, SessionStatusService>();
            services.AddScoped<IMessageTypeService, MessageTypeService>();
            services.AddScoped<IMessageService, MessageService>();


            return services;
        }

    }
}
