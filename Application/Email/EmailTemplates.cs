using System;
using Domain.Models;

namespace Application.Email
{
    /// <summary>
    /// All email message templates for the system.
    /// Each method returns a ready-to-send message body string.
    /// </summary>
    public static class EmailTemplates
    {
        // ── 1. Payment Receipt ────────────────────────────────────────────────
        public static string PaymentReceipt(Student student, Payment payment, string centerName) =>
            $"""
            Dear {student.FullName},

            This is a confirmation that your payment has been successfully recorded.

            ┌─────────────────────────────────────┐
              Payment Receipt
            ├─────────────────────────────────────┤
              Student   : {student.FullName}
              Code      : {student.Code}
              Amount    : {payment.Amount:C}
              Month     : {new DateTime(2000, payment.Month, 1):MMMM}
              Date      : {payment.DateTime:MMM dd, yyyy}
              Recorded By: {payment.PerformedBy?.UserName ?? "-"}
            └─────────────────────────────────────┘

            Thank you for your payment.

            {centerName}
            """;

        // ── 2. Exam Result ────────────────────────────────────────────────────
        public static string ExamResult(Student student, Exam exam, ExamResult result, string centerName)
        {
            string status = result.ExceptFullMark ? "Excused" : (result.Result >= exam.FullMark * 0.5 ? "Passed ✓" : "Failed ✗");
            string scoreStr = result.ExceptFullMark ? "Excused" : $"{result.Result} / {exam.FullMark}";

            return $"""
            Dear {student.FullName},

            Your exam result has been recorded.

            ┌─────────────────────────────────────┐
              Exam Result
            ├─────────────────────────────────────┤
              Student   : {student.FullName}
              Exam      : {exam.Name}
              Group     : {exam.Group?.Name ?? "-"}
              Score     : {scoreStr}
              Status    : {status}
            └─────────────────────────────────────┘

            {centerName}
            """;
        }

        // ── 3. Enrollment Confirmation ────────────────────────────────────────
        public static string EnrollmentConfirmation(Student student, Group group, string centerName) =>
            $"""
            Dear {student.FullName},

            You have been successfully enrolled in a new group.

            ┌─────────────────────────────────────┐
              Enrollment Confirmation
            ├─────────────────────────────────────┤
              Student   : {student.FullName}
              Code      : {student.Code}
              Group     : {group.Name}
              Subject   : {group.SubjectGrade?.Subject?.Name ?? "-"}
              Grade     : {group.SubjectGrade?.Grade?.Name ?? "-"}
            └─────────────────────────────────────┘

            We look forward to seeing you in class!

            {centerName}
            """;

        // ── 4. Monthly Fee Reminder ───────────────────────────────────────────
        public static string MonthlyFeeReminder(Student student, int month, string centerName) =>
            $"""
            Dear {student.FullName},

            This is a friendly reminder that your payment for {new DateTime(2000, month, 1):MMMM} has not been recorded yet.

            ┌─────────────────────────────────────┐
              Payment Reminder
            ├─────────────────────────────────────┤
              Student   : {student.FullName}
              Code      : {student.Code}
              Month Due : {new DateTime(2000, month, 1):MMMM}
            └─────────────────────────────────────┘

            Please visit us to complete your payment as soon as possible.

            {centerName}
            """;

        // ── 5. Exam Reminder ──────────────────────────────────────────────────
        public static string ExamReminder(Student student, Exam exam, string centerName) =>
            $"""
            Dear {student.FullName},

            This is a reminder that you have an upcoming exam tomorrow.

            ┌─────────────────────────────────────┐
              Exam Reminder
            ├─────────────────────────────────────┤
              Student   : {student.FullName}
              Exam      : {exam.Name}
              Group     : {exam.Group?.Name ?? "-"}
              Full Mark : {exam.FullMark}
              Date      : {exam.ExamDate:dddd, MMM dd, yyyy}
              Time      : {exam.ExamDate:hh:mm tt}
            └─────────────────────────────────────┘

            Good luck!

            {centerName}
            """;
    }
}