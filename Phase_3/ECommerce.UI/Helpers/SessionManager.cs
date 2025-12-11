using ECommerce.Models;

namespace ECommerce.UI.Helpers
{
    public static class SessionManager
    {
        public static User? CurrentUser { get; set; }
        public static bool IsLoggedIn => CurrentUser != null;
        public static bool IsAdmin { get; set; }
    }
}
