using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceLearningApp.Model
{
    public class ApplicationUser : IdentityUser
    {
        public override string UserName { get => base.UserName; set => base.UserName = value; }
        public override string? Email { get => base.Email; set => base.Email = value; }
        public override string PasswordHash { get => base.PasswordHash; set => base.PasswordHash = value; }
        public string? NISN { get; set; }
        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }

        [ForeignKey("FkImageId")]
        public Upload Image { get; set; }
        public int? FkImageId { get; set; }

    }
}
