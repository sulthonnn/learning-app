using Microsoft.AspNetCore.Authorization;
using ServiceLearningApp.Model;
using System.Security.Claims;

namespace ServiceLearningApp.Security
{
    public class EditUserRequirement : IAuthorizationRequirement { }
    public class EditUserHandler : AuthorizationHandler<EditUserRequirement, ApplicationUser>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EditUserRequirement requirement, ApplicationUser resource)
        {
            var isTeacher = context.User.HasClaim(e => e.Type == ClaimTypes.Role && e.Value == Role.Teacher);
            var isSameUser = context.User.Claims.FirstOrDefault(e => e.Type == ClaimTypes.NameIdentifier && e.Value == resource.Id) != null;

            if (isTeacher || isSameUser)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
