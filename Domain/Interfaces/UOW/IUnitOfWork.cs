using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces.Repos;

namespace Domain.Interfaces.UOW
{
    public interface IUnitOfWork : IDisposable
    {
        // ---------- Core ----------
        IUser Users { get; }
 
        IStudent Students { get; }
        IGrade Grades { get; }
        ISubjectRepository Subjects { get; }

        // ---------- Academic ----------
        ISubjectGradeHandlerRepository SubjectGradeHandlers { get; }
        IGroup Groups { get; }
        IGroupScheduleRepository GroupSchedules { get; }
        IClassSession ClassSessions { get; }

        // ---------- Student Operations ----------
        IStudentRegistration StudentRegistrations { get; }
        IStudentGroupAggregationRepository StudentGroupAggregations { get; }
        IPayment Payments { get; }

        // ---------- Exams ----------
        IExam Exams { get; }
        IExamResult ExamResults { get; }
        IExamStatusRepository ExamStatuses { get; }          // ✅ ADDED

        // ---------- Messaging ----------
        IMessage Messages { get; }
        IMessageTypeRepository MessageTypes { get; }         // ✅ ADDED

        // ---------- Session / Lookup ----------
        ISessionStatusRepository SessionStatuses { get; }    // ✅ ADDED

        Task<int> SaveChangesAsync();
        int SaveChanges();
    }

}
