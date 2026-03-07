using System.Collections.Generic;
using Domain.Common.Roles;
using Domain.Models;

namespace Presentation
{
    public static class AppSession
    {
        public static User? CurrentUser { get; private set; }
        public static bool IsAuthenticated => CurrentUser is not null;
        public static string CenterName { get; set; } = "My Center";

        public static void Login(User user) => CurrentUser = user;
        public static void Logout() => CurrentUser = null;

        // Parse the stored string permission to the enum (case-insensitive, default Admin)
        public static UserPermission Permission
        {
            get
            {
                if (CurrentUser?.Permission is string p &&
                    System.Enum.TryParse<UserPermission>(p, ignoreCase: true, out var result))
                    return result;
                return UserPermission.Staff;
            }
        }

        public static bool IsAdmin => Permission == UserPermission.Admin;
        public static bool IsManager => Permission == UserPermission.Admin || Permission == UserPermission.Manager;

        // Pages each role can see
        private static readonly Dictionary<UserPermission, HashSet<string>> _accessMap = new()
        {
            [UserPermission.Admin] = new() { "Students", "Groups", "Sessions", "Exams", "Payments", "Users", "Lookups" },
            [UserPermission.Manager] = new() { "Students", "Groups", "Sessions", "Exams", "Payments" },
            [UserPermission.Teacher] = new() { "Students", "Groups", "Sessions", "Exams" },
            [UserPermission.Staff] = new() { "Students", "Payments" },
        };

        public static bool CanAccess(string page) =>
            _accessMap.TryGetValue(Permission, out var pages) && pages.Contains(page);
    }
}