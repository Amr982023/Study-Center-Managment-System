using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.ServicesInterfaces.Email;
using Application.Settings;
using Domain.Models;

namespace Application.Email
{
    public class EmailNotificationService
    {
        private readonly IEmailService _email;
        private readonly CenterSettings _center;

        public EmailNotificationService(IEmailService email, CenterSettings center)
        {
            _email = email;
            _center = center;
        }

        // ── 1. Payment Receipt ────────────────────────────────────────────────
        /// <summary>Send receipt to student after payment is recorded.</summary>
        public async Task<(bool sent, string error)> SendPaymentReceiptAsync(Student student, Payment payment)
        {
            if (string.IsNullOrWhiteSpace(student.Email))
                return (false, "Student has no email address.");
            try
            {
                var body = EmailTemplates.PaymentReceipt(student, payment, _center.CenterName);
                await _email.SendMessageAsync(student.Email, body);
                return (true, null);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        // ── 2. Exam Result ────────────────────────────────────────────────────
        /// <summary>Notify student of their exam result.</summary>
        public async Task<(bool sent, string error)> SendExamResultAsync(Student student, Exam exam, ExamResult result)
        {
            if (string.IsNullOrWhiteSpace(student.Email))
                return (false, "Student has no email address.");
            try
            {
                var body = EmailTemplates.ExamResult(student, exam, result, _center.CenterName);
                await _email.SendMessageAsync(student.Email, body);
                return (true, null);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        // ── 3. Enrollment Confirmation ────────────────────────────────────────
        /// <summary>Confirm enrollment to student after joining a group.</summary>
        public async Task<(bool sent, string error)> SendEnrollmentConfirmationAsync(Student student, Group group)
        {
            if (string.IsNullOrWhiteSpace(student.Email))
                return (false, "Student has no email address.");
            try
            {
                var body = EmailTemplates.EnrollmentConfirmation(student, group, _center.CenterName);
                await _email.SendMessageAsync(student.Email, body);
                return (true, null);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        // ── 4. Monthly Fee Reminder (bulk) ────────────────────────────────────
        /// <summary>
        /// Send reminders to all students in the list (unpaid this month).
        /// Returns a summary: how many sent, how many skipped (no email).
        /// </summary>
        public async Task<(int sent, int skipped, List<string> errors)> SendMonthlyRemindersAsync(
            IEnumerable<Student> unpaidStudents, int month)
        {
            int sent = 0, skipped = 0;
            var errors = new List<string>();

            foreach (var student in unpaidStudents)
            {
                if (string.IsNullOrWhiteSpace(student.Email)) { skipped++; continue; }
                try
                {
                    var body = EmailTemplates.MonthlyFeeReminder(student, month, _center.CenterName);
                    await _email.SendMessageAsync(student.Email, body);
                    sent++;
                }
                catch (Exception ex)
                {
                    errors.Add($"{student.FullName}: {ex.Message}");
                }
            }

            return (sent, skipped, errors);
        }

        // ── 5. Exam Reminder (bulk) ───────────────────────────────────────────
        /// <summary>
        /// Send exam reminders to all enrolled students for exams scheduled tomorrow.
        /// </summary>
        public async Task<(int sent, int skipped, List<string> errors)> SendExamRemindersAsync(
            IEnumerable<Student> enrolledStudents, Exam exam)
        {
            int sent = 0, skipped = 0;
            var errors = new List<string>();

            foreach (var student in enrolledStudents)
            {
                if (string.IsNullOrWhiteSpace(student.Email)) { skipped++; continue; }
                try
                {
                    var body = EmailTemplates.ExamReminder(student, exam, _center.CenterName);
                    await _email.SendMessageAsync(student.Email, body);
                    sent++;
                }
                catch (Exception ex)
                {
                    errors.Add($"{student.FullName}: {ex.Message}");
                }
            }

            return (sent, skipped, errors);
        }
    }
}
