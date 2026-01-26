using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class CenterDbContext : DbContext
    { 
        public DbSet<Person> Persons => Set<Person>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Grade> Grades => Set<Grade>();
        public DbSet<Subject> Subjects => Set<Subject>();

       
        public DbSet<SubjectGradeHandler> SubjectGradeHandlers => Set<SubjectGradeHandler>();
        public DbSet<Group> Groups => Set<Group>();
        public DbSet<GroupSchedule> GroupSchedules => Set<GroupSchedule>();
        public DbSet<ClassSession> ClassSessions => Set<ClassSession>();

       
        public DbSet<StudentRegistration> StudentRegistrations => Set<StudentRegistration>();
        public DbSet<StudentGroupAggregation> StudentGroupAggregations => Set<StudentGroupAggregation>();
        public DbSet<Payment> Payments => Set<Payment>();

      
        public DbSet<Exam> Exams => Set<Exam>();
        public DbSet<ExamResult> ExamResults => Set<ExamResult>();
        public DbSet<ExamStatus> ExamStatuses => Set<ExamStatus>();

     
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<MessageType> MessageTypes => Set<MessageType>();

        public DbSet<SessionStatus> SessionStatuses => Set<SessionStatus>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CenterDbContext).Assembly);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           
        }
    }
}
