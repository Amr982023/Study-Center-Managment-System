using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Validation
{
    public static class Validator
    {
        // ── Phone ─────────────────────────────────────────────────────────────
        // Must be exactly 11 digits and start with 01
        private static readonly Regex PhoneRegex = new(@"^01[0-9]{9}$", RegexOptions.Compiled);

        public static bool IsValidPhone(string phone) =>
            !string.IsNullOrWhiteSpace(phone) && PhoneRegex.IsMatch(phone.Trim());

        public static string ValidatePhone(string phone) =>
            IsValidPhone(phone) ? null : "Phone must be 11 digits and start with 01.";

        // ── Email ─────────────────────────────────────────────────────────────
        private static readonly Regex EmailRegex = new(
            @"^[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static bool IsValidEmail(string email) =>
            !string.IsNullOrWhiteSpace(email) && EmailRegex.IsMatch(email.Trim());

        public static string ValidateEmail(string email) =>
            IsValidEmail(email) ? null : "Enter a valid email address (e.g. user@example.com).";

        // ── Name ──────────────────────────────────────────────────────────────
        // Not empty, letters and spaces only, 2–50 chars
        private static readonly Regex NameRegex = new(@"^[\p{L}\s]{2,50}$", RegexOptions.Compiled);

        public static bool IsValidName(string name) =>
            !string.IsNullOrWhiteSpace(name) && NameRegex.IsMatch(name.Trim());

        public static string ValidateName(string name, string fieldName = "Name") =>
            IsValidName(name) ? null : $"{fieldName} must be 2–50 letters only.";

        // ── Password ──────────────────────────────────────────────────────────
        public static bool IsValidPassword(string password) =>
            !string.IsNullOrWhiteSpace(password) && password.Length >= 6;

        public static string ValidatePassword(string password, string confirm = null)
        {
            if (!IsValidPassword(password))
                return "Password must be at least 6 characters.";
            if (confirm != null && password != confirm)
                return "Passwords do not match.";
            return null;
        }

        // ── Required ──────────────────────────────────────────────────────────
        public static string ValidateRequired(string value, string fieldName) =>
            string.IsNullOrWhiteSpace(value) ? $"{fieldName} is required." : null;

        // ── Convenience: validate multiple required fields at once ────────────
        // Pass tuples of (value, fieldName) — returns first error found or null
        public static string ValidateAllRequired(params (string value, string field)[] fields)
        {
            foreach (var (value, field) in fields)
            {
                var err = ValidateRequired(value, field);
                if (err != null) return err;
            }
            return null;
        }
    }
}
