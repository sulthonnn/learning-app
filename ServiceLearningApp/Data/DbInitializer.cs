using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ServiceLearningApp.Data;
using ServiceLearningApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ServiceEsgDataHub.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            // Dapatkan layanan yang diperlukan dari penyedia layanan
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Daftar nama pengguna yang akan menjadi admin
            var usernames = new List<string> { "teacher@teacher.com" };

            foreach (var userName in usernames)
            {
                // Pastikan basis data telah dibuat
                context.Database.EnsureCreated();

                // Periksa apakah pengguna sudah ada
                var existingUser = await userManager.FindByNameAsync(userName);

                // Jika pengguna belum ada, buat pengguna baru
                if (existingUser == null)
                {
                    var password = "teacherteacher";
                    ApplicationUser user = new ApplicationUser
                    {
                        UserName = userName,
                        Email = userName,
                        EmailConfirmed = true
                    };

                    // Buat pengguna baru
                    var result = await userManager.CreateAsync(user, password);

                    // Periksa apakah pengguna berhasil dibuat
                    if (!result.Succeeded)
                    {
                        // Tangani kesalahan pembuatan pengguna di sini (misalnya, log, lempar pengecualian, dll.)
                        throw new System.InvalidOperationException("Can't create user");
                    }
                    else
                    {
                        // Buat peran "Administrator" jika belum ada
                        var teacherRole = await roleManager.FindByNameAsync("Teacher");
                        if (teacherRole == null)
                        {
                            teacherRole = new IdentityRole("Teacher");
                            await roleManager.CreateAsync(teacherRole);
                        }

                        // Tambahkan pengguna ke peran "Administrator"
                        await userManager.AddToRoleAsync(user, "Teacher");

                        // Tambahkan klaim peran jika belum ada
                        if (!context.UserClaims.Any(r => r.ClaimType == ClaimTypes.Role && r.ClaimValue == "Teacher"))
                            await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "Teacher"));
                    }
                }
            }
        }
    }
}
