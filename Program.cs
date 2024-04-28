using BarRating.Data;
using BarRating.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BarRating
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 0;
            });

            builder.Services.AddControllersWithViews();

            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var roles = new[]
                {
                    "Administrator",
                    "User"
                };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
            }

            using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                if (await userManager.FindByNameAsync("admin") == null)
                {
                    var admin = new User()
                    {
                        UserName = "admin",
                        FirstName = "Admin",
                        LastName = "Admin"
                    };

                    await userManager.CreateAsync(admin, "admin");
                    await userManager.AddToRoleAsync(admin, "Administrator");
                }

                if (await userManager.FindByNameAsync("suatalikoch") == null)
                {
                    var user = new User()
                    {
                        UserName = "suatalikoch",
                        FirstName = "Suat",
                        LastName = "Alikoch"
                    };

                    await userManager.CreateAsync(user, "suatalikoch");
                    await userManager.AddToRoleAsync(user, "User");
                }

                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                if (!context.Bars.Any())
                {
                    var bars = new List<Bar>
                    {
                        new()
                        {
                            Name = "Elinor",
                            Description = "Summer time place!",
                            ImageLocation = "/images/1feb703d-2f76-4b1a-a884-13759fb1ae32_elinor.jpg"
                        },
                        new()
                        {
                            Name = "Sense",
                            Description = "All time party place!",
                            ImageLocation = "/images/812fff6d-fa83-4e15-8597-56474aaa2cad_sense.jpg"
                        }
                    };

                    await context.Bars.AddRangeAsync(bars);
                    await context.SaveChangesAsync();
                }

                if (!context.Reviews.Any())
                {
                    var reviews = new List<Review>()
                    {
                        new()
                        {
                            Rating = 5,
                            Description = "I like the place so much!",
                            UserId = context.Users.Where(u => u.UserName == "suatalikoch").FirstOrDefault().Id,
                            BarId = 1,
                        },
                        new()
                        {
                            Rating = 3,
                            Description = "Bar Sense is controversial I can say.",
                            UserId = context.Users.Where(u => u.UserName == "suatalikoch").FirstOrDefault().Id,
                            BarId = 2,
                        },
                        new()
                        {
                            Rating = 1,
                            Description = "I didn't like this place at all! :(",
                            UserId = context.Users.Where(u => u.UserName == "admin").FirstOrDefault().Id,
                            BarId = 2,
                        }
                    };

                    await context.Reviews.AddRangeAsync(reviews);
                    await context.SaveChangesAsync();
                }
            }

            app.Run();
        }
    }
}
