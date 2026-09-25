using AppointmentsManagementSystem.Data;
using AppointmentsManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AppointmentsManagementSystem
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<ApplicationUser>(
                options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddControllersWithViews();

            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Create Roles and Admin User
            using (var scope = app.Services.CreateScope())
            {
                var roleManager =
                    scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                var userManager =
                    scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                // Create Roles
                string[] roles = { "Admin", "Staff", "Customer" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        var roleResult =
                            await roleManager.CreateAsync(new IdentityRole(role));

                        if (!roleResult.Succeeded)
                        {
                            throw new InvalidOperationException(
                                string.Join(", ",
                                    roleResult.Errors.Select(e => e.Description)));
                        }
                    }
                }

                // Get Admin credentials from User Secrets
                string adminEmail = builder.Configuration["AdminSeed:Email"]
                    ?? throw new InvalidOperationException(
                        "Admin email is not configured.");

                string adminPassword = builder.Configuration["AdminSeed:Password"]
                    ?? throw new InvalidOperationException(
                        "Admin password is not configured.");

                // Check if Admin already exists
                var adminUser =
                    await userManager.FindByEmailAsync(adminEmail);

                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FullName= "System Admin",
                        EmailConfirmed = true
                    };

                    var result =
                        await userManager.CreateAsync(
                            adminUser,
                            adminPassword);

                    if (!result.Succeeded)
                    {
                        throw new InvalidOperationException(
                            string.Join(", ",
                                result.Errors.Select(e => e.Description)));
                    }

                    await userManager.AddToRoleAsync(
                        adminUser,
                        "Admin");
                }
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }
    }
}