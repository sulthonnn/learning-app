using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using ServiceLearningApp.Model;

namespace ServiceLearningApp.Security
{
    public class Policies
    {
        public static void AddPolicies(AuthorizationOptions options)
        {
            options.AddPolicy("Bearer", policy =>
            {
                policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser().Build();
            });

            options.AddPolicy("Teacher", policy =>
            {
                policy.RequireRole(Role.Teacher);
            });

            options.AddPolicy("Student", policy =>
            {
                policy.RequireRole(Role.Student);
            });

        }
    }
}
