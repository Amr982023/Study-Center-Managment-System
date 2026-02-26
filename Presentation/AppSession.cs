using Domain.Models;

namespace Presentation
{
    /// <summary>
    /// Holds the currently authenticated user for the session lifetime.
    /// </summary>
    public static class AppSession
    {
        public static User? CurrentUser { get; private set; }
        public static bool IsAuthenticated => CurrentUser is not null;

        public static void Login(User user) => CurrentUser = user;
        public static void Logout() => CurrentUser = null;

        public static bool HasPermission(string permission)
            => CurrentUser?.Permission?.Equals(permission,
               System.StringComparison.OrdinalIgnoreCase) ?? false;

        public static bool IsAdmin
            => HasPermission("Admin");
    }
}