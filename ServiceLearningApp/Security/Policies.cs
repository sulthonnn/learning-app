using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
                policy.RequireRole(Role.Student);
            });

            options.AddPolicy("EditPassword", policy =>
                policy.RequireRole(Role.Student)
                        .RequireAssertion(context =>
                            context.User.HasClaim(c =>
                                (c.Type == ClaimTypes.Role && c.Value == Role.Student) &&
                                (c.Type == ClaimTypes.NameIdentifier && c.Value == context.User.FindFirstValue(ClaimTypes.NameIdentifier))
                            )
                        )
            );

        }
    }
}
