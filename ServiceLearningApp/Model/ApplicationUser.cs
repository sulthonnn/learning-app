using Microsoft.AspNetCore.Identity;

namespace ServiceLearningApp.Model
{
    public class ApplicationUser : IdentityUser
    {
        public override string UserName { get => base.UserName; set => base.UserName = value; }
        public override string? Email { get => base.Email; set => base.Email = value; }
        public override string PasswordHash { get => base.PasswordHash; set => base.PasswordHash = value; }
        public string? NISN { get; set; }
        public string? FullName { get; set; }

    }
}
