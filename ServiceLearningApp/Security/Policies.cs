using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using ServiceLearningApp.Helpers;
using ServiceLearningApp.Model;
using System.Security.Claims;

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
                policy.RequireRole(Role.Teacher, Role.Student);
            });

        }
    }
}
