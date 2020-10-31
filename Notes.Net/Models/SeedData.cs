using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Notes.Net.Models
{
    public static class SeedData
    {
        private const string adminUser = "admin";
        private const string adminPassword = "admin";

        public static async void EnsurePopulated(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            NotesDbContext context = serviceScope.ServiceProvider.GetRequiredService<NotesDbContext>();
            context.Database.Migrate();

            UserManager<User> userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
            User user = await userManager.FindByNameAsync(adminUser);
            var passwordHasher = new PasswordHasher<User>();
            if (user == null)
            {
                user = new User() { Name = adminUser };
                user.Passwort = passwordHasher.HashPassword(user, adminPassword);
                user.Admin = true;
                context.Users.Add(user);
                context.SaveChanges();
            }
        }
    }
}
