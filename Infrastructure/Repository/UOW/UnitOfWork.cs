using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces.Repos;
using Domain.Interfaces.UOW;

namespace Infrastructure.Repository.UOW
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly CenterDbContext _context;

        public UnitOfWork(CenterDbContext context)
        {
            _context = context;
        }

        // ---------- backing fields ----------
        private IUser? _users;
        private IPerson? _persons;
        private IStudent? _students;
        private IGrade? _grades;
        private ISubjectRepository? _subjects;

        private ISubjectGradeHandlerRepository? _subjectGradeHandlers;
        private IGroup? _groups;
        private IGroupScheduleRepository? _groupSchedules;
        private IClassSession? _classSessions;

        private IStudentRegistration? _studentRegistrations;
        private IStudentGroupAggregationRepository? _studentGroupAggregations;
        private IPayment? _payments;

        private IExam? _exams;
        private IExamResult? _examResults;
        private IExamStatusRepository? _examStatuses;

        private IMessage? _messages;
        private IMessageTypeRepository? _messageTypes;

        private ISessionStatusRepository? _sessionStatuses;

        // ---------- Core ----------
        public IUser Users
            => _users ??= new UserRepository(_context);

        public IPerson Persons
            => _persons ??= new PersonRepository(_context);

        public IStudent Students
            => _students ??= new StudentRepository(_context);

        public IGrade Grades
            => _grades ??= new GradeRepository(_context);

        public ISubjectRepository Subjects
            => _subjects ??= new SubjectRepository(_context);

        // ---------- Academic ----------
        public ISubjectGradeHandlerRepository SubjectGradeHandlers
            => _subjectGradeHandlers ??= new SubjectGradeHandlerRepository(_context);

        public IGroup Groups
            => _groups ??= new GroupRepository(_context);

        public IGroupScheduleRepository GroupSchedules
            => _groupSchedules ??= new GroupScheduleRepository(_context);

        public IClassSession ClassSessions
            => _classSessions ??= new ClassSessionRepository(_context);

        // ---------- Student Operations ----------
        public IStudentRegistration StudentRegistrations
            => _studentRegistrations ??= new StudentRegistrationRepository(_context);

        public IStudentGroupAggregationRepository StudentGroupAggregations
            => _studentGroupAggregations ??= new StudentGroupAggregationRepository(_context);

        public IPayment Payments
            => _payments ??= new PaymentRepository(_context);

        // ---------- Exams ----------
        public IExam Exams
            => _exams ??= new ExamRepository(_context);

        public IExamResult ExamResults
            => _examResults ??= new ExamResultRepository(_context);

        public IExamStatusRepository ExamStatuses
            => _examStatuses ??= new ExamStatusRepository(_context);

        // ---------- Messaging ----------
        public IMessage Messages
            => _messages ??= new MessageRepository(_context);

        public IMessageTypeRepository MessageTypes
            => _messageTypes ??= new MessageTypeRepository(_context);

        // ---------- Session / Lookup ----------
        public ISessionStatusRepository SessionStatuses
            => _sessionStatuses ??= new SessionStatusRepository(_context);

        // ---------- Commit ----------
        public Task<int> SaveChangesAsync()
            => _context.SaveChangesAsync();

        public int SaveChanges()
            => _context.SaveChanges();

        public void Dispose()
            => _context.Dispose();
    }

}
