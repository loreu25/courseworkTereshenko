using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using UniversitySystem2;
using System.Linq;

namespace UniversitySystem2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<UniversityContext>();
                    context.Database.EnsureCreated();
                    // Проверяем, есть ли администратор
                    if (!context.Users.Any(u => u.Role == UniversitySystem2.Models.UserRole.Администратор))
                    {
                        context.Users.Add(new UniversitySystem2.Models.User
                        {
                            Login = "admin",
                            Password = "admin123",
                            FirstName = "Admin",
                            LastName = "Admin",
                            Role = UniversitySystem2.Models.UserRole.Администратор
                        });
                        context.SaveChanges();
                    }
                }
                catch { /* Ошибки инициализации можно логировать */ }
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
