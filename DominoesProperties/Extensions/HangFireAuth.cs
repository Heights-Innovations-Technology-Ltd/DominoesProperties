using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace DominoesProperties.Extensions
{
    public class HangFireAuth : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            return httpContext.User.IsInRole("ADMIN") || httpContext.User.IsInRole("SUPER");
        }
    }
}

