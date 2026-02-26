using System;
using System.Windows.Forms;
using Application.Services;
using Application.ServicesInterfaces;
using Domain.Interfaces;
using Domain.Interfaces.UOW;
using Infrastructure;
using Infrastructure.Repository.UOW;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Forms;
using Presentation.UserControls;


// Alias to avoid conflict between "Application" project namespace
// and System.Windows.Forms.Application
using WinApp = System.Windows.Forms.Application;

namespace Presentation
{
    public class ServiceLocator
    {
        private readonly IServiceProvider _provider;
        public ServiceLocator(IServiceProvider provider) => _provider = provider;
        public T Resolve<T>() where T : notnull => _provider.GetRequiredService<T>();
    }

    internal static class Program
    {
        public static ServiceLocator ServiceLocator { get; private set; } = null!;

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var services = new ServiceCollection();
            ConfigureServices(services);

            var provider = services.BuildServiceProvider();
            ServiceLocator = new ServiceLocator(provider);

            WinApp.Run(provider.GetRequiredService<LoginForm>());
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CenterDbContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

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

            services.AddTransient<LoginForm>();
            services.AddTransient<MainShell>();
            services.AddTransient<DashboardPage>();
            services.AddTransient<StudentsPage>();
            services.AddTransient<StudentDialog>();
            services.AddTransient<GroupsPage>();
            services.AddTransient<GroupDialog>();
            services.AddTransient<EnrollStudentDialog>();
            services.AddTransient<GroupScheduleDialog>();
            services.AddTransient<SessionsPage>();
            services.AddTransient<SessionDialog>();
            services.AddTransient<AttendanceDialog>();
            services.AddTransient<ExamsPage>();
            services.AddTransient<ExamDialog>();
            services.AddTransient<ExamResultsDialog>();
            services.AddTransient<PaymentsPage>();
            services.AddTransient<PaymentDialog>();
            services.AddTransient<UsersPage>();
            services.AddTransient<UserDialog>();
            services.AddTransient<LookupsPage>();
        }
    }
}